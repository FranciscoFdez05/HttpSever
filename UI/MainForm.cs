using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using HttpServer.Core;

namespace HttpServer.UI
{
    public partial class MainForm : Form
    {
        private const int MaximoLineasRegistro = 5000;
        private const int LineasARecortar = 1000;

        private static readonly string Version =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        private readonly Configuracion configuracion;
        private ServidorArchivos? servidor;
        private string? archivoSeleccionado;
        private bool deteniendo;
        private bool cerrando;
        private bool avisoFirewallMostrado;

        public MainForm()
        {
            InitializeComponent();

            configuracion = Configuracion.Cargar();
            Icon = Recursos.CargarIcono();
            Text = $"HttpServer {Version}";
            statusVersion.Text = $"v{Version}";
            barraEstado.Renderer = new RenderizadorBarraEstado();

            comboHerramienta.Items.AddRange(Enum.GetValues<TipoComando>().Cast<object>().ToArray());
            comboHerramienta.SelectedItem = configuracion.TipoComando;
            if (comboHerramienta.SelectedIndex < 0)
            {
                comboHerramienta.SelectedIndex = 0;
            }

            numericPuerto.Value = Math.Clamp(configuracion.Puerto, (int)numericPuerto.Minimum, (int)numericPuerto.Maximum);
            CargarDirecciones(configuracion.UltimaDireccion);

            BotonPlano_EnabledChanged(btnIniciar, EventArgs.Empty);
            BotonPlano_EnabledChanged(btnDetener, EventArgs.Empty);
            ActualizarInterfaz();
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            pictureBoxLogo.Image = Recursos.CargarLogo(pictureBoxLogo.Height);
            AgregarRegistro(NivelRegistro.Info, "Aplicación iniciada. Selecciona un archivo, elige el puerto y pulsa «Iniciar servidor».");
        }

        #region Selección de archivo

        private void btnExaminar_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialogo = new()
            {
                Title = "Selecciona el archivo que quieres compartir",
                Multiselect = false,
                CheckFileExists = true
            };

            if (!string.IsNullOrEmpty(configuracion.UltimaCarpeta) && Directory.Exists(configuracion.UltimaCarpeta))
            {
                dialogo.InitialDirectory = configuracion.UltimaCarpeta;
            }

            if (dialogo.ShowDialog(this) == DialogResult.OK)
            {
                SeleccionarArchivo(dialogo.FileName);
            }
        }

