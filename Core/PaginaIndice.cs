using System.Net;

namespace HttpServer.Core;

/// <summary>
/// Página HTML que se muestra al abrir la raíz del servidor desde un navegador.
/// </summary>
internal static class PaginaIndice
{
    private const string Logo = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 256"><defs><linearGradient id="f" x1="8" y1="8" x2="248" y2="248" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#2563EB"/><stop offset="1" stop-color="#06B6D4"/></linearGradient></defs><rect x="8" y="8" width="240" height="240" rx="56" fill="url(#f)"/><path d="M110 56h36v68h34l-52 54-52-54h34z" fill="#fff" stroke="#fff" stroke-width="8" stroke-linejoin="round"/><path d="M64 160v40h128v-40" fill="none" stroke="#fff" stroke-width="22" stroke-linecap="round" stroke-linejoin="round"/></svg>""";

    private const string Plantilla = """
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{{NOMBRE}} · HttpServer</title>
<link rel="icon" href="data:image/svg+xml,{{FAVICON}}">
<style>
  :root { --bg:#f1f5f9; --card:#fff; --text:#0f172a; --muted:#64748b; --accent:#2563eb; --accent-hover:#1d4ed8; --border:#e2e8f0; }
  @media (prefers-color-scheme: dark) {
    :root { --bg:#0b1120; --card:#111827; --text:#e5e7eb; --muted:#94a3b8; --accent:#3b82f6; --accent-hover:#60a5fa; --border:#1f2937; }
  }
  * { box-sizing:border-box; }
  body { margin:0; min-height:100vh; display:grid; place-items:center; padding:16px;
         font-family:"Segoe UI",system-ui,-apple-system,sans-serif; background:var(--bg); color:var(--text); }
  main { width:100%; max-width:480px; background:var(--card); border:1px solid var(--border);
         border-radius:16px; padding:32px; text-align:center; box-shadow:0 10px 30px rgba(0,0,0,.08); }
  .logo { width:72px; height:72px; }
  h1 { font-size:1.15rem; margin:16px 0 4px; word-break:break-all; }
  p { margin:0; color:var(--muted); font-size:.9rem; }
  a.boton { display:inline-block; margin-top:24px; padding:12px 28px; border-radius:10px;
            background:var(--accent); color:#fff; text-decoration:none; font-weight:600; }
  a.boton:hover { background:var(--accent-hover); }
  footer { margin-top:24px; font-size:.75rem; color:var(--muted); }
</style>
</head>
<body>
<main>
  <div class="logo">{{LOGO}}</div>
  <h1>{{NOMBRE}}</h1>
  <p>{{TAMANO}} · modificado el {{FECHA}}</p>
  <a class="boton" href="{{URL}}" download>Descargar archivo</a>
  <footer>Servido por HttpServer</footer>
</main>
</body>
</html>
""";

    public static string Generar(FileInfo archivo) => Plantilla
        .Replace("{{LOGO}}", Logo)
        .Replace("{{FAVICON}}", Uri.EscapeDataString(Logo))
        .Replace("{{NOMBRE}}", WebUtility.HtmlEncode(archivo.Name))
        .Replace("{{URL}}", "/" + Uri.EscapeDataString(archivo.Name))
        .Replace("{{TAMANO}}", UtilidadesRed.FormatearTamano(archivo.Length))
        .Replace("{{FECHA}}", archivo.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
}
