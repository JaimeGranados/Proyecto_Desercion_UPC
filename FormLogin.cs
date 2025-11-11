using System;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormLogin : Form
    {
        private float fadeStep = 0.05f;
        private Timer animacionEntrada;
        private int movimientoY = 30; // distancia inicial del movimiento

        public FormLogin()
        {
            InitializeComponent();

            Database.SetInitializer<ProyectoVisualEntities>(null);

            RedondearBordes();

            this.BackgroundImage = Properties.Resources.album_joji;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;

            SetPlaceholder(txtUsuario, "Usuario");
            SetPlaceholder(txtContraseña, "Contraseña");

            CenterPanel();
            this.Resize += (s, e) => CenterPanel();

            
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += FormLogin_Load;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            animacionEntrada = new Timer();
            animacionEntrada.Interval = 15;
            animacionEntrada.Tick += (s, ev) =>
            {
                // Efecto de aparición gradual
                if (this.Opacity < 1)
                    this.Opacity += fadeStep;

                // Movimiento vertical suave hacia arriba
                if (movimientoY > 0)
                {
                    this.Top -= 1;
                    movimientoY -= 1;
                }

                if (this.Opacity >= 1 && movimientoY <= 0)
                    animacionEntrada.Stop();
            };

            animacionEntrada.Start();
        }

        private void TimerFade_Tick(object sender, EventArgs e)
        {
            // misma lógica que tenías antes: aumenta la opacidad hasta 1
            if (this.Opacity < 1)
                this.Opacity += fadeStep;
            else
                TimerFade.Stop(); // Nota: TimerFade es el Timer que creó el diseñador
        }



        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string correo = txtUsuario.Text.Trim();
            string contrasena = txtContraseña.Text.Trim();

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("Por favor ingrese usuario y contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var db = ConexionGlobal.Contexto;
            var usuario = db.Usuarios.FirstOrDefault(u => u.Correo == correo && u.Contrasena == contrasena);

            if (usuario == null)
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            FormAdminPanel panelAdmin = new FormAdminPanel();
            panelAdmin.Show();

            this.Hide();
        }

        
        private void RedondearBordes()
        {
            panelLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panelLogin.Width, panelLogin.Height, 30, 30));

            RedondearControl(txtUsuario, 10);
            RedondearControl(txtContraseña, 10);
            RedondearControl(btnLogin, 15);
        }

        private void RedondearControl(Control control, int radio)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(control.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(control.Width - radio, control.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, control.Height - radio, radio, radio, 90, 90);
            path.CloseAllFigures();
            control.Region = new Region(path);
        }

        
        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.White;
                    if (textBox == txtContraseña)
                        textBox.UseSystemPasswordChar = true;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.ForeColor = Color.Gray;
                    textBox.Text = placeholder;
                    if (textBox == txtContraseña)
                        textBox.UseSystemPasswordChar = false;
                }
            };
        }

        
        private void CenterPanel()
        {
            if (panelLogin != null)
            {
                int x = (this.ClientSize.Width - panelLogin.Width) / 2;
                int y = (this.ClientSize.Height - panelLogin.Height) / 2;
                panelLogin.Location = new Point(Math.Max(0, x), Math.Max(0, y));
            }
        }

        // 🎨 Hover del botón
        private void BtnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(200, 80, 60);
        }

        private void BtnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(180, 60, 40);
        }

        
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        
        [DllImport("user32.dll")]
        public static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
