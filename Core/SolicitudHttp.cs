using System.Text;

namespace HttpServer.Core;

/// <summary>
/// Línea de petición y cabeceras de una solicitud HTTP/1.x.
/// </summary>
public sealed class SolicitudHttp
{
    private static readonly byte[] FinCabeceras = "\r\n\r\n"u8.ToArray();

    private SolicitudHttp(string metodo, string objetivo, string ruta, string version, Dictionary<string, string> cabeceras)
    {
        Metodo = metodo;
        Objetivo = objetivo;
        Ruta = ruta;
        Version = version;
        Cabeceras = cabeceras;
    }

    public string Metodo { get; }

    /// <summary>Objetivo tal como llega en la línea de petición (puede incluir query string).</summary>
    public string Objetivo { get; }

    /// <summary>Ruta decodificada y sin query string.</summary>
    public string Ruta { get; }

    public string Version { get; }

    public IReadOnlyDictionary<string, string> Cabeceras { get; }

    public string? Cabecera(string nombre) => Cabeceras.TryGetValue(nombre, out string? valor) ? valor : null;

    /// <summary>
    /// Lee la cabecera de una solicitud desde el flujo. Devuelve <c>null</c> si el cliente cierra la
    /// conexión antes de enviar nada y lanza <see cref="FormatException"/> si la solicitud no es válida.
    /// </summary>
    public static async Task<SolicitudHttp?> LeerAsync(Stream flujo, int longitudMaxima, CancellationToken cancelacion)
    {
        byte[] buffer = new byte[longitudMaxima];
        int leidos = 0;
        int fin;

        while ((fin = buffer.AsSpan(0, leidos).IndexOf(FinCabeceras)) < 0)
        {
            if (leidos == buffer.Length)
            {
                throw new FormatException("Cabeceras demasiado grandes.");
            }

            int n = await flujo.ReadAsync(buffer.AsMemory(leidos), cancelacion).ConfigureAwait(false);
            if (n == 0)
            {
                return leidos == 0 ? null : throw new FormatException("Solicitud incompleta.");
            }

            leidos += n;
        }

        string texto = Encoding.Latin1.GetString(buffer, 0, fin);
        string[] lineas = texto.Split("\r\n");

        string[] partes = lineas[0].Split(' ');
        if (partes.Length != 3 || !partes[2].StartsWith("HTTP/1.", StringComparison.Ordinal))
        {
            throw new FormatException("Línea de petición no válida.");
        }

        Dictionary<string, string> cabeceras = new(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i < lineas.Length; i++)
        {
            int separador = lineas[i].IndexOf(':');
            if (separador <= 0)
            {
                continue;
            }

            cabeceras[lineas[i][..separador].Trim()] = lineas[i][(separador + 1)..].Trim();
        }

        string objetivo = partes[1];
        string ruta = objetivo;
        int query = ruta.IndexOfAny(['?', '#']);
        if (query >= 0)
        {
            ruta = ruta[..query];
        }

        if (!ruta.StartsWith('/'))
        {
            // Forma absoluta (http://host/ruta): nos quedamos solo con la ruta.
            ruta = Uri.TryCreate(ruta, UriKind.Absolute, out Uri? uri) ? uri.AbsolutePath : "/";
        }

        return new SolicitudHttp(partes[0].ToUpperInvariant(), objetivo, Uri.UnescapeDataString(ruta), partes[2], cabeceras);
    }
}
