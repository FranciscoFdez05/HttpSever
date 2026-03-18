using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public partial class Form1 : Form
    {
        private string? archivoSeleccionado;
        private TcpListener? servidorTcp;
        private CancellationTokenSource? cancelacionServidor;
        private Task? tareaServidor;
        private bool servidorActivo;
        private bool servidorDeteniendose;

        public Form1()
        {
            InitializeComponent();
            inicializarFormulario();
        }

        private void inicializarFormulario()
        {
            Text = "HttpServer";

            textBoxComando.ReadOnly = true;
            textBoxSubirArchivo.ReadOnly = true;
            btnCopiar.Text = "Copiar";

            richTextBoxTerminal.ReadOnly = true;
            richTextBoxTerminal.BackColor = Color.Black;
            richTextBoxTerminal.ForeColor = Color.Lime;
            richTextBoxTerminal.Font = new Font("Consolas", 10);
            richTextBoxTerminal.Clear();

            btnServidorUp.Click += btnServidorUp_Click;
            btnServerDown.Click += btnServerDown_Click;
            BtnsubirArchivo.Click += btnSubirArchivo_Click;
            btnCopiar.Click += btnCopiar_Click;
            FormClosing += form1_FormClosing;

            actualizarEstadoBotones();
            agregarLogInfo("Aplicación iniciada.");
        }

        private async void btnServidorUp_Click(object? sender, EventArgs e)
        {
            try
            {
                if (servidorDeteniendose)
                {
                    agregarLogWarning("El servidor se está deteniendo.");
                    return;
                }

                if (servidorActivo)
                {
                    agregarLogWarning("El servidor ya está activo.");
                    return;
                }

                if (!int.TryParse(puerto.Text.Trim(), out int puertoServidor) || puertoServidor < 1 || puertoServidor > 65535)
                {
                    agregarLogError("Puerto inválido. Debe estar entre 1 y 65535.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(archivoSeleccionado) || !File.Exists(archivoSeleccionado))
                {
                    agregarLogError("No hay archivo válido seleccionado.");
                    return;
                }

                if (puertoEnUso(puertoServidor))
                {
                    agregarLogError($"El puerto {puertoServidor} ya está en uso.");
                    return;
                }

                cancelacionServidor = new CancellationTokenSource();
                servidorTcp = new TcpListener(IPAddress.Any, puertoServidor);
                servidorTcp.Start();

                servidorActivo = true;
                actualizarEstadoBotones();

                tareaServidor = Task.Run(() => bucleServidorAsync(cancelacionServidor.Token), cancelacionServidor.Token);

                generarComandoCliente(puertoServidor);

                agregarLogInfo($"Servidor levantado en el puerto {puertoServidor}.");
                agregarLogInfo($"Archivo publicado: {Path.GetFileName(archivoSeleccionado)}");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                agregarLogError($"Error al levantar el servidor: {ex.Message}");
                await detenerServidorInternoAsync(false);
            }
        }

        private async void btnServerDown_Click(object? sender, EventArgs e)
        {
            await pararServidorAsync();
        }

        private void btnSubirArchivo_Click(object? sender, EventArgs e)
        {
            try
            {
                using OpenFileDialog openFileDialog = new()
                {
                    Title = "Selecciona un archivo",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                archivoSeleccionado = openFileDialog.FileName;
                textBoxSubirArchivo.Text = archivoSeleccionado;
                textBoxComando.Text = string.Empty;
            }
            catch (Exception ex)
            {
                agregarLogError($"Error al seleccionar el archivo: {ex.Message}");
            }
        }

        private void btnCopiar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxComando.Text))
                {
                    return;
                }

                Clipboard.SetText(textBoxComando.Text);
            }
            catch (Exception ex)
            {
                agregarLogError($"Error al copiar el comando: {ex.Message}");
            }
        }

        private bool puertoEnUso(int puertoServidor)
        {
            try
            {
                IPGlobalProperties propiedades = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endpoints = propiedades.GetActiveTcpListeners();

                foreach (IPEndPoint endpoint in endpoints)
                {
                    if (endpoint.Port == puertoServidor)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                agregarLogError($"Error al comprobar el puerto: {ex.Message}");
                return true;
            }
        }

        private async Task bucleServidorAsync(CancellationToken cancellationToken)
        {
            agregarLogNormal("[OUT] Servidor HTTP escuchando en 0.0.0.0");

            try
            {
                while (!cancellationToken.IsCancellationRequested && servidorTcp != null)
                {
                    TcpClient cliente;

                    try
                    {
                        cliente = await servidorTcp.AcceptTcpClientAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            agregarLogError($"Error aceptando conexión: {ex.Message}");
                        }

                        continue;
                    }

                    _ = Task.Run(() => manejarClienteAsync(cliente, cancellationToken), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    agregarLogError($"Error en el servidor: {ex.Message}");
                }
            }
        }

        private async Task manejarClienteAsync(TcpClient cliente, CancellationToken cancellationToken)
        {
            using (cliente)
            {
                try
                {
                    using NetworkStream networkStream = cliente.GetStream();
                    using StreamReader streamReader = new(networkStream, Encoding.ASCII, false, 8192, true);

                    string? requestLine = await streamReader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(requestLine))
                    {
                        return;
                    }

                    string? headerLine;
                    do
                    {
                        headerLine = await streamReader.ReadLineAsync();
                    }
                    while (!string.IsNullOrEmpty(headerLine));

                    string[] partes = requestLine.Split(' ');

                    if (partes.Length < 2)
                    {
                        await enviarRespuestaSimpleAsync(networkStream, 400, "Bad Request", "Solicitud inválida.", "text/plain; charset=utf-8", cancellationToken);
                        return;
                    }

                    string metodo = partes[0].Trim().ToUpperInvariant();
                    string rutaSolicitada = partes[1].Trim();

                    string ipCliente = "desconocida";
                    try
                    {
                        if (cliente.Client.RemoteEndPoint is IPEndPoint endPoint)
                        {
                            ipCliente = endPoint.Address.ToString();
                        }
                    }
                    catch
                    {
                    }

                    agregarLogNormal($"[HTTP] {ipCliente} \"{requestLine}\"");

                    if (string.IsNullOrWhiteSpace(archivoSeleccionado) || !File.Exists(archivoSeleccionado))
                    {
                        await enviarRespuestaSimpleAsync(networkStream, 404, "Not Found", "Archivo no disponible.", "text/plain; charset=utf-8", cancellationToken);
                        return;
                    }

                    string nombreArchivo = Path.GetFileName(archivoSeleccionado);
                    string rutaEsperada = "/" + Uri.EscapeDataString(nombreArchivo);

                    if (rutaSolicitada == "/")
                    {
                        string html = generarPaginaIndice(nombreArchivo);
                        await enviarRespuestaSimpleAsync(networkStream, 200, "OK", html, "text/html; charset=utf-8", cancellationToken);
                        return;
                    }

                    if (rutaSolicitada != rutaEsperada)
                    {
                        await enviarRespuestaSimpleAsync(networkStream, 404, "Not Found", "Archivo no encontrado.", "text/plain; charset=utf-8", cancellationToken);
                        return;
                    }

                    if (metodo != "GET" && metodo != "HEAD")
                    {
                        await enviarRespuestaSimpleAsync(networkStream, 405, "Method Not Allowed", "Método no permitido.", "text/plain; charset=utf-8", cancellationToken);
                        return;
                    }

                    await enviarArchivoAsync(networkStream, archivoSeleccionado, metodo == "HEAD", cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (IOException)
                {
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        agregarLogError($"Error manejando cliente: {ex.Message}");
                    }
                }
            }
        }

        private string generarPaginaIndice(string nombreArchivo)
        {
            string nombreArchivoCodificado = Uri.EscapeDataString(nombreArchivo);

            return """
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <title>HttpServer</title>
</head>
<body>
    <h1>Archivo disponible</h1>
    <p><a href="%ARCHIVO_URL%">%ARCHIVO_NOMBRE%</a></p>
</body>
</html>
"""
            .Replace("%ARCHIVO_URL%", nombreArchivoCodificado)
            .Replace("%ARCHIVO_NOMBRE%", WebUtility.HtmlEncode(nombreArchivo));
        }

        private async Task enviarArchivoAsync(NetworkStream networkStream, string rutaArchivo, bool soloCabeceras, CancellationToken cancellationToken)
        {
            FileInfo fileInfo = new(rutaArchivo);
            string contentType = obtenerContentType(fileInfo.Extension);
            string headers =
                $"HTTP/1.1 200 OK\r\n" +
                $"Content-Type: {contentType}\r\n" +
                $"Content-Length: {fileInfo.Length}\r\n" +
                $"Connection: close\r\n" +
                $"\r\n";

            byte[] headerBytes = Encoding.UTF8.GetBytes(headers);
            await networkStream.WriteAsync(headerBytes, cancellationToken);

            if (soloCabeceras)
            {
                await networkStream.FlushAsync(cancellationToken);
                return;
            }

            using FileStream fileStream = File.OpenRead(rutaArchivo);
            await fileStream.CopyToAsync(networkStream, cancellationToken);
            await networkStream.FlushAsync(cancellationToken);
        }

        private async Task enviarRespuestaSimpleAsync(NetworkStream networkStream, int statusCode, string statusText, string cuerpo, string contentType, CancellationToken cancellationToken)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(cuerpo);

            string headers =
                $"HTTP/1.1 {statusCode} {statusText}\r\n" +
                $"Content-Type: {contentType}\r\n" +
                $"Content-Length: {bodyBytes.Length}\r\n" +
                $"Connection: close\r\n" +
                $"\r\n";

            byte[] headerBytes = Encoding.UTF8.GetBytes(headers);

            await networkStream.WriteAsync(headerBytes, cancellationToken);
            await networkStream.WriteAsync(bodyBytes, cancellationToken);
            await networkStream.FlushAsync(cancellationToken);
        }

        private string obtenerContentType(string extension)
        {
            string extensionNormalizada = extension.ToLowerInvariant();

            return extensionNormalizada switch
            {
                ".txt" => "text/plain; charset=utf-8",
                ".json" => "application/json; charset=utf-8",
                ".html" => "text/html; charset=utf-8",
                ".htm" => "text/html; charset=utf-8",
                ".csv" => "text/csv; charset=utf-8",
                ".xml" => "application/xml; charset=utf-8",
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".mp4" => "video/mp4",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        private async Task pararServidorAsync()
        {
            if (servidorDeteniendose)
            {
                return;
            }

            if (!servidorActivo)
            {
                actualizarEstadoBotones();
                return;
            }

            servidorDeteniendose = true;
            actualizarEstadoBotones();
            agregarLogInfo("Deteniendo servidor...");

            await detenerServidorInternoAsync(true);

            servidorDeteniendose = false;
            actualizarEstadoBotones();
        }

        private async Task detenerServidorInternoAsync(bool escribirLogFinal)
        {
            try
            {
                try
                {
                    cancelacionServidor?.Cancel();
                }
                catch
                {
                }

                try
                {
                    servidorTcp?.Stop();
                }
                catch
                {
                }

                if (tareaServidor != null)
                {
                    try
                    {
                        await Task.WhenAny(tareaServidor, Task.Delay(2000));
                    }
                    catch
                    {
                    }
                }

                servidorTcp = null;
                cancelacionServidor?.Dispose();
                cancelacionServidor = null;
                tareaServidor = null;
                servidorActivo = false;

                if (escribirLogFinal)
                {
                    agregarLogInfo("Servidor detenido correctamente.");
                }
            }
            catch (Exception ex)
            {
                agregarLogError($"Error al detener el servidor: {ex.Message}");
            }
        }

        private void generarComandoCliente(int puertoServidor)
        {
            if (string.IsNullOrWhiteSpace(archivoSeleccionado))
            {
                textBoxComando.Text = string.Empty;
                return;
            }

            string nombreArchivo = Path.GetFileName(archivoSeleccionado);
            string nombreArchivoFormateado = Uri.EscapeDataString(nombreArchivo);
            string ipLocal = obtenerPrimeraIpLocalValida() ?? "127.0.0.1";

            textBoxComando.Text = $"wget \"http://{ipLocal}:{puertoServidor}/{nombreArchivoFormateado}\"";
        }

        private string? obtenerPrimeraIpLocalValida()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                IPInterfaceProperties propiedades = networkInterface.GetIPProperties();

                foreach (UnicastIPAddressInformation direccion in propiedades.UnicastAddresses)
                {
                    if (direccion.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    string ipActual = direccion.Address.ToString();

                    if (ipActual.StartsWith("192.168.") || ipActual.StartsWith("10.") || ipActual.StartsWith("172."))
                    {
                        return ipActual;
                    }
                }
            }

            return null;
        }

        private void actualizarEstadoBotones()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(actualizarEstadoBotones));
                    return;
                }

                btnServidorUp.Enabled = !servidorActivo && !servidorDeteniendose;
                btnServerDown.Enabled = servidorActivo && !servidorDeteniendose;
            }
            catch
            {
            }
        }

        private void agregarLogNormal(string texto)
        {
            agregarLog(texto, Color.Lime);
        }

        private void agregarLogInfo(string texto)
        {
            agregarLog($"[INFO] {texto}", Color.DeepSkyBlue);
        }

        private void agregarLogWarning(string texto)
        {
            agregarLog($"[WARN] {texto}", Color.Gold);
        }

        private void agregarLogError(string texto)
        {
            agregarLog($"[ERROR] {texto}", Color.OrangeRed);
        }

        private void agregarLog(string texto, Color color)
        {
            try
            {
                if (richTextBoxTerminal.InvokeRequired)
                {
                    richTextBoxTerminal.Invoke(new Action<string, Color>(agregarLog), texto, color);
                    return;
                }

                richTextBoxTerminal.SelectionStart = richTextBoxTerminal.TextLength;
                richTextBoxTerminal.SelectionLength = 0;
                richTextBoxTerminal.SelectionColor = color;
                richTextBoxTerminal.AppendText(texto + Environment.NewLine);
                richTextBoxTerminal.SelectionColor = richTextBoxTerminal.ForeColor;
                richTextBoxTerminal.ScrollToCaret();
            }
            catch
            {
            }
        }

        private async void form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (servidorActivo)
                {
                    e.Cancel = true;
                    Hide();
                    await detenerServidorInternoAsync(false);
                    Close();
                    return;
                }
            }
            catch
            {
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}