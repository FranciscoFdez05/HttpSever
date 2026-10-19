namespace HttpServer.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelCabecera = new Panel();
            labelEstado = new Label();
            labelSubtitulo = new Label();
            labelTitulo = new Label();
            pictureBoxLogo = new PictureBox();
            tablaPrincipal = new TableLayoutPanel();
            grupoConfiguracion = new Tarjeta();
            tablaConfiguracion = new TableLayoutPanel();
            labelArchivo = new Label();
            textBoxArchivo = new TextBox();
            btnExaminar = new Button();
            labelPuerto = new Label();
            numericPuerto = new NumericUpDown();
            panelBotonesServidor = new FlowLayoutPanel();
            btnIniciar = new Button();
            btnDetener = new Button();
            grupoComando = new Tarjeta();
            tablaComando = new TableLayoutPanel();
            labelDireccion = new Label();
            comboDireccion = new ComboBox();
            labelHerramienta = new Label();
            comboHerramienta = new ComboBox();
            textBoxComando = new TextBox();
            btnCopiar = new Button();
            btnAbrir = new Button();
            grupoRegistro = new Tarjeta();
            panelRegistro = new Panel();
            richTextBoxRegistro = new RichTextBox();
            menuRegistro = new ContextMenuStrip(components);
            menuCopiarRegistro = new ToolStripMenuItem();
            menuGuardarRegistro = new ToolStripMenuItem();
            menuSeparadorRegistro = new ToolStripSeparator();
            menuLimpiarRegistro = new ToolStripMenuItem();
            barraEstado = new StatusStrip();
            statusDescargas = new ToolStripStatusLabel();
            statusEnviado = new ToolStripStatusLabel();
            statusConexiones = new ToolStripStatusLabel();
            statusVersion = new ToolStripStatusLabel();
            timerEstadisticas = new System.Windows.Forms.Timer(components);
            toolTip = new ToolTip(components);
            panelCabecera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            tablaPrincipal.SuspendLayout();
            grupoConfiguracion.SuspendLayout();
            tablaConfiguracion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericPuerto).BeginInit();
            panelBotonesServidor.SuspendLayout();
            grupoComando.SuspendLayout();
            tablaComando.SuspendLayout();
            grupoRegistro.SuspendLayout();
            panelRegistro.SuspendLayout();
            menuRegistro.SuspendLayout();
            barraEstado.SuspendLayout();
            SuspendLayout();
            //
            // panelCabecera
            //
            panelCabecera.BackColor = Color.FromArgb(15, 23, 42);
            panelCabecera.Controls.Add(labelEstado);
            panelCabecera.Controls.Add(labelSubtitulo);
            panelCabecera.Controls.Add(labelTitulo);
            panelCabecera.Controls.Add(pictureBoxLogo);
            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Location = new Point(0, 0);
            panelCabecera.Name = "panelCabecera";
            panelCabecera.Size = new Size(684, 76);
            panelCabecera.TabIndex = 0;
            panelCabecera.Paint += panelCabecera_Paint;
            //
            // labelEstado
            //
            labelEstado.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelEstado.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            labelEstado.ForeColor = Color.FromArgb(148, 163, 184);
            labelEstado.Location = new Point(424, 26);
            labelEstado.Name = "labelEstado";
            labelEstado.Size = new Size(244, 24);
            labelEstado.TabIndex = 3;
            labelEstado.Text = "● Detenido";
            labelEstado.TextAlign = ContentAlignment.MiddleRight;
            //
            // labelSubtitulo
            //
            labelSubtitulo.AutoSize = true;
            labelSubtitulo.ForeColor = Color.FromArgb(148, 163, 184);
            labelSubtitulo.Location = new Point(77, 44);
            labelSubtitulo.Name = "labelSubtitulo";
            labelSubtitulo.Size = new Size(229, 17);
            labelSubtitulo.TabIndex = 2;
            labelSubtitulo.Text = "Comparte archivos en tu red local";
            //
            // labelTitulo
            //
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            labelTitulo.ForeColor = Color.White;
            labelTitulo.Location = new Point(74, 12);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(122, 30);
            labelTitulo.TabIndex = 1;
            labelTitulo.Text = "HttpServer";
            //
            // pictureBoxLogo
            //
            pictureBoxLogo.Location = new Point(16, 14);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(48, 48);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            //
            // tablaPrincipal
            //
            tablaPrincipal.ColumnCount = 1;
            tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablaPrincipal.Controls.Add(grupoConfiguracion, 0, 0);
            tablaPrincipal.Controls.Add(grupoComando, 0, 1);
            tablaPrincipal.Controls.Add(grupoRegistro, 0, 2);
            tablaPrincipal.Dock = DockStyle.Fill;
            tablaPrincipal.Location = new Point(0, 76);
            tablaPrincipal.Name = "tablaPrincipal";
            tablaPrincipal.Padding = new Padding(14);
            tablaPrincipal.RowCount = 3;
            tablaPrincipal.RowStyles.Add(new RowStyle());
            tablaPrincipal.RowStyles.Add(new RowStyle());
            tablaPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablaPrincipal.Size = new Size(684, 543);
            tablaPrincipal.TabIndex = 1;
            //
            // grupoConfiguracion
            //
            grupoConfiguracion.AutoSize = true;
            grupoConfiguracion.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            grupoConfiguracion.Controls.Add(tablaConfiguracion);
            grupoConfiguracion.Dock = DockStyle.Fill;
            grupoConfiguracion.Margin = new Padding(0, 0, 0, 12);
            grupoConfiguracion.Location = new Point(12, 12);
            grupoConfiguracion.Name = "grupoConfiguracion";
            grupoConfiguracion.Padding = new Padding(14, 40, 14, 14);
            grupoConfiguracion.Size = new Size(660, 145);
            grupoConfiguracion.TabIndex = 0;
            grupoConfiguracion.Text = "Servidor";
            //
            // tablaConfiguracion
            //
            tablaConfiguracion.AutoSize = true;
            tablaConfiguracion.ColumnCount = 3;
            tablaConfiguracion.ColumnStyles.Add(new ColumnStyle());
            tablaConfiguracion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablaConfiguracion.ColumnStyles.Add(new ColumnStyle());
            tablaConfiguracion.Controls.Add(labelArchivo, 0, 0);
            tablaConfiguracion.Controls.Add(textBoxArchivo, 1, 0);
            tablaConfiguracion.Controls.Add(btnExaminar, 2, 0);
            tablaConfiguracion.Controls.Add(labelPuerto, 0, 1);
            tablaConfiguracion.Controls.Add(numericPuerto, 1, 1);
            tablaConfiguracion.Controls.Add(panelBotonesServidor, 0, 2);
            tablaConfiguracion.Dock = DockStyle.Fill;
            tablaConfiguracion.Location = new Point(9, 24);
            tablaConfiguracion.Name = "tablaConfiguracion";
            tablaConfiguracion.RowCount = 3;
            tablaConfiguracion.RowStyles.Add(new RowStyle());
            tablaConfiguracion.RowStyles.Add(new RowStyle());
            tablaConfiguracion.RowStyles.Add(new RowStyle());
            tablaConfiguracion.Size = new Size(642, 112);
            tablaConfiguracion.TabIndex = 0;
            //
            // labelArchivo
            //
            labelArchivo.Anchor = AnchorStyles.Left;
            labelArchivo.AutoSize = true;
            labelArchivo.ForeColor = Color.FromArgb(71, 85, 105);
            labelArchivo.Location = new Point(3, 9);
            labelArchivo.Name = "labelArchivo";
            labelArchivo.Size = new Size(56, 17);
            labelArchivo.TabIndex = 0;
            labelArchivo.Text = "Archivo";
            //
            // textBoxArchivo
            //
            textBoxArchivo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxArchivo.BackColor = Color.FromArgb(248, 250, 252);
            textBoxArchivo.ForeColor = Color.FromArgb(30, 41, 59);
            textBoxArchivo.Location = new Point(65, 5);
            textBoxArchivo.Name = "textBoxArchivo";
            textBoxArchivo.PlaceholderText = "Selecciona un archivo o arrástralo a la ventana";
            textBoxArchivo.ReadOnly = true;
            textBoxArchivo.Size = new Size(458, 25);
            textBoxArchivo.TabIndex = 1;
            //
            // btnExaminar
            //
            btnExaminar.AutoSize = true;
            btnExaminar.BackColor = Color.White;
            btnExaminar.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnExaminar.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 232, 240);
            btnExaminar.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnExaminar.FlatStyle = FlatStyle.Flat;
            btnExaminar.ForeColor = Color.FromArgb(30, 41, 59);
            btnExaminar.Location = new Point(529, 3);
            btnExaminar.MinimumSize = new Size(110, 30);
            btnExaminar.Name = "btnExaminar";
            btnExaminar.Size = new Size(110, 30);
            btnExaminar.TabIndex = 2;
            btnExaminar.Text = "Examinar…";
            toolTip.SetToolTip(btnExaminar, "Elegir el archivo que se va a compartir");
            btnExaminar.UseVisualStyleBackColor = false;
            btnExaminar.Click += btnExaminar_Click;
            //
            // labelPuerto
            //
            labelPuerto.Anchor = AnchorStyles.Left;
            labelPuerto.AutoSize = true;
            labelPuerto.ForeColor = Color.FromArgb(71, 85, 105);
            labelPuerto.Location = new Point(3, 42);
            labelPuerto.Name = "labelPuerto";
            labelPuerto.Size = new Size(47, 17);
            labelPuerto.TabIndex = 3;
            labelPuerto.Text = "Puerto";
            //
            // numericPuerto
            //
            numericPuerto.Anchor = AnchorStyles.Left;
            numericPuerto.Location = new Point(65, 39);
            numericPuerto.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericPuerto.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericPuerto.Name = "numericPuerto";
            numericPuerto.Size = new Size(100, 25);
            numericPuerto.TabIndex = 4;
            numericPuerto.TextAlign = HorizontalAlignment.Right;
            toolTip.SetToolTip(numericPuerto, "Puerto TCP en el que escuchará el servidor (1-65535)");
            numericPuerto.Value = new decimal(new int[] { 8080, 0, 0, 0 });
            numericPuerto.ValueChanged += Opciones_Changed;
            //
            // panelBotonesServidor
            //
            panelBotonesServidor.AutoSize = true;
            tablaConfiguracion.SetColumnSpan(panelBotonesServidor, 3);
            panelBotonesServidor.Controls.Add(btnIniciar);
            panelBotonesServidor.Controls.Add(btnDetener);
            panelBotonesServidor.Location = new Point(0, 70);
            panelBotonesServidor.Margin = new Padding(0, 3, 0, 0);
            panelBotonesServidor.Name = "panelBotonesServidor";
            panelBotonesServidor.Size = new Size(344, 42);
            panelBotonesServidor.TabIndex = 5;
            //
            // btnIniciar
            //
            btnIniciar.BackColor = Color.FromArgb(37, 99, 235);
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
            btnIniciar.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            btnIniciar.FlatStyle = FlatStyle.Flat;
            btnIniciar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnIniciar.ForeColor = Color.White;
            btnIniciar.Location = new Point(3, 3);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(166, 36);
            btnIniciar.TabIndex = 0;
            btnIniciar.Text = "▶  Iniciar servidor";
            toolTip.SetToolTip(btnIniciar, "Publicar el archivo en el puerto indicado");
            btnIniciar.UseVisualStyleBackColor = false;
            btnIniciar.EnabledChanged += BotonPlano_EnabledChanged;
            btnIniciar.Click += btnIniciar_Click;
            //
            // btnDetener
            //
            btnDetener.BackColor = Color.FromArgb(220, 38, 38);
            btnDetener.FlatAppearance.BorderSize = 0;
            btnDetener.FlatAppearance.MouseDownBackColor = Color.FromArgb(153, 27, 27);
            btnDetener.FlatAppearance.MouseOverBackColor = Color.FromArgb(185, 28, 28);
            btnDetener.FlatStyle = FlatStyle.Flat;
            btnDetener.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnDetener.ForeColor = Color.White;
            btnDetener.Location = new Point(175, 3);
            btnDetener.Name = "btnDetener";
            btnDetener.Size = new Size(166, 36);
            btnDetener.TabIndex = 1;
            btnDetener.Text = "■  Detener servidor";
            toolTip.SetToolTip(btnDetener, "Dejar de publicar el archivo y cerrar las conexiones");
            btnDetener.UseVisualStyleBackColor = false;
            btnDetener.EnabledChanged += BotonPlano_EnabledChanged;
            btnDetener.Click += btnDetener_Click;
            //
            // grupoComando
            //
            grupoComando.AutoSize = true;
            grupoComando.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            grupoComando.Controls.Add(tablaComando);
            grupoComando.Dock = DockStyle.Fill;
            grupoComando.Margin = new Padding(0, 0, 0, 12);
            grupoComando.Location = new Point(12, 163);
            grupoComando.Name = "grupoComando";
            grupoComando.Padding = new Padding(14, 40, 14, 14);
            grupoComando.Size = new Size(660, 107);
            grupoComando.TabIndex = 1;
            grupoComando.Text = "Comando de descarga";
            //
            // tablaComando
            //
            tablaComando.AutoSize = true;
            tablaComando.ColumnCount = 6;
            tablaComando.ColumnStyles.Add(new ColumnStyle());
            tablaComando.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tablaComando.ColumnStyles.Add(new ColumnStyle());
            tablaComando.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tablaComando.ColumnStyles.Add(new ColumnStyle());
            tablaComando.ColumnStyles.Add(new ColumnStyle());
            tablaComando.Controls.Add(labelDireccion, 0, 0);
            tablaComando.Controls.Add(comboDireccion, 1, 0);
            tablaComando.Controls.Add(labelHerramienta, 2, 0);
            tablaComando.Controls.Add(comboHerramienta, 3, 0);
            tablaComando.Controls.Add(textBoxComando, 0, 1);
            tablaComando.Controls.Add(btnCopiar, 4, 1);
            tablaComando.Controls.Add(btnAbrir, 5, 1);
            tablaComando.Dock = DockStyle.Fill;
            tablaComando.Location = new Point(9, 24);
            tablaComando.Name = "tablaComando";
            tablaComando.RowCount = 2;
            tablaComando.RowStyles.Add(new RowStyle());
            tablaComando.RowStyles.Add(new RowStyle());
            tablaComando.Size = new Size(642, 74);
            tablaComando.TabIndex = 0;
            //
            // labelDireccion
            //
            labelDireccion.Anchor = AnchorStyles.Left;
            labelDireccion.AutoSize = true;
            labelDireccion.ForeColor = Color.FromArgb(71, 85, 105);
            labelDireccion.Location = new Point(3, 7);
            labelDireccion.Name = "labelDireccion";
            labelDireccion.Size = new Size(63, 17);
            labelDireccion.TabIndex = 0;
            labelDireccion.Text = "Dirección";
            //
            // comboDireccion
            //
            comboDireccion.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboDireccion.DropDownWidth = 320;
            comboDireccion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDireccion.FormattingEnabled = true;
            comboDireccion.Location = new Point(72, 3);
            comboDireccion.Name = "comboDireccion";
            comboDireccion.Size = new Size(255, 25);
            comboDireccion.TabIndex = 1;
            toolTip.SetToolTip(comboDireccion, "IP de este equipo que usará el cliente para conectarse");
            comboDireccion.DropDown += comboDireccion_DropDown;
            comboDireccion.SelectedIndexChanged += Opciones_Changed;
            //
            // labelHerramienta
            //
            labelHerramienta.Anchor = AnchorStyles.Left;
            labelHerramienta.AutoSize = true;
            labelHerramienta.ForeColor = Color.FromArgb(71, 85, 105);
            labelHerramienta.Location = new Point(333, 7);
            labelHerramienta.Name = "labelHerramienta";
            labelHerramienta.Size = new Size(80, 17);
            labelHerramienta.TabIndex = 2;
            labelHerramienta.Text = "Herramienta";
            //
            // comboHerramienta
            //
            comboHerramienta.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tablaComando.SetColumnSpan(comboHerramienta, 3);
            comboHerramienta.DropDownStyle = ComboBoxStyle.DropDownList;
            comboHerramienta.FormattingEnabled = true;
            comboHerramienta.Location = new Point(419, 3);
            comboHerramienta.Name = "comboHerramienta";
            comboHerramienta.Size = new Size(220, 25);
            comboHerramienta.TabIndex = 3;
            toolTip.SetToolTip(comboHerramienta, "Formato del comando según el sistema del equipo cliente");
            comboHerramienta.Format += comboHerramienta_Format;
            comboHerramienta.SelectedIndexChanged += Opciones_Changed;
            //
            // textBoxComando
            //
            textBoxComando.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxComando.BackColor = Color.FromArgb(248, 250, 252);
            textBoxComando.ForeColor = Color.FromArgb(15, 23, 42);
            tablaComando.SetColumnSpan(textBoxComando, 4);
            textBoxComando.Font = new Font("Consolas", 10.5F);
            textBoxComando.Location = new Point(3, 40);
            textBoxComando.Name = "textBoxComando";
            textBoxComando.ReadOnly = true;
            textBoxComando.Size = new Size(458, 24);
            textBoxComando.TabIndex = 4;
            //
            // btnCopiar
            //
            btnCopiar.BackColor = Color.FromArgb(239, 246, 255);
            btnCopiar.FlatAppearance.BorderColor = Color.FromArgb(147, 197, 253);
            btnCopiar.FlatAppearance.MouseDownBackColor = Color.FromArgb(191, 219, 254);
            btnCopiar.FlatAppearance.MouseOverBackColor = Color.FromArgb(219, 234, 254);
            btnCopiar.FlatStyle = FlatStyle.Flat;
            btnCopiar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btnCopiar.ForeColor = Color.FromArgb(29, 78, 216);
            btnCopiar.Location = new Point(467, 37);
            btnCopiar.MinimumSize = new Size(84, 30);
            btnCopiar.Name = "btnCopiar";
            btnCopiar.Size = new Size(84, 30);
            btnCopiar.TabIndex = 5;
            btnCopiar.Text = "Copiar";
            toolTip.SetToolTip(btnCopiar, "Copiar el comando al portapapeles");
            btnCopiar.UseVisualStyleBackColor = false;
            btnCopiar.Click += btnCopiar_Click;
            //
            // btnAbrir
            //
            btnAbrir.BackColor = Color.White;
            btnAbrir.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnAbrir.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 232, 240);
            btnAbrir.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnAbrir.FlatStyle = FlatStyle.Flat;
            btnAbrir.ForeColor = Color.FromArgb(30, 41, 59);
            btnAbrir.Location = new Point(557, 37);
            btnAbrir.MinimumSize = new Size(82, 30);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(82, 30);
            btnAbrir.TabIndex = 6;
            btnAbrir.Text = "Abrir";
            toolTip.SetToolTip(btnAbrir, "Abrir la página de descarga en el navegador");
            btnAbrir.UseVisualStyleBackColor = false;
            btnAbrir.Click += btnAbrir_Click;
            //
            // grupoRegistro
            //
            grupoRegistro.Controls.Add(panelRegistro);
            grupoRegistro.Dock = DockStyle.Fill;
            grupoRegistro.Margin = new Padding(0);
            grupoRegistro.Location = new Point(12, 276);
            grupoRegistro.Name = "grupoRegistro";
            grupoRegistro.Padding = new Padding(14, 40, 14, 14);
            grupoRegistro.Size = new Size(660, 255);
            grupoRegistro.TabIndex = 2;
            grupoRegistro.Text = "Registro";
            //
            // panelRegistro
            //
            panelRegistro.BackColor = Color.FromArgb(15, 23, 42);
            panelRegistro.Controls.Add(richTextBoxRegistro);
            panelRegistro.Dock = DockStyle.Fill;
            panelRegistro.Location = new Point(14, 40);
            panelRegistro.Name = "panelRegistro";
            panelRegistro.Padding = new Padding(10, 8, 4, 8);
            panelRegistro.Size = new Size(628, 200);
            panelRegistro.TabIndex = 0;
            //
            // richTextBoxRegistro
            //
            richTextBoxRegistro.BackColor = Color.FromArgb(15, 23, 42);
            richTextBoxRegistro.BorderStyle = BorderStyle.None;
            richTextBoxRegistro.ContextMenuStrip = menuRegistro;
            richTextBoxRegistro.DetectUrls = false;
            richTextBoxRegistro.Dock = DockStyle.Fill;
            richTextBoxRegistro.Font = new Font("Consolas", 9.75F);
            richTextBoxRegistro.ForeColor = Color.FromArgb(203, 213, 225);
            richTextBoxRegistro.HideSelection = false;
            richTextBoxRegistro.Location = new Point(9, 24);
            richTextBoxRegistro.Name = "richTextBoxRegistro";
            richTextBoxRegistro.ReadOnly = true;
            richTextBoxRegistro.Size = new Size(642, 222);
            richTextBoxRegistro.TabIndex = 0;
            richTextBoxRegistro.Text = "";
            //
            // menuRegistro
            //
            menuRegistro.Items.AddRange(new ToolStripItem[] { menuCopiarRegistro, menuGuardarRegistro, menuSeparadorRegistro, menuLimpiarRegistro });
            menuRegistro.Name = "menuRegistro";
            menuRegistro.Size = new Size(181, 76);
            //
            // menuCopiarRegistro
            //
            menuCopiarRegistro.Name = "menuCopiarRegistro";
            menuCopiarRegistro.ShortcutKeyDisplayString = "Ctrl+C";
            menuCopiarRegistro.Size = new Size(180, 22);
            menuCopiarRegistro.Text = "Copiar";
            menuCopiarRegistro.Click += menuCopiarRegistro_Click;
            //
            // menuGuardarRegistro
            //
            menuGuardarRegistro.Name = "menuGuardarRegistro";
            menuGuardarRegistro.Size = new Size(180, 22);
            menuGuardarRegistro.Text = "Guardar registro…";
            menuGuardarRegistro.Click += menuGuardarRegistro_Click;
            //
            // menuSeparadorRegistro
            //
            menuSeparadorRegistro.Name = "menuSeparadorRegistro";
            menuSeparadorRegistro.Size = new Size(177, 6);
            //
            // menuLimpiarRegistro
            //
            menuLimpiarRegistro.Name = "menuLimpiarRegistro";
            menuLimpiarRegistro.Size = new Size(180, 22);
            menuLimpiarRegistro.Text = "Limpiar registro";
            menuLimpiarRegistro.Click += menuLimpiarRegistro_Click;
            //
            // barraEstado
            //
            barraEstado.BackColor = Color.White;
            barraEstado.ForeColor = Color.FromArgb(100, 116, 139);
            barraEstado.Padding = new Padding(8, 2, 8, 2);
            barraEstado.SizingGrip = false;
            barraEstado.Items.AddRange(new ToolStripItem[] { statusDescargas, statusEnviado, statusConexiones, statusVersion });
            barraEstado.Location = new Point(0, 619);
            barraEstado.Name = "barraEstado";
            barraEstado.Size = new Size(684, 22);
            barraEstado.TabIndex = 2;
            //
            // statusDescargas
            //
            statusDescargas.Name = "statusDescargas";
            statusDescargas.Size = new Size(80, 17);
            statusDescargas.Text = "Descargas: 0";
            //
            // statusEnviado
            //
            statusEnviado.Margin = new Padding(16, 3, 0, 2);
            statusEnviado.Name = "statusEnviado";
            statusEnviado.Size = new Size(80, 17);
            statusEnviado.Text = "Enviado: 0 B";
            //
            // statusConexiones
            //
            statusConexiones.Margin = new Padding(16, 3, 0, 2);
            statusConexiones.Name = "statusConexiones";
            statusConexiones.Size = new Size(124, 17);
            statusConexiones.Text = "Conexiones activas: 0";
            //
            // statusVersion
            //
            statusVersion.ForeColor = Color.FromArgb(148, 163, 184);
            statusVersion.Name = "statusVersion";
            statusVersion.Size = new Size(385, 17);
            statusVersion.Spring = true;
            statusVersion.Text = "v2.0.0";
            statusVersion.TextAlign = ContentAlignment.MiddleRight;
            //
            // timerEstadisticas
            //
            timerEstadisticas.Interval = 500;
            timerEstadisticas.Tick += timerEstadisticas_Tick;
            //
            // MainForm
            //
            AllowDrop = true;
            BackColor = Color.FromArgb(241, 245, 249);
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 641);
            Controls.Add(tablaPrincipal);
            Controls.Add(barraEstado);
            Controls.Add(panelCabecera);
            Font = new Font("Segoe UI", 9.75F);
            MinimumSize = new Size(600, 560);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HttpServer";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            panelCabecera.ResumeLayout(false);
            panelCabecera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            tablaPrincipal.ResumeLayout(false);
            tablaPrincipal.PerformLayout();
            grupoConfiguracion.ResumeLayout(false);
            grupoConfiguracion.PerformLayout();
            tablaConfiguracion.ResumeLayout(false);
            tablaConfiguracion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericPuerto).EndInit();
            panelBotonesServidor.ResumeLayout(false);
            grupoComando.ResumeLayout(false);
            grupoComando.PerformLayout();
            tablaComando.ResumeLayout(false);
            tablaComando.PerformLayout();
            grupoRegistro.ResumeLayout(false);
            panelRegistro.ResumeLayout(false);
            menuRegistro.ResumeLayout(false);
            barraEstado.ResumeLayout(false);
            barraEstado.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelCabecera;
        private PictureBox pictureBoxLogo;
        private Label labelTitulo;
        private Label labelSubtitulo;
        private Label labelEstado;
        private TableLayoutPanel tablaPrincipal;
        private Tarjeta grupoConfiguracion;
        private TableLayoutPanel tablaConfiguracion;
        private Label labelArchivo;
        private TextBox textBoxArchivo;
        private Button btnExaminar;
        private Label labelPuerto;
        private NumericUpDown numericPuerto;
        private FlowLayoutPanel panelBotonesServidor;
        private Button btnIniciar;
        private Button btnDetener;
        private Tarjeta grupoComando;
        private TableLayoutPanel tablaComando;
        private Label labelDireccion;
        private ComboBox comboDireccion;
        private Label labelHerramienta;
        private ComboBox comboHerramienta;
        private TextBox textBoxComando;
        private Button btnCopiar;
        private Button btnAbrir;
        private Tarjeta grupoRegistro;
        private Panel panelRegistro;
        private RichTextBox richTextBoxRegistro;
        private ContextMenuStrip menuRegistro;
        private ToolStripMenuItem menuCopiarRegistro;
        private ToolStripMenuItem menuGuardarRegistro;
        private ToolStripSeparator menuSeparadorRegistro;
        private ToolStripMenuItem menuLimpiarRegistro;
        private StatusStrip barraEstado;
        private ToolStripStatusLabel statusDescargas;
        private ToolStripStatusLabel statusEnviado;
        private ToolStripStatusLabel statusConexiones;
        private ToolStripStatusLabel statusVersion;
        private System.Windows.Forms.Timer timerEstadisticas;
        private ToolTip toolTip;
    }
}