        private void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = servidor == null && ObtenerArchivoArrastrado(e) != null ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            string? ruta = ObtenerArchivoArrastrado(e);
            if (servidor == null && ruta != null)
            {
                SeleccionarArchivo(ruta);
            }
        }

        private static string? ObtenerArchivoArrastrado(DragEventArgs e) =>
            e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: 1 } rutas && File.Exists(rutas[0]) ? rutas[0] : null;

        private void SeleccionarArchivo(string ruta)
        {
            FileInfo archivo = new(ruta);
            if (!archivo.Exists)
            {
                AgregarRegistro(NivelRegistro.Error, $"El archivo no existe: {ruta}");
                return;
            }

            archivoSeleccionado = archivo.FullName;
            textBoxArchivo.Text = archivo.FullName;
            configuracion.UltimaCarpeta = archivo.DirectoryName;

            AgregarRegistro(NivelRegistro.Info, $"Archivo seleccionado: {archivo.Name} ({UtilidadesRed.FormatearTamano(archivo.Length)})");
            ActualizarComando();
            ActualizarInterfaz();
        }

        #endregion

        #region Servidor

        private void btnIniciar_Click(object? sender, EventArgs e)
        {
            if (servidor != null || deteniendo)
            {
                return;
            }

            if (archivoSeleccionado == null || !File.Exists(archivoSeleccionado))
            {
                AgregarRegistro(NivelRegistro.Error, "Selecciona un archivo válido antes de iniciar el servidor.");
                return;
            }

            int puerto = (int)numericPuerto.Value;
            ServidorArchivos nuevo = new(archivoSeleccionado, puerto);
            nuevo.Registro += Servidor_Registro;

            try
            {
                nuevo.Iniciar();
            }
            catch (Exception ex) when (ex is SocketException or IOException)
            {
                nuevo.Registro -= Servidor_Registro;
                AgregarRegistro(NivelRegistro.Error, DescribirErrorInicio(ex, puerto));
                return;
            }

            servidor = nuevo;
            timerEstadisticas.Start();
            ActualizarEstadisticas();
            ActualizarComando();
            ActualizarInterfaz();

            if (!avisoFirewallMostrado)
            {
                avisoFirewallMostrado = true;
                AgregarRegistro(NivelRegistro.Info, "Si otro equipo no puede conectar, permite HttpServer en el Firewall de Windows (redes privadas).");
            }
        }

        private static string DescribirErrorInicio(Exception ex, int puerto) => ex switch
        {
            SocketException { SocketErrorCode: SocketError.AddressAlreadyInUse } =>
                $"El puerto {puerto} ya está en uso por otra aplicación. Elige otro puerto.",
            SocketException { SocketErrorCode: SocketError.AccessDenied } =>
                $"No hay permiso para usar el puerto {puerto} (puede estar reservado por Windows). Elige otro puerto.",
            FileNotFoundException =>
                "El archivo seleccionado ya no existe.",
            _ => $"No se pudo iniciar el servidor: {ex.Message}"
        };

        private async void btnDetener_Click(object? sender, EventArgs e)
        {
            await DetenerServidorAsync();
        }

        private async Task DetenerServidorAsync()
        {
            if (servidor == null || deteniendo)
            {
                return;
            }

            deteniendo = true;
            ActualizarInterfaz();

            try
            {
                await servidor.DetenerAsync();
            }
            finally
            {
                servidor.Registro -= Servidor_Registro;
                timerEstadisticas.Stop();
                ActualizarEstadisticas();
                servidor = null;
                deteniendo = false;
                ActualizarComando();
                ActualizarInterfaz();
            }
        }

        private void Servidor_Registro(object? sender, RegistroEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke(() => AgregarRegistro(e.Nivel, e.Mensaje, e.Fecha));
            }
            catch (InvalidOperationException)
            {
                // El formulario se está cerrando.
            }
        }

        private void timerEstadisticas_Tick(object? sender, EventArgs e) => ActualizarEstadisticas();

        private void ActualizarEstadisticas()
        {
            if (servidor == null)
            {
                return;
            }

            statusDescargas.Text = $"Descargas: {servidor.DescargasCompletadas}";
            statusEnviado.Text = $"Enviado: {UtilidadesRed.FormatearTamano(servidor.BytesEnviados)}";
            statusConexiones.Text = $"Conexiones activas: {(servidor.Activo ? servidor.ConexionesActivas : 0)}";
        }

        #endregion

        #region Comando de descarga

        private void Opciones_Changed(object? sender, EventArgs e) => ActualizarComando();

        private void comboHerramienta_Format(object? sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is TipoComando tipo)
            {
                e.Value = GeneradorComandos.Descripcion(tipo);
            }
        }

        private void comboDireccion_DropDown(object? sender, EventArgs e)
        {
            // Las interfaces pueden cambiar (VPN, Wi-Fi...): se refresca la lista al desplegarla.
            CargarDirecciones((comboDireccion.SelectedItem as DireccionLocal)?.Direccion.ToString());
        }

        private void CargarDirecciones(string? preferida)
        {
            IReadOnlyList<DireccionLocal> direcciones = UtilidadesRed.ObtenerDireccionesLocales();

            comboDireccion.BeginUpdate();
            comboDireccion.Items.Clear();
            comboDireccion.Items.AddRange(direcciones.Cast<object>().ToArray());
            int indice = direcciones.ToList().FindIndex(d => d.Direccion.ToString() == preferida);
            comboDireccion.SelectedIndex = indice >= 0 ? indice : 0;
            comboDireccion.EndUpdate();
        }

        private string HostSeleccionado => (comboDireccion.SelectedItem as DireccionLocal)?.Direccion.ToString() ?? "127.0.0.1";

        private int PuertoActual => servidor?.Puerto ?? (int)numericPuerto.Value;

        private void ActualizarComando()
        {
            if (archivoSeleccionado == null || comboHerramienta.SelectedItem is not TipoComando tipo)
            {
                textBoxComando.Text = string.Empty;
            }
            else
            {
                string nombre = Path.GetFileName(archivoSeleccionado);
                string url = GeneradorComandos.ConstruirUrl(HostSeleccionado, PuertoActual, nombre);
                textBoxComando.Text = GeneradorComandos.Generar(tipo, url, nombre);
            }

            btnCopiar.Enabled = textBoxComando.TextLength > 0;
        }

        private async void btnCopiar_Click(object? sender, EventArgs e)
        {
            if (textBoxComando.TextLength == 0)
            {
                return;
            }

            try
            {
                Clipboard.SetText(textBoxComando.Text);
            }
            catch (ExternalException ex)
            {
                AgregarRegistro(NivelRegistro.Error, $"No se pudo copiar al portapapeles: {ex.Message}");
                return;
            }

            btnCopiar.Text = "¡Copiado!";
            await Task.Delay(1500);
            if (!IsDisposed)
            {
                btnCopiar.Text = "Copiar";
            }
        }

        private void btnAbrir_Click(object? sender, EventArgs e)
        {
            string url = $"http://{HostSeleccionado}:{PuertoActual}/";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                AgregarRegistro(NivelRegistro.Error, $"No se pudo abrir el navegador: {ex.Message}");
            }
        }

        #endregion

        #region Registro

        private void AgregarRegistro(NivelRegistro nivel, string mensaje, DateTime? fecha = null)
        {
            RichTextBox rtb = richTextBoxRegistro;
            RecortarRegistroSiEsNecesario(rtb);

            (string etiqueta, Color color) = nivel switch
            {
                NivelRegistro.Peticion => ("HTTP ", Tema.RegistroPeticion),
                NivelRegistro.Exito => ("OK   ", Tema.RegistroExito),
                NivelRegistro.Advertencia => ("WARN ", Tema.RegistroAdvertencia),
                NivelRegistro.Error => ("ERROR", Tema.RegistroError),
                _ => ("INFO ", Tema.RegistroInfo)
            };

            // Solo la etiqueta va en color; el mensaje se tiñe únicamente en avisos y errores.
            Color colorMensaje = nivel is NivelRegistro.Advertencia or NivelRegistro.Error ? color : Tema.RegistroTexto;

            AnexarTexto(rtb, $"{fecha ?? DateTime.Now:HH:mm:ss}  ", Tema.RegistroFecha);
            AnexarTexto(rtb, $"{etiqueta}  ", color);
            AnexarTexto(rtb, mensaje + Environment.NewLine, colorMensaje);
            rtb.ScrollToCaret();
        }

        private static void AnexarTexto(RichTextBox rtb, string texto, Color color)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = color;
            rtb.AppendText(texto);
        }

        private static void RecortarRegistroSiEsNecesario(RichTextBox rtb)
        {
            int lineas = rtb.GetLineFromCharIndex(rtb.TextLength);
            if (lineas < MaximoLineasRegistro)
            {
                return;
            }

            int corte = rtb.GetFirstCharIndexFromLine(LineasARecortar);
            rtb.ReadOnly = false;
            rtb.Select(0, corte);
            rtb.SelectedText = string.Empty;
            rtb.ReadOnly = true;
        }

        private void menuCopiarRegistro_Click(object? sender, EventArgs e)
        {
            string texto = richTextBoxRegistro.SelectionLength > 0 ? richTextBoxRegistro.SelectedText : richTextBoxRegistro.Text;
            if (texto.Length == 0)
            {
                return;
            }

            try
            {
                Clipboard.SetText(texto);
            }
            catch (ExternalException ex)
            {
                AgregarRegistro(NivelRegistro.Error, $"No se pudo copiar al portapapeles: {ex.Message}");
            }
        }

        private void menuGuardarRegistro_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog dialogo = new()
            {
                Title = "Guardar registro",
                Filter = "Archivo de registro (*.log)|*.log|Texto (*.txt)|*.txt",
                FileName = $"HttpServer_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (dialogo.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                File.WriteAllText(dialogo.FileName, richTextBoxRegistro.Text);
                AgregarRegistro(NivelRegistro.Info, $"Registro guardado en {dialogo.FileName}");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AgregarRegistro(NivelRegistro.Error, $"No se pudo guardar el registro: {ex.Message}");
            }
        }

        private void menuLimpiarRegistro_Click(object? sender, EventArgs e) => richTextBoxRegistro.Clear();

        #endregion

        #region Estado de la interfaz

        private void ActualizarInterfaz()
        {
            bool activo = servidor != null;

            btnIniciar.Enabled = !activo && !deteniendo && archivoSeleccionado != null;
            btnDetener.Enabled = activo && !deteniendo;
            btnExaminar.Enabled = !activo;
            numericPuerto.Enabled = !activo;
            AllowDrop = !activo;
            btnAbrir.Enabled = activo && !deteniendo;
            btnCopiar.Enabled = textBoxComando.TextLength > 0;

            (labelEstado.Text, labelEstado.ForeColor) = (activo, deteniendo) switch
            {
                (_, true) => ("● Deteniendo…", Tema.EstadoTransicion),
                (true, _) => ($"● Activo · puerto {servidor!.Puerto}", Tema.EstadoActivo),
                _ => ("● Detenido", Tema.EstadoDetenido)
            };
        }

        private void panelCabecera_Paint(object? sender, PaintEventArgs e)
        {
            // Línea de acento inferior con el degradado del logo.
            int alto = LogicalToDeviceUnits(3);
            Rectangle linea = new(0, panelCabecera.Height - alto, panelCabecera.Width, alto);
            if (linea.Width <= 0)
            {
                return;
            }

            using System.Drawing.Drawing2D.LinearGradientBrush degradado = new(linea, Tema.Acento, Tema.AcentoSecundario, 0F);
            e.Graphics.FillRectangle(degradado, linea);
        }

        private void BotonPlano_EnabledChanged(object? sender, EventArgs e)
        {
            if (sender is not Button boton)
            {
                return;
            }

            Color colorActivo = boton == btnIniciar ? Tema.Acento : Tema.Peligro;
            boton.BackColor = boton.Enabled ? colorActivo : Tema.BotonDeshabilitado;
            boton.Cursor = boton.Enabled ? Cursors.Hand : Cursors.Default;
        }

        /// <summary>
        /// Barra de estado plana: fondo blanco con una línea superior sutil.
        /// </summary>
        private sealed class RenderizadorBarraEstado : ToolStripProfessionalRenderer
        {
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) =>
                e.Graphics.Clear(e.ToolStrip.BackColor);

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using Pen borde = new(Tema.Borde);
                e.Graphics.DrawLine(borde, 0, 0, e.ToolStrip.Width, 0);
            }
        }

        #endregion

        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            configuracion.Puerto = (int)numericPuerto.Value;
            configuracion.TipoComando = comboHerramienta.SelectedItem is TipoComando tipo ? tipo : TipoComando.Wget;
            configuracion.UltimaDireccion = (comboDireccion.SelectedItem as DireccionLocal)?.Direccion.ToString();
            configuracion.Guardar();

            if (servidor == null || cerrando)
            {
                return;
            }

            // Se cancela el cierre, se detiene el servidor de forma ordenada y se vuelve a cerrar.
            e.Cancel = true;
            cerrando = true;
            Enabled = false;
            await DetenerServidorAsync();
            Close();
        }
    }
}
