using System.Drawing;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelOverlay;
        private Panel panelLogin;
        private Label lblTitulo;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnLogin;
        private Timer TimerFade;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelOverlay = new System.Windows.Forms.Panel();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.TimerFade = new System.Windows.Forms.Timer(this.components);
            this.panelOverlay.SuspendLayout();
            this.panelLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.panelOverlay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Opacity = 0D;
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLogin_MouseDown);
            // 
            // panelOverlay
            // 
            this.panelOverlay.BackColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            this.panelOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOverlay.Controls.Add(this.panelLogin);
            // 
            // panelLogin
            // 
            this.panelLogin.BackColor = System.Drawing.Color.FromArgb(220, 30, 30, 30);
            this.panelLogin.Size = new System.Drawing.Size(350, 400);
            this.panelLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelLogin.Controls.Add(this.lblTitulo);
            this.panelLogin.Controls.Add(this.txtUsuario);
            this.panelLogin.Controls.Add(this.txtContrasena);
            this.panelLogin.Controls.Add(this.btnLogin);
            this.panelLogin.Location = new System.Drawing.Point(275, 100);
            // 
            // lblTitulo
            // 
            this.lblTitulo.Text = "UPC SMART RETENTION";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitulo.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitulo.Dock = DockStyle.Top;
            this.lblTitulo.Height = 80;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsuario.ForeColor = System.Drawing.Color.White;
            this.txtUsuario.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            this.txtUsuario.SetBounds(50, 120, 250, 35);
            // 
            // txtContrasena
            // 
            this.txtContrasena.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtContrasena.ForeColor = System.Drawing.Color.White;
            this.txtContrasena.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtContrasena.BorderStyle = BorderStyle.FixedSingle;
            this.txtContrasena.PasswordChar = '●';
            this.txtContrasena.SetBounds(50, 180, 250, 35);
            // 
            // btnLogin
            // 
            this.btnLogin.Text = "Iniciar Sesión";
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(180, 60, 40);
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.SetBounds(50, 250, 250, 40);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            this.btnLogin.MouseEnter += new System.EventHandler(this.BtnLogin_MouseEnter);
            this.btnLogin.MouseLeave += new System.EventHandler(this.BtnLogin_MouseLeave);
            // 
            // TimerFade
            // 
            this.TimerFade.Interval = 30;
            this.TimerFade.Tick += new System.EventHandler(this.TimerFade_Tick);
            // 
            // FormLogin
            // 
            this.panelOverlay.ResumeLayout(false);
            this.panelLogin.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}

