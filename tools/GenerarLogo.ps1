<#
.SYNOPSIS
    Genera los recursos gráficos del logo de HttpServer (Assets/logo.png y Assets/logo.ico).

.DESCRIPTION
    Dibuja el logo con GDI+ a partir de la misma geometría que Assets/logo.svg
    (lienzo de 256x256) y produce:
      - logo.png : 256x256
      - logo.ico : 16, 20, 24, 32, 40, 48, 64, 128 y 256 px (256 en PNG, el resto en DIB 32 bits)

.EXAMPLE
    pwsh ./tools/GenerarLogo.ps1
#>
[CmdletBinding()]
param(
    [string]$Destino = (Join-Path $PSScriptRoot '..\Assets')
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

function New-RoundedRectPath([float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $d = $r * 2
    $path.AddArc($x, $y, $d, $d, 180, 90)
    $path.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $path.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $path.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function New-LogoBitmap([int]$size) {
    $bmp = [System.Drawing.Bitmap]::new($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    try {
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $g.Clear([System.Drawing.Color]::Transparent)
        $s = $size / 256.0
        $g.ScaleTransform($s, $s)

        # Fondo: cuadrado redondeado con degradado azul -> cian
        $fondo = New-RoundedRectPath 8 8 240 240 56
        $degradado = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
            [System.Drawing.PointF]::new(8, 8), [System.Drawing.PointF]::new(248, 248),
            [System.Drawing.ColorTranslator]::FromHtml('#2563EB'),
            [System.Drawing.ColorTranslator]::FromHtml('#06B6D4'))
        $g.FillPath($degradado, $fondo)

        $blanco = [System.Drawing.Brushes]::White

        # Flecha de descarga
        $flecha = [System.Drawing.Drawing2D.GraphicsPath]::new()
        $flecha.AddPolygon([System.Drawing.PointF[]]@(
            [System.Drawing.PointF]::new(110, 56), [System.Drawing.PointF]::new(146, 56),
            [System.Drawing.PointF]::new(146, 124), [System.Drawing.PointF]::new(180, 124),
            [System.Drawing.PointF]::new(128, 178), [System.Drawing.PointF]::new(76, 124),
            [System.Drawing.PointF]::new(110, 124)))
        $g.FillPath($blanco, $flecha)
        $bordeFlecha = [System.Drawing.Pen]::new([System.Drawing.Color]::White, 8)
        $bordeFlecha.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
        $g.DrawPath($bordeFlecha, $flecha)

        # Bandeja
        $bandeja = [System.Drawing.Pen]::new([System.Drawing.Color]::White, 22)
        $bandeja.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
        $bandeja.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
        $bandeja.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
        $g.DrawLines($bandeja, [System.Drawing.PointF[]]@(
            [System.Drawing.PointF]::new(64, 160), [System.Drawing.PointF]::new(64, 200),
            [System.Drawing.PointF]::new(192, 200), [System.Drawing.PointF]::new(192, 160)))
    }
    finally {
        $g.Dispose()
    }
    return $bmp
}

function Get-PngBytes([System.Drawing.Bitmap]$bmp) {
    $ms = [System.IO.MemoryStream]::new()
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    return , $ms.ToArray()
}

function Get-DibBytes([System.Drawing.Bitmap]$bmp) {
    $w = $bmp.Width; $h = $bmp.Height
    $rect = [System.Drawing.Rectangle]::new(0, 0, $w, $h)
    $data = $bmp.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadOnly, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $stride = $w * 4
    $pixeles = [byte[]]::new($stride * $h)
    for ($y = 0; $y -lt $h; $y++) {
        # Los DIB se guardan de abajo hacia arriba
        $origen = [IntPtr]::Add($data.Scan0, $y * $data.Stride)
        [System.Runtime.InteropServices.Marshal]::Copy($origen, $pixeles, ($h - 1 - $y) * $stride, $stride)
    }
    $bmp.UnlockBits($data)

    $mascaraStride = [int]([Math]::Ceiling($w / 32.0) * 4)
    $mascara = [byte[]]::new($mascaraStride * $h)

    $ms = [System.IO.MemoryStream]::new()
    $bw = [System.IO.BinaryWriter]::new($ms)
    $bw.Write([int]40); $bw.Write([int]$w); $bw.Write([int]($h * 2))
    $bw.Write([int16]1); $bw.Write([int16]32); $bw.Write([int]0)
    $bw.Write([int]($pixeles.Length + $mascara.Length))
    $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0)
    $bw.Write($pixeles); $bw.Write($mascara)
    $bw.Flush()
    return , $ms.ToArray()
}

New-Item -ItemType Directory -Force $Destino | Out-Null

$logo256 = New-LogoBitmap 256
$rutaPng = Join-Path $Destino 'logo.png'
$logo256.Save($rutaPng, [System.Drawing.Imaging.ImageFormat]::Png)

$tamanos = 16, 20, 24, 32, 40, 48, 64, 128, 256
$imagenes = [System.Collections.Generic.List[byte[]]]::new()
foreach ($t in $tamanos) {
    $bmp = New-LogoBitmap $t
    if ($t -eq 256) { $imagenes.Add((Get-PngBytes $bmp)) } else { $imagenes.Add((Get-DibBytes $bmp)) }
    $bmp.Dispose()
}

$ms = [System.IO.MemoryStream]::new()
$bw = [System.IO.BinaryWriter]::new($ms)
$bw.Write([int16]0); $bw.Write([int16]1); $bw.Write([int16]$tamanos.Count)
$offset = 6 + 16 * $tamanos.Count
for ($i = 0; $i -lt $tamanos.Count; $i++) {
    $t = $tamanos[$i]
    $lado = if ($t -ge 256) { 0 } else { $t }
    $bw.Write([byte]$lado); $bw.Write([byte]$lado); $bw.Write([byte]0); $bw.Write([byte]0)
    $bw.Write([int16]1); $bw.Write([int16]32)
    $bw.Write([int]$imagenes[$i].Length); $bw.Write([int]$offset)
    $offset += $imagenes[$i].Length
}
foreach ($img in $imagenes) { $bw.Write($img) }
$bw.Flush()

$rutaIco = Join-Path $Destino 'logo.ico'
[System.IO.File]::WriteAllBytes($rutaIco, $ms.ToArray())
$logo256.Dispose()

Write-Host "Generado: $rutaPng"
Write-Host "Generado: $rutaIco"
