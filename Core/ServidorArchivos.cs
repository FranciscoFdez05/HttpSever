using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace HttpServer.Core;

/// <summary>
/// Servidor HTTP/1.1 mínimo que publica un único archivo en la red local.
/// </summary>
/// <remarks>
/// Rutas disponibles:
/// <list type="bullet">
///   <item><c>/</c>: página HTML con el enlace de descarga.</item>
///   <item><c>/{nombre-del-archivo}</c>: el archivo, con soporte de descargas parciales (Range) para reanudar.</item>
/// </list>
/// Cada conexión atiende una única petición (<c>Connection: close</c>).
/// </remarks>
public sealed class ServidorArchivos : IAsyncDisposable
{
    private const int LongitudMaximaCabeceras = 16 * 1024;
    private const int TamanoBuffer = 128 * 1024;
    private static readonly TimeSpan TiempoMaximoCabeceras = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan TiempoMaximoParada = TimeSpan.FromSeconds(3);
    private static readonly string NombreServidor =
        $"HttpServer/{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0"}";

    private readonly ConcurrentDictionary<long, Task> conexiones = new();
    private TcpListener? listener;
    private CancellationTokenSource? cancelacion;
    private Task? tareaAceptacion;
    private long siguienteIdConexion;
    private int descargasCompletadas;
    private long bytesEnviados;

    public ServidorArchivos(string rutaArchivo, int puerto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
        ArgumentOutOfRangeException.ThrowIfLessThan(puerto, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(puerto, IPEndPoint.MaxPort);

        RutaArchivo = Path.GetFullPath(rutaArchivo);
        Puerto = puerto;
    }

    /// <summary>
    /// Se produce cada vez que el servidor genera una entrada de registro.
    /// Se invoca desde hilos del pool, no desde el hilo de la interfaz.
    /// </summary>
    public event EventHandler<RegistroEventArgs>? Registro;

    public string RutaArchivo { get; }

    public string NombreArchivo => Path.GetFileName(RutaArchivo);

    public int Puerto { get; }

    public bool Activo => Volatile.Read(ref listener) != null;

    public int ConexionesActivas => conexiones.Count;

    public int DescargasCompletadas => Volatile.Read(ref descargasCompletadas);

    public long BytesEnviados => Interlocked.Read(ref bytesEnviados);

    /// <summary>
    /// Empieza a escuchar en todas las interfaces IPv4.
    /// </summary>
    /// <exception cref="FileNotFoundException">El archivo ya no existe.</exception>
    /// <exception cref="SocketException">El puerto está ocupado o no se tiene permiso para usarlo.</exception>
    public void Iniciar()
    {
        if (listener != null)
        {
            throw new InvalidOperationException("El servidor ya está iniciado.");
        }

        FileInfo archivo = new(RutaArchivo);
        if (!archivo.Exists)
        {
            throw new FileNotFoundException("El archivo seleccionado no existe.", RutaArchivo);
        }

        TcpListener nuevo = new(IPAddress.Any, Puerto) { ExclusiveAddressUse = true };
        nuevo.Start();

        CancellationTokenSource cts = new();
        cancelacion = cts;
        listener = nuevo;
        tareaAceptacion = Task.Run(() => BucleAceptacionAsync(nuevo, cts.Token));

        Registrar(NivelRegistro.Exito, $"Servidor escuchando en 0.0.0.0:{Puerto}");
        Registrar(NivelRegistro.Info, $"Archivo publicado: {archivo.Name} ({UtilidadesRed.FormatearTamano(archivo.Length)})");
    }

    /// <summary>
    /// Deja de aceptar conexiones y cancela las descargas en curso. Es seguro llamarlo varias veces.
    /// </summary>
    public async Task DetenerAsync()
    {
        TcpListener? actual = Interlocked.Exchange(ref listener, null);
        if (actual == null)
        {
            return;
        }

        cancelacion?.Cancel();
        actual.Stop();

        Task pendientes = Task.WhenAll(conexiones.Values.Append(tareaAceptacion ?? Task.CompletedTask));
        try
        {
            await pendientes.WaitAsync(TiempoMaximoParada).ConfigureAwait(false);
            cancelacion?.Dispose();
        }
        catch (TimeoutException)
        {
            Registrar(NivelRegistro.Advertencia, "Algunas conexiones no se cerraron a tiempo.");
        }

        cancelacion = null;
        tareaAceptacion = null;
        Registrar(NivelRegistro.Info, "Servidor detenido.");
    }

    public async ValueTask DisposeAsync() => await DetenerAsync().ConfigureAwait(false);

    private async Task BucleAceptacionAsync(TcpListener escucha, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            TcpClient cliente;
            try
            {
                cliente = await escucha.AcceptTcpClientAsync(token).ConfigureAwait(false);
            }
            catch (Exception ex) when (token.IsCancellationRequested || ex is ObjectDisposedException)
            {
                break;
            }
            catch (SocketException ex)
            {
                Registrar(NivelRegistro.Error, $"Error aceptando conexión: {ex.Message}");
                continue;
            }

            long id = Interlocked.Increment(ref siguienteIdConexion);
            Task tarea = Task.Run(() => AtenderClienteAsync(cliente, token), CancellationToken.None);
            conexiones[id] = tarea;
            _ = tarea.ContinueWith(_ => conexiones.TryRemove(id, out Task? _), TaskScheduler.Default);
        }
    }

    private async Task AtenderClienteAsync(TcpClient cliente, CancellationToken token)
    {
        using (cliente)
        {
            string ip = (cliente.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "desconocida";

            try
            {
                cliente.NoDelay = true;
                NetworkStream flujo = cliente.GetStream();

                SolicitudHttp? solicitud;
                using (CancellationTokenSource limite = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    limite.CancelAfter(TiempoMaximoCabeceras);
                    try
                    {
                        solicitud = await SolicitudHttp.LeerAsync(flujo, LongitudMaximaCabeceras, limite.Token).ConfigureAwait(false);
                    }
                    catch (FormatException ex)
                    {
                        Registrar(NivelRegistro.Advertencia, $"{ip} solicitud no válida: {ex.Message}");
                        await EnviarTextoAsync(flujo, 400, "Bad Request", "Solicitud no válida.", false, token).ConfigureAwait(false);
                        return;
                    }
                    catch (OperationCanceledException) when (!token.IsCancellationRequested)
                    {
                        Registrar(NivelRegistro.Advertencia, $"{ip} no envió la solicitud a tiempo; conexión cerrada.");
                        return;
                    }
                }

                if (solicitud != null)
                {
                    await ProcesarAsync(solicitud, flujo, ip, token).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or IOException or SocketException)
            {
                // El cliente cerró la conexión o el servidor se está deteniendo.
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    Registrar(NivelRegistro.Error, $"Error atendiendo a {ip}: {ex.Message}");
                }
            }
        }
    }

    private async Task ProcesarAsync(SolicitudHttp solicitud, NetworkStream flujo, string ip, CancellationToken token)
    {
        string resumen = $"{ip} \"{solicitud.Metodo} {solicitud.Ruta}\"";
        bool soloCabeceras = solicitud.Metodo == "HEAD";

        if (solicitud.Metodo is not ("GET" or "HEAD"))
        {
            Registrar(NivelRegistro.Advertencia, $"{resumen} 405");
            await EnviarTextoAsync(flujo, 405, "Method Not Allowed", "Método no permitido.", false, token, "Allow: GET, HEAD").ConfigureAwait(false);
            return;
        }

        if (solicitud.Ruta == "/favicon.ico")
        {
            await EnviarTextoAsync(flujo, 404, "Not Found", "No encontrado.", soloCabeceras, token).ConfigureAwait(false);
            return;
        }

        FileInfo archivo = new(RutaArchivo);
        if (!archivo.Exists)
        {
            Registrar(NivelRegistro.Error, $"{resumen} 404 — el archivo publicado ya no existe en disco.");
            await EnviarTextoAsync(flujo, 404, "Not Found", "Archivo no disponible.", soloCabeceras, token).ConfigureAwait(false);
            return;
        }

        if (solicitud.Ruta == "/")
        {
            byte[] html = Encoding.UTF8.GetBytes(PaginaIndice.Generar(archivo));
            await EnviarRespuestaAsync(flujo, 200, "OK", "text/html; charset=utf-8", html, soloCabeceras, token).ConfigureAwait(false);
            Registrar(NivelRegistro.Peticion, $"{resumen} 200");
            return;
        }

        if (!string.Equals(solicitud.Ruta, "/" + archivo.Name, StringComparison.Ordinal))
        {
            Registrar(NivelRegistro.Advertencia, $"{resumen} 404");
            await EnviarTextoAsync(flujo, 404, "Not Found", "Archivo no encontrado.", soloCabeceras, token).ConfigureAwait(false);
            return;
        }

        await EnviarArchivoAsync(solicitud, flujo, archivo, resumen, soloCabeceras, token).ConfigureAwait(false);
    }

    private async Task EnviarArchivoAsync(SolicitudHttp solicitud, NetworkStream flujo, FileInfo archivo, string resumen, bool soloCabeceras, CancellationToken token)
    {
        FileStream lectura;
        try
        {
            lectura = new FileStream(archivo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                TamanoBuffer, FileOptions.Asynchronous | FileOptions.SequentialScan);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Registrar(NivelRegistro.Error, $"{resumen} 500 — no se pudo abrir el archivo: {ex.Message}");
            await EnviarTextoAsync(flujo, 500, "Internal Server Error", "No se pudo leer el archivo.", soloCabeceras, token).ConfigureAwait(false);
            return;
        }

        await using (lectura.ConfigureAwait(false))
        {
            long longitud = lectura.Length;
            DateTime modificado = archivo.LastWriteTimeUtc;
            string ultimaModificacion = modificado.ToString("R", CultureInfo.InvariantCulture);
            string etag = $"\"{modificado.Ticks:x}-{longitud:x}\"";

            long inicio = 0;
            long fin = longitud - 1;
            bool parcial = false;

            string? rango = solicitud.Cabecera("Range");
            string? siRango = solicitud.Cabecera("If-Range");
            if (rango != null && (siRango == null || siRango == etag || siRango == ultimaModificacion))
            {
                switch (InterpretarRango(rango, longitud, out long inicioRango, out long finRango))
                {
                    case ResultadoRango.Valido:
                        (inicio, fin, parcial) = (inicioRango, finRango, true);
                        break;

                    case ResultadoRango.NoSatisfacible:
                        Registrar(NivelRegistro.Advertencia, $"{resumen} 416");
                        await EnviarTextoAsync(flujo, 416, "Range Not Satisfiable", "Rango no satisfacible.", soloCabeceras, token,
                            $"Content-Range: bytes */{longitud}").ConfigureAwait(false);
                        return;
                }
            }

            long cantidad = Math.Max(0, fin - inicio + 1);

            StringBuilder cabeceras = new();
            cabeceras.Append(parcial ? "HTTP/1.1 206 Partial Content\r\n" : "HTTP/1.1 200 OK\r\n");
            AgregarCabecerasComunes(cabeceras, TiposMime.Obtener(archivo.Extension), cantidad);
            cabeceras.Append("Content-Disposition: ").Append(ContentDisposition(archivo.Name)).Append("\r\n");
            cabeceras.Append("Accept-Ranges: bytes\r\n");
            cabeceras.Append("Last-Modified: ").Append(ultimaModificacion).Append("\r\n");
            cabeceras.Append("ETag: ").Append(etag).Append("\r\n");
            if (parcial)
            {
                cabeceras.Append(CultureInfo.InvariantCulture, $"Content-Range: bytes {inicio}-{fin}/{longitud}\r\n");
            }
            cabeceras.Append("\r\n");

            await flujo.WriteAsync(Encoding.ASCII.GetBytes(cabeceras.ToString()), token).ConfigureAwait(false);

            int codigo = parcial ? 206 : 200;
            if (soloCabeceras)
            {
                Registrar(NivelRegistro.Peticion, $"{resumen} {codigo}");
                return;
            }

            string detalle = parcial ? $" desde el byte {inicio:N0}" : string.Empty;
            Registrar(NivelRegistro.Peticion, $"{resumen} {codigo} — enviando {UtilidadesRed.FormatearTamano(cantidad)}{detalle}");

            lectura.Seek(inicio, SeekOrigin.Begin);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(TamanoBuffer);
            long restante = cantidad;
            Stopwatch cronometro = Stopwatch.StartNew();

            try
            {
                while (restante > 0)
                {
                    int leidos = await lectura.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, restante)), token).ConfigureAwait(false);
                    if (leidos == 0)
                    {
                        break;
                    }

                    await flujo.WriteAsync(buffer.AsMemory(0, leidos), token).ConfigureAwait(false);
                    restante -= leidos;
                    Interlocked.Add(ref bytesEnviados, leidos);
                }
            }
            catch (Exception ex) when (ex is IOException or SocketException or OperationCanceledException)
            {
                // Se informa más abajo como descarga interrumpida.
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            cronometro.Stop();
            long enviados = cantidad - restante;
            string ip = resumen[..resumen.IndexOf(' ')];

            if (restante > 0)
            {
                Registrar(NivelRegistro.Advertencia,
                    $"{ip} descarga interrumpida tras {UtilidadesRed.FormatearTamano(enviados)} de {UtilidadesRed.FormatearTamano(cantidad)}");
                return;
            }

            Interlocked.Increment(ref descargasCompletadas);
            double segundos = Math.Max(cronometro.Elapsed.TotalSeconds, 0.001);
            Registrar(NivelRegistro.Exito,
                $"{ip} descarga completada: {UtilidadesRed.FormatearTamano(enviados)} en {segundos:0.0} s " +
                $"({UtilidadesRed.FormatearTamano((long)(enviados / segundos))}/s)");
        }
    }

    private enum ResultadoRango
    {
        Ignorar,
        Valido,
        NoSatisfacible
    }

    /// <summary>
    /// Interpreta una cabecera <c>Range</c> de un único rango (RFC 9110 §14.1.2).
    /// Los rangos múltiples o mal formados se ignoran y se sirve el archivo completo.
    /// </summary>
    private static ResultadoRango InterpretarRango(string valor, long longitud, out long inicio, out long fin)
    {
        inicio = 0;
        fin = longitud - 1;

        const string prefijo = "bytes=";
        if (!valor.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
        {
            return ResultadoRango.Ignorar;
        }

        string especificacion = valor[prefijo.Length..].Trim();
        int guion = especificacion.IndexOf('-');
        if (guion < 0 || especificacion.Contains(','))
        {
            return ResultadoRango.Ignorar;
        }

        string desde = especificacion[..guion].Trim();
        string hasta = especificacion[(guion + 1)..].Trim();

        if (desde.Length == 0)
        {
            // bytes=-N: los últimos N bytes
            if (!long.TryParse(hasta, NumberStyles.None, CultureInfo.InvariantCulture, out long sufijo))
            {
                return ResultadoRango.Ignorar;
            }

            if (sufijo == 0 || longitud == 0)
            {
                return ResultadoRango.NoSatisfacible;
            }

            inicio = Math.Max(0, longitud - sufijo);
            return ResultadoRango.Valido;
        }

        if (!long.TryParse(desde, NumberStyles.None, CultureInfo.InvariantCulture, out inicio))
        {
            return ResultadoRango.Ignorar;
        }

        if (hasta.Length > 0)
        {
            if (!long.TryParse(hasta, NumberStyles.None, CultureInfo.InvariantCulture, out long finSolicitado) || finSolicitado < inicio)
            {
                return ResultadoRango.Ignorar;
            }

            fin = Math.Min(finSolicitado, longitud - 1);
        }

        return inicio >= longitud ? ResultadoRango.NoSatisfacible : ResultadoRango.Valido;
    }

    /// <summary>
    /// Cabecera Content-Disposition con nombre ASCII de respaldo y nombre UTF-8 (RFC 6266).
    /// </summary>
    private static string ContentDisposition(string nombre)
    {
        StringBuilder ascii = new(nombre.Length);
        foreach (char c in nombre)
        {
            ascii.Append(c is >= ' ' and <= '~' and not '"' and not '\\' ? c : '_');
        }

        return $"attachment; filename=\"{ascii}\"; filename*=UTF-8''{Uri.EscapeDataString(nombre)}";
    }

    private static void AgregarCabecerasComunes(StringBuilder cabeceras, string tipoContenido, long longitudContenido)
    {
        cabeceras.Append("Server: ").Append(NombreServidor).Append("\r\n");
        cabeceras.Append("Date: ").Append(DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture)).Append("\r\n");
        cabeceras.Append("Content-Type: ").Append(tipoContenido).Append("\r\n");
        cabeceras.Append(CultureInfo.InvariantCulture, $"Content-Length: {longitudContenido}\r\n");
        cabeceras.Append("Cache-Control: no-cache\r\n");
        cabeceras.Append("X-Content-Type-Options: nosniff\r\n");
        cabeceras.Append("Connection: close\r\n");
    }

    private static Task EnviarTextoAsync(NetworkStream flujo, int codigo, string estado, string texto, bool soloCabeceras,
        CancellationToken token, params string[] cabecerasExtra) =>
        EnviarRespuestaAsync(flujo, codigo, estado, "text/plain; charset=utf-8", Encoding.UTF8.GetBytes(texto), soloCabeceras, token, cabecerasExtra);

    private static async Task EnviarRespuestaAsync(NetworkStream flujo, int codigo, string estado, string tipoContenido, byte[] cuerpo,
        bool soloCabeceras, CancellationToken token, params string[] cabecerasExtra)
    {
        StringBuilder cabeceras = new();
        cabeceras.Append(CultureInfo.InvariantCulture, $"HTTP/1.1 {codigo} {estado}\r\n");
        AgregarCabecerasComunes(cabeceras, tipoContenido, cuerpo.Length);
        foreach (string extra in cabecerasExtra)
        {
            cabeceras.Append(extra).Append("\r\n");
        }
        cabeceras.Append("\r\n");

        await flujo.WriteAsync(Encoding.ASCII.GetBytes(cabeceras.ToString()), token).ConfigureAwait(false);
        if (!soloCabeceras)
        {
            await flujo.WriteAsync(cuerpo, token).ConfigureAwait(false);
        }
    }

    private void Registrar(NivelRegistro nivel, string mensaje) => Registro?.Invoke(this, new RegistroEventArgs(nivel, mensaje));
}
