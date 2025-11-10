using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormLogin : Form
    {
        private float fadeStep = 0.05f;

        public FormLogin()
        {
            InitializeComponent();
            RedondearBordes();

            this.BackgroundImage = Properties.Resources.album_joji;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;

            SetPlaceholder(txtUsuario, "Usuario");
            SetPlaceholder(txtContrasena, "Contraseña");

            CenterPanel();
            this.Resize += (s, e) => CenterPanel();
        }

        private void RedondearBordes()
        {
            // Bordes redondeados del panel principal
            panelLogin.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelLogin.Width, panelLogin.Height, 30, 30)
            );

            // Redondear los cuadros de texto
            RedondearControl(txtUsuario, 10);
            RedondearControl(txtContrasena, 10);

            // Redondear botón
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

        // Import para crear regiones redondeadas
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            TimerFade.Start();
        }

        private void TimerFade_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1)
                this.Opacity += fadeStep;
            else
                TimerFade.Stop();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena) ||
                usuario == "Usuario" || contrasena == "Contraseña")
            {
                MessageBox.Show("Ingrese usuario y contraseña", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: conectar a la base de datos y validar usuario; por ahora demo:
            if (usuario == "admin" && contrasena == "1234")
            {
                MessageBox.Show("Inicio de sesión correcto", "Acceso permitido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(200, 80, 60);
        }

        private void BtnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(180, 60, 40);
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
                    if (textBox == txtContrasena)
                        textBox.UseSystemPasswordChar = true;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.ForeColor = Color.Gray;
                    textBox.Text = placeholder;
                    if (textBox == txtContrasena)
                        textBox.UseSystemPasswordChar = false;
                }
            };
        }

        // Permite mover la ventana sin borde
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
