namespace UPC.SmartRetention.UI
{
    partial class FormEstudianteEditar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.cmbPrograma = new System.Windows.Forms.ComboBox();
            this.nudSemestre = new System.Windows.Forms.NumericUpDown();
            this.nudPromedio = new System.Windows.Forms.NumericUpDown();
            this.nudAsistencia = new System.Windows.Forms.NumericUpDown();
            this.nudEstrato = new System.Windows.Forms.NumericUpDown();
            this.cmbRiesgo = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudSemestre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPromedio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAsistencia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEstrato)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoEllipsis = true;
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblTitulo.Location = new System.Drawing.Point(293, 40);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(102, 13);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Registrar Estudiante";
            this.lblTitulo.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(65, 87);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(162, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // cmbPrograma
            // 
            this.cmbPrograma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrograma.FormattingEnabled = true;
            this.cmbPrograma.Location = new System.Drawing.Point(64, 127);
            this.cmbPrograma.Name = "cmbPrograma";
            this.cmbPrograma.Size = new System.Drawing.Size(162, 21);
            this.cmbPrograma.TabIndex = 2;
            // 
            // nudSemestre
            // 
            this.nudSemestre.Location = new System.Drawing.Point(67, 162);
            this.nudSemestre.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nudSemestre.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSemestre.Name = "nudSemestre";
            this.nudSemestre.Size = new System.Drawing.Size(159, 20);
            this.nudSemestre.TabIndex = 3;
            this.nudSemestre.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudPromedio
            // 
            this.nudPromedio.DecimalPlaces = 2;
            this.nudPromedio.Location = new System.Drawing.Point(66, 219);
            this.nudPromedio.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudPromedio.Name = "nudPromedio";
            this.nudPromedio.Size = new System.Drawing.Size(160, 20);
            this.nudPromedio.TabIndex = 4;
            // 
            // nudAsistencia
            // 
            this.nudAsistencia.Location = new System.Drawing.Point(70, 269);
            this.nudAsistencia.Name = "nudAsistencia";
            this.nudAsistencia.Size = new System.Drawing.Size(156, 20);
            this.nudAsistencia.TabIndex = 5;
            // 
            // nudEstrato
            // 
            this.nudEstrato.Location = new System.Drawing.Point(71, 314);
            this.nudEstrato.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudEstrato.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEstrato.Name = "nudEstrato";
            this.nudEstrato.Size = new System.Drawing.Size(156, 20);
            this.nudEstrato.TabIndex = 6;
            this.nudEstrato.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmbRiesgo
            // 
            this.cmbRiesgo.FormattingEnabled = true;
            this.cmbRiesgo.Items.AddRange(new object[] {
            "Bajo",
            "Medio",
            "Alto"});
            this.cmbRiesgo.Location = new System.Drawing.Point(396, 87);
            this.cmbRiesgo.Name = "cmbRiesgo";
            this.cmbRiesgo.Size = new System.Drawing.Size(156, 21);
            this.cmbRiesgo.TabIndex = 7;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(370, 127);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(173, 59);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(370, 198);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(173, 59);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // FormEstudianteEditar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.cmbRiesgo);
            this.Controls.Add(this.nudEstrato);
            this.Controls.Add(this.nudAsistencia);
            this.Controls.Add(this.nudPromedio);
            this.Controls.Add(this.nudSemestre);
            this.Controls.Add(this.cmbPrograma);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblTitulo);
            this.Name = "FormEstudianteEditar";
            this.Text = "FormEstudianteEditar";
            ((System.ComponentModel.ISupportInitialize)(this.nudSemestre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPromedio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAsistencia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEstrato)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.ComboBox cmbPrograma;
        private System.Windows.Forms.NumericUpDown nudSemestre;
        private System.Windows.Forms.NumericUpDown nudPromedio;
        private System.Windows.Forms.NumericUpDown nudAsistencia;
        private System.Windows.Forms.NumericUpDown nudEstrato;
        private System.Windows.Forms.ComboBox cmbRiesgo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}