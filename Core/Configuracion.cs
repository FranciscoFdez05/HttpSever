using System.Text.Json;

namespace HttpServer.Core;

/// <summary>
/// Preferencias del usuario que se conservan entre ejecuciones
/// (%AppData%\HttpServer\configuracion.json).
/// </summary>
public sealed class Configuracion
{
    private static readonly JsonSerializerOptions OpcionesJson = new() { WriteIndented = true };

    private static readonly string Ruta = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HttpServer", "configuracion.json");

    public int Puerto { get; set; } = 8080;

    public TipoComando TipoComando { get; set; } = TipoComando.Wget;

    public string? UltimaDireccion { get; set; }

    public string? UltimaCarpeta { get; set; }

    public static Configuracion Cargar()
    {
        try
        {
            if (File.Exists(Ruta))
            {
                return JsonSerializer.Deserialize<Configuracion>(File.ReadAllText(Ruta), OpcionesJson) ?? new Configuracion();
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // Configuración ilegible: se usan los valores por defecto.
        }

        return new Configuracion();
    }

    public void Guardar()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);
            File.WriteAllText(Ruta, JsonSerializer.Serialize(this, OpcionesJson));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // No poder guardar preferencias no debe impedir cerrar la aplicación.
        }
    }
}
