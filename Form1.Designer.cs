namespace HttpServer
{
    partial class Form1
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
            puerto = new TextBox();
            label1 = new Label();
            btnServidorUp = new Button();
            BtnsubirArchivo = new Button();
            btnCopiar = new Button();
            textBoxComando = new TextBox();
            textBoxSubirArchivo = new TextBox();
            btnServerDown = new Button();
            richTextBoxTerminal = new RichTextBox();
            SuspendLayout();
            // 
            // puerto
            // 
            puerto.Font = new Font("Segoe UI", 12F);
            puerto.Location = new Point(88, 68);
            puerto.Name = "puerto";
            puerto.Size = new Size(68, 29);
            puerto.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(12, 65);
            label1.Name = "label1";
            label1.Size = new Size(70, 28);
            label1.TabIndex = 1;
            label1.Text = "Puerto";
            label1.Click += label1_Click;
            // 
            // btnServidorUp
            // 
            btnServidorUp.Font = new Font("Segoe UI", 12F);
            btnServidorUp.Location = new Point(12, 12);
            btnServidorUp.Name = "btnServidorUp";
            btnServidorUp.Size = new Size(223, 29);
            btnServidorUp.TabIndex = 4;
            btnServidorUp.Text = "Server Up";
            btnServidorUp.UseVisualStyleBackColor = true;
            // 
            // BtnsubirArchivo
            // 
            BtnsubirArchivo.Font = new Font("Segoe UI", 12F);
            BtnsubirArchivo.Location = new Point(12, 128);
            BtnsubirArchivo.Name = "BtnsubirArchivo";
            BtnsubirArchivo.Size = new Size(117, 29);
            BtnsubirArchivo.TabIndex = 6;
            BtnsubirArchivo.Text = "Subir Archivo";
            BtnsubirArchivo.UseVisualStyleBackColor = true;
            // 
            // btnCopiar
            // 
            btnCopiar.Font = new Font("Segoe UI", 12F);
            btnCopiar.Location = new Point(12, 196);
            btnCopiar.Name = "btnCopiar";
            btnCopiar.Size = new Size(70, 34);
            btnCopiar.TabIndex = 10;
            btnCopiar.Text = "Copair";
            btnCopiar.UseVisualStyleBackColor = true;
            // 
            // textBoxComando
            // 
            textBoxComando.Font = new Font("Segoe UI", 13F);
            textBoxComando.Location = new Point(88, 196);
            textBoxComando.Name = "textBoxComando";
            textBoxComando.Size = new Size(414, 31);
            textBoxComando.TabIndex = 11;
            // 
            // textBoxSubirArchivo
            // 
            textBoxSubirArchivo.Font = new Font("Segoe UI", 13F);
            textBoxSubirArchivo.Location = new Point(135, 128);
            textBoxSubirArchivo.Name = "textBoxSubirArchivo";
            textBoxSubirArchivo.Size = new Size(367, 31);
            textBoxSubirArchivo.TabIndex = 12;
            // 
            // btnServerDown
            // 
            btnServerDown.Font = new Font("Segoe UI", 12F);
            btnServerDown.Location = new Point(279, 13);
            btnServerDown.Name = "btnServerDown";
            btnServerDown.Size = new Size(223, 28);
            btnServerDown.TabIndex = 13;
            btnServerDown.Text = "Server Down";
            btnServerDown.UseVisualStyleBackColor = true;
            // 
            // richTextBoxTerminal
            // 
            richTextBoxTerminal.Location = new Point(12, 247);
            richTextBoxTerminal.Name = "richTextBoxTerminal";
            richTextBoxTerminal.Size = new Size(490, 214);
            richTextBoxTerminal.TabIndex = 14;
            richTextBoxTerminal.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 492);
            Controls.Add(richTextBoxTerminal);
            Controls.Add(btnServerDown);
            Controls.Add(textBoxSubirArchivo);
            Controls.Add(textBoxComando);
            Controls.Add(btnCopiar);
            Controls.Add(BtnsubirArchivo);
            Controls.Add(btnServidorUp);
            Controls.Add(label1);
            Controls.Add(puerto);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox puerto;
        private Label label1;
        private Button btnServidorUp;
        private Button BtnsubirArchivo;
        private Button btnCopiar;
        private TextBox textBoxComando;
        private TextBox textBoxSubirArchivo;
        private Button btnServerDown;
        private RichTextBox richTextBoxTerminal;
    }
}
