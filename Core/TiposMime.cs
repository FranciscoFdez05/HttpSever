namespace HttpServer.Core;

/// <summary>
/// Resolución del tipo MIME a partir de la extensión de un archivo.
/// </summary>
public static class TiposMime
{
    private const string Predeterminado = "application/octet-stream";

    private static readonly Dictionary<string, string> Tipos = new(StringComparer.OrdinalIgnoreCase)
    {
        // Texto
        [".txt"] = "text/plain; charset=utf-8",
        [".log"] = "text/plain; charset=utf-8",
        [".md"] = "text/markdown; charset=utf-8",
        [".csv"] = "text/csv; charset=utf-8",
        [".html"] = "text/html; charset=utf-8",
        [".htm"] = "text/html; charset=utf-8",
        [".css"] = "text/css; charset=utf-8",
        [".js"] = "text/javascript; charset=utf-8",
        [".json"] = "application/json; charset=utf-8",
        [".xml"] = "application/xml; charset=utf-8",
        [".yaml"] = "application/yaml; charset=utf-8",
        [".yml"] = "application/yaml; charset=utf-8",

        // Documentos
        [".pdf"] = "application/pdf",
        [".doc"] = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xls"] = "application/vnd.ms-excel",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        [".ppt"] = "application/vnd.ms-powerpoint",
        [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        [".odt"] = "application/vnd.oasis.opendocument.text",

        // Imágenes
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".bmp"] = "image/bmp",
        [".webp"] = "image/webp",
        [".svg"] = "image/svg+xml",
        [".ico"] = "image/x-icon",

        // Audio y vídeo
        [".mp3"] = "audio/mpeg",
        [".wav"] = "audio/wav",
        [".ogg"] = "audio/ogg",
        [".flac"] = "audio/flac",
        [".mp4"] = "video/mp4",
        [".mkv"] = "video/x-matroska",
        [".webm"] = "video/webm",
        [".avi"] = "video/x-msvideo",
        [".mov"] = "video/quicktime",

        // Archivos comprimidos y binarios
        [".zip"] = "application/zip",
        [".7z"] = "application/x-7z-compressed",
        [".rar"] = "application/vnd.rar",
        [".tar"] = "application/x-tar",
        [".gz"] = "application/gzip",
        [".iso"] = "application/x-iso9660-image",
        [".exe"] = "application/vnd.microsoft.portable-executable",
        [".msi"] = "application/x-msi",
        [".apk"] = "application/vnd.android.package-archive",
    };

    public static string Obtener(string rutaOExtension)
    {
        string extension = Path.GetExtension(rutaOExtension);
        if (string.IsNullOrEmpty(extension))
        {
            extension = rutaOExtension;
        }

        return Tipos.TryGetValue(extension, out string? tipo) ? tipo : Predeterminado;
    }
}
