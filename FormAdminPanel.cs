using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormAdminPanel : Form
    {
        Panel panelLateral;
        Panel panelSuperior;
        Panel panelContenido;
        Label lblTitulo;
        Button btnCerrar;
        Button btnMinimizar;
        Panel indicadorActivo;

        Timer animacionEntrada;
        Timer animacionSalida;
        Timer animacionIndicador;

        int animStep = 0;
        bool cerrarSesion = false;
        int destinoY = 0;

        public FormAdminPanel()
        {
            InitializeComponent();
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterScreen;
            CrearInterfaz();
            ConfigurarAnimacion();
            ConfigurarSalida();
        }

        private void CrearInterfaz()
        {
            // 🎨 Colores base
            Color fondoClaro = Color.FromArgb(225, 228, 233);
            Color fondoOscuro = Color.FromArgb(20, 20, 20);
            Color barraSuperior = Color.FromArgb(242, 243, 247);
            Color azulMarino = Color.FromArgb(40, 70, 120);

            // 🪟 Propiedades base
            this.BackColor = fondoClaro;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1300, 800);
            this.DoubleBuffered = true;

            // ⚫ Panel lateral
            panelLateral = new Panel();
            panelLateral.BackColor = fondoOscuro;
            panelLateral.Dock = DockStyle.Left;
            panelLateral.Width = 240;
            panelLateral.Padding = new Padding(20, 40, 0, 0);
            panelLateral.Left = -240;
            this.Controls.Add(panelLateral);

            // 🔷 Indicador activo (barra azul marino)
            indicadorActivo = new Panel();
            indicadorActivo.Size = new Size(5, 40);
            indicadorActivo.BackColor = azulMarino;
            indicadorActivo.Visible = false;
            panelLateral.Controls.Add(indicadorActivo);

            // 🎯 Panel superior
            panelSuperior = new Panel();
            panelSuperior.BackColor = Color.FromArgb(240, 242, 245); // Gris muy suave
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 45;
            panelSuperior.Padding = new Padding(10, 5, 10, 5);

            
            panelSuperior.MouseDown += MoverVentana;

            
            this.Controls.Add(panelSuperior);
            panelSuperior.BringToFront();


            // ⚪ Panel de contenido
            panelContenido = new Panel();
            panelContenido.BackColor = fondoClaro;
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Padding = new Padding(30);
            this.Controls.Add(panelContenido);

            // 🧑 Título lateral
            lblTitulo = new Label();
            lblTitulo.Text = "Administrador";
            lblTitulo.ForeColor = Color.WhiteSmoke;
            lblTitulo.Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 20);
            panelLateral.Controls.Add(lblTitulo);

            // 🧭 Menú
            int posY = 100;
            CrearMenu(panelLateral, "Usuarios", posY); posY += 60;
            CrearMenu(panelLateral, "Estadísticas", posY); posY += 60;
            CrearMenu(panelLateral, "Encuestas", posY); posY += 60;
            CrearMenu(panelLateral, "Configuración", posY); posY += 60;
            CrearMenu(panelLateral, "Cerrar Sesión", posY);

            // 🔘 Botones de control
            btnMinimizar = new Button();
            btnMinimizar.Text = "–";
            btnMinimizar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnMinimizar.ForeColor = Color.Black;
            btnMinimizar.BackColor = Color.Transparent;
            btnMinimizar.FlatStyle = FlatStyle.Flat;
            btnMinimizar.FlatAppearance.BorderSize = 0;
            btnMinimizar.Size = new Size(40, 30);
            btnMinimizar.Location = new Point(this.Width - 90, 7);
            btnMinimizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimizar.Cursor = Cursors.Hand;
            btnMinimizar.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            btnCerrar = new Button();
            btnCerrar.Text = "✕";
            btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCerrar.ForeColor = Color.Black;
            btnCerrar.BackColor = Color.Transparent;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Size = new Size(40, 30);
            btnCerrar.Location = new Point(this.Width - 45, 7);
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.Click += (s, e) => this.Close();

            // Hover
            btnCerrar.MouseEnter += (s, e) => btnCerrar.BackColor = Color.FromArgb(255, 90, 90);
            btnCerrar.MouseLeave += (s, e) => btnCerrar.BackColor = Color.Transparent;
            btnMinimizar.MouseEnter += (s, e) => btnMinimizar.BackColor = Color.FromArgb(230, 230, 230);
            btnMinimizar.MouseLeave += (s, e) => btnMinimizar.BackColor = Color.Transparent;

            panelSuperior.Controls.Add(btnMinimizar);
            panelSuperior.Controls.Add(btnCerrar);
        }

        private void ConfigurarAnimacion()
        {
            animacionEntrada = new Timer();
            animacionEntrada.Interval = 15;
            animacionEntrada.Tick += (s, e) =>
            {
                if (panelLateral.Left < 0)
                    panelLateral.Left += 20;

                if (this.Opacity < 1)
                    this.Opacity += 0.05;

                animStep++;
                if (animStep > 25)
                {
                    animacionEntrada.Stop();
                    this.Opacity = 1;
                    panelLateral.Left = 0;
                }
            };

            this.Load += (s, e) =>
            {
                animacionEntrada.Start();
                RedondearBordes(this, 20);
                RedondearBordes(panelLateral, 20);
            };

            // 🟦 Animación del indicador
            animacionIndicador = new Timer();
            animacionIndicador.Interval = 10;
            animacionIndicador.Tick += (s, e) =>
            {
                if (Math.Abs(indicadorActivo.Top - destinoY) > 2)
                {
                    int paso = (destinoY - indicadorActivo.Top) / 5;
                    indicadorActivo.Top += paso;
                }
                else
                {
                    indicadorActivo.Top = destinoY;
                    animacionIndicador.Stop();
                }
            };
        }

        private void ConfigurarSalida()
        {
            animacionSalida = new Timer();
            animacionSalida.Interval = 15;
            animacionSalida.Tick += (s, e) =>
            {
                if (panelLateral.Left > -240)
                    panelLateral.Left -= 20;

                if (this.Opacity > 0)
                    this.Opacity -= 0.05;

                if (this.Opacity <= 0)
                {
                    animacionSalida.Stop();
                    if (cerrarSesion)
                    {
                        FormLogin login = new FormLogin();
                        login.Show();
                    }
                    this.Close();
                }
            };
        }

        private void CrearMenu(Panel panel, string texto, int posY)
        {
            Label lblMenu = new Label();
            lblMenu.Text = texto;
            lblMenu.ForeColor = Color.WhiteSmoke;
            lblMenu.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblMenu.AutoSize = false;
            lblMenu.Size = new Size(200, 40);
            lblMenu.Location = new Point(25, posY);
            lblMenu.Cursor = Cursors.Hand;
            lblMenu.TextAlign = ContentAlignment.MiddleLeft;

            lblMenu.MouseEnter += (s, e) =>
            {
                lblMenu.ForeColor = Color.FromArgb(255, 100, 100);
                lblMenu.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
            };
            lblMenu.MouseLeave += (s, e) =>
            {
                lblMenu.ForeColor = Color.WhiteSmoke;
                lblMenu.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            };

            lblMenu.Click += (s, e) =>
            {
                if (texto == "Cerrar Sesión")
                {
                    cerrarSesion = true;
                    animacionSalida.Start();
                }
                else
                {
                    panelContenido.Controls.Clear();
                    Control control = null;

                    switch (texto)
                    {
                        case "Usuarios":
                            control = new UCUsuarios();
                            break;
                        case "Estadísticas":
                            control = new UCEstadisticas();
                            break;
                        case "Encuestas":
                            control = new UCEncuestas();
                            break;
                        default:
                            control = new Label()
                            {
                                Text = $"📄 Sección: {texto} (en desarrollo)",
                                AutoSize = true,
                                Font = new Font("Segoe UI", 14, FontStyle.Italic),
                                ForeColor = Color.Gray,
                                Location = new Point(50, 50)
                            };
                            break;
                    }


                    control.Dock = DockStyle.Fill;
                    panelContenido.Controls.Add(control);

                    // Mover el indicador
                    indicadorActivo.Visible = true;
                    destinoY = lblMenu.Top;
                    animacionIndicador.Start();
                }
            };

            panel.Controls.Add(lblMenu);
        }

        private void RedondearBordes(Control control, int radio)
        {
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radio, radio, 180, 90);
            path.AddArc(rect.Right - radio, rect.Y, radio, radio, 270, 90);
            path.AddArc(rect.Right - radio, rect.Bottom - radio, radio, radio, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radio, radio, radio, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x00020000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        [DllImport("user32.dll")]
        public static extern void ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void MoverVentana(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }
    }
}
