# Changelog

Todos los cambios relevantes de HttpServer se documentan en este archivo.

## [2.0.0] - 2026-10-19

### Añadido
- Logo e icono de la aplicación (ejecutable, ventana y página web).
- Nueva interfaz redimensionable con cabecera, tarjetas por sección, indicador de estado y barra de estadísticas (descargas, datos enviados y conexiones activas).
- Arrastrar y soltar un archivo sobre la ventana para seleccionarlo.
- Selector de la IP local (Ethernet, Wi-Fi, VPN…) para generar el comando.
- Comandos de descarga para `wget`, `curl`, PowerShell, `certutil` o solo la URL.
- Botón «Abrir» para ver la página de descarga en el navegador.
- Página web de descarga en `http://IP:puerto/` con modo oscuro.
- Descargas reanudables mediante cabeceras `Range` (`206 Partial Content`).
- Nombre de archivo original en la descarga (`Content-Disposition`), también con espacios y acentos.
- Registro con hora, nivel, velocidad de transferencia y descargas interrumpidas; se puede copiar, guardar y limpiar.
- Preferencias guardadas entre ejecuciones (puerto, IP, herramienta y última carpeta).
- Compilación automática con GitHub Actions y publicación del ejecutable en las releases.

### Cambiado
- El servidor HTTP se separa de la interfaz (`Core/`) y la ventana pasa a `UI/MainForm`.
- El ejecutable se llama ahora `HttpServer.exe` y se publica como un único archivo.
- Mensajes de error claros cuando el puerto está ocupado o reservado por Windows.

### Corregido
- La detección de IP privada aceptaba cualquier dirección `172.*`; ahora solo el rango `172.16.0.0/12`.
- Las peticiones mal formadas o que no llegan a tiempo se rechazan en lugar de dejar la conexión abierta.
- Al detener el servidor o cerrar la ventana se cancelan correctamente las descargas en curso.

## [1.0] - 2026-03-18

- Primera versión.
