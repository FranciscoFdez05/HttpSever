using System.Drawing.Drawing2D;

namespace HttpServer.UI;

/// <summary>
/// Paleta de colores de la aplicación (basada en la escala "slate" con azul de acento, el del logo).
/// </summary>
internal static class Tema
{
    // Superficies
    public static readonly Color Fondo = Color.FromArgb(241, 245, 249);          // slate-100
    public static readonly Color Superficie = Color.White;
    public static readonly Color SuperficieSecundaria = Color.FromArgb(248, 250, 252); // slate-50
    public static readonly Color Borde = Color.FromArgb(226, 232, 240);          // slate-200
    public static readonly Color Oscuro = Color.FromArgb(15, 23, 42);            // slate-900

    // Texto
    public static readonly Color TextoPrincipal = Color.FromArgb(30, 41, 59);    // slate-800
    public static readonly Color TextoSecundario = Color.FromArgb(100, 116, 139); // slate-500

    // Acento (degradado del logo)
    public static readonly Color Acento = Color.FromArgb(37, 99, 235);           // blue-600
    public static readonly Color AcentoSecundario = Color.FromArgb(6, 182, 212); // cyan-500
    public static readonly Color Peligro = Color.FromArgb(220, 38, 38);          // red-600
    public static readonly Color BotonDeshabilitado = Color.FromArgb(226, 232, 240);

    // Indicador de estado (sobre la cabecera oscura)
    public static readonly Color EstadoActivo = Color.FromArgb(74, 222, 128);
    public static readonly Color EstadoDetenido = Color.FromArgb(148, 163, 184);
    public static readonly Color EstadoTransicion = Color.FromArgb(250, 204, 21);

    // Registro (sobre fondo oscuro)
    public static readonly Color RegistroTexto = Color.FromArgb(203, 213, 225);
    public static readonly Color RegistroFecha = Color.FromArgb(71, 85, 105);
    public static readonly Color RegistroInfo = Color.FromArgb(96, 165, 250);
    public static readonly Color RegistroPeticion = Color.FromArgb(167, 139, 250);
    public static readonly Color RegistroExito = Color.FromArgb(74, 222, 128);
    public static readonly Color RegistroAdvertencia = Color.FromArgb(251, 191, 36);
    public static readonly Color RegistroError = Color.FromArgb(248, 113, 113);

    public static GraphicsPath RectanguloRedondeado(Rectangle r, int radio)
    {
        GraphicsPath path = new();
        int d = radio * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
