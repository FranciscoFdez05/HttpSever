namespace HttpServer.UI;

/// <summary>
/// Acceso a los recursos gráficos incrustados en el ensamblado.
/// </summary>
internal static class Recursos
{
    private const string NombreIcono = "HttpServer.logo.ico";

    /// <summary>
    /// Carga el icono de la aplicación. Si se indica <paramref name="lado"/>, se elige la imagen del
    /// .ico más cercana a ese tamaño.
    /// </summary>
    public static Icon? CargarIcono(int lado = 0)
    {
        using Stream? flujo = typeof(Recursos).Assembly.GetManifestResourceStream(NombreIcono);
        if (flujo == null)
        {
            return null;
        }

        return lado > 0 ? new Icon(flujo, lado, lado) : new Icon(flujo);
    }

    public static Bitmap? CargarLogo(int lado)
    {
        using Icon? icono = CargarIcono(lado);
        return icono?.ToBitmap();
    }
}
