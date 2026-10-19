using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;

namespace HttpServer.UI;

/// <summary>
/// Panel con fondo blanco, borde redondeado sutil y título. Sustituye a <see cref="GroupBox"/>
/// para agrupar secciones con un aspecto más limpio.
/// </summary>
[DesignerCategory("Code")]
internal sealed class Tarjeta : Panel
{
    private const int Radio = 8;
    private Font? fuenteTitulo;

    public Tarjeta()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        BackColor = Tema.Superficie;
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            Invalidate();
        }
    }

    private Font FuenteTitulo => fuenteTitulo ??= new Font("Segoe UI Semibold", Font.SizeInPoints + 0.75F, FontStyle.Bold);

    protected override void OnFontChanged(EventArgs e)
    {
        fuenteTitulo?.Dispose();
        fuenteTitulo = null;
        base.OnFontChanged(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Las esquinas fuera del redondeado toman el color del contenedor.
        e.Graphics.Clear(Parent?.BackColor ?? Tema.Fondo);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle area = new(0, 0, Width - 1, Height - 1);
        using GraphicsPath forma = Tema.RectanguloRedondeado(area, LogicalToDeviceUnits(Radio));
        using SolidBrush fondo = new(BackColor);
        using Pen borde = new(Tema.Borde);
        e.Graphics.FillPath(fondo, forma);
        e.Graphics.DrawPath(borde, forma);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        TextRenderer.DrawText(e.Graphics, Text, FuenteTitulo,
            new Point(LogicalToDeviceUnits(14), LogicalToDeviceUnits(11)),
            Tema.TextoPrincipal, TextFormatFlags.NoPrefix);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            fuenteTitulo?.Dispose();
        }

        base.Dispose(disposing);
    }
}
