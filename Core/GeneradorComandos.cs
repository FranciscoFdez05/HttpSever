namespace HttpServer.Core;

/// <summary>
/// Herramienta con la que el equipo cliente descargará el archivo.
/// </summary>
public enum TipoComando
{
    Wget,
    Curl,
    PowerShell,
    Certutil,
    Url
}

/// <summary>
/// Genera comandos de descarga listos para pegar en el equipo cliente.
/// </summary>
public static class GeneradorComandos
{
    public static string Descripcion(TipoComando tipo) => tipo switch
    {
        TipoComando.Wget => "wget (Linux/macOS)",
        TipoComando.Curl => "curl (Linux/macOS)",
        TipoComando.PowerShell => "PowerShell (Windows)",
        TipoComando.Certutil => "certutil (Windows)",
        TipoComando.Url => "Solo URL",
        _ => tipo.ToString()
    };

    public static string ConstruirUrl(string host, int puerto, string nombreArchivo) =>
        $"http://{host}:{puerto}/{Uri.EscapeDataString(nombreArchivo)}";

    public static string Generar(TipoComando tipo, string url, string nombreArchivo) => tipo switch
    {
        TipoComando.Wget => $"wget -O {ComillasPosix(nombreArchivo)} {ComillasPosix(url)}",
        TipoComando.Curl => $"curl -fL -o {ComillasPosix(nombreArchivo)} {ComillasPosix(url)}",
        TipoComando.PowerShell => $"Invoke-WebRequest -Uri {ComillasPowerShell(url)} -OutFile {ComillasPowerShell(nombreArchivo)}",
        TipoComando.Certutil => $"certutil -urlcache -split -f \"{url}\" \"{nombreArchivo.Replace("\"", string.Empty)}\"",
        _ => url
    };

    private static string ComillasPosix(string valor) => "'" + valor.Replace("'", "'\\''") + "'";

    private static string ComillasPowerShell(string valor) => "'" + valor.Replace("'", "''") + "'";
}
