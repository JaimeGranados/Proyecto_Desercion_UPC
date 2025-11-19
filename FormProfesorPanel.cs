using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormProfesorPanel : Form
    {
        Panel panelLateral;
        Panel panelSuperior;
        Panel panelContenido;
        Label lblTitulo;
        Button btnCerrar;
        Button btnMinimizar;
        Button btnCerrarSesion;

        Panel indicadorActivo;

        Timer animacionEntrada;
        Timer animacionIndicador;

        int destinoY = 0;

        public FormProfesorPanel()
        {
            InitializeComponent();

            // 🖥️ Ahora SIEMPRE abre en pantalla completa
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            this.Opacity = 0;
            CrearInterfaz();
            ConfigurarAnimacion();
        }

        private void CrearInterfaz()
        {
          
            Color fondoClaro = Color.FromArgb(225, 228, 233);
            Color fondoOscuro = Color.FromArgb(20, 20, 20);
            Color barraSuperior = Color.FromArgb(200, 200, 200); // gris
            Color azul = Color.FromArgb(40, 120, 190);

           
            this.BackColor = fondoClaro;
            this.DoubleBuffered = true;

        
            panelLateral = new Panel();
            panelLateral.BackColor = fondoOscuro;
            panelLateral.Dock = DockStyle.Left;
            panelLateral.Width = 240;
            panelLateral.Padding = new Padding(20, 40, 0, 0);
            panelLateral.Left = -240;
            this.Controls.Add(panelLateral);

          
            indicadorActivo = new Panel();
            indicadorActivo.Size = new Size(5, 40);
            indicadorActivo.BackColor = azul;
            indicadorActivo.Visible = false;
            panelLateral.Controls.Add(indicadorActivo);

           
            panelSuperior = new Panel();
            panelSuperior.BackColor = barraSuperior;
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 45;
            panelSuperior.MouseDown += MoverVentana;
            this.Controls.Add(panelSuperior);

           
            panelContenido = new Panel();
            panelContenido.BackColor = fondoClaro;
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Padding = new Padding(40);
            this.Controls.Add(panelContenido);

        
            lblTitulo = new Label();
            lblTitulo.Text = "Profesor";
            lblTitulo.ForeColor = Color.WhiteSmoke;
            lblTitulo.Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 20);
            panelLateral.Controls.Add(lblTitulo);

          
            int posY = 100;
            CrearMenu(panelLateral, "Inicio", posY); posY += 60;
            CrearMenu(panelLateral, "Estudiantes", posY); posY += 60;
            CrearMenu(panelLateral, "Rendimiento", posY); posY += 60;
            CrearMenu(panelLateral, "Encuestas", posY); posY += 60;
            CrearMenu(panelLateral, "Configuración", posY);

            // 🔘 Cerrar sesión (NUEVO)
            btnCerrarSesion = new Button();
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.ForeColor = Color.WhiteSmoke;
            btnCerrarSesion.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.Cursor = Cursors.Hand;
            btnCerrarSesion.BackColor = Color.FromArgb(40, 40, 40);
            btnCerrarSesion.Size = new Size(200, 45);
            btnCerrarSesion.Location = new Point(20, this.Height - 150);
            btnCerrarSesion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCerrarSesion.Click += CerrarSesion_Click;

            panelLateral.Controls.Add(btnCerrarSesion);

          
            btnMinimizar = new Button();
            btnMinimizar.Text = "–";
            btnMinimizar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnMinimizar.FlatStyle = FlatStyle.Flat;
            btnMinimizar.FlatAppearance.BorderSize = 0;
            btnMinimizar.Size = new Size(40, 30);
            btnMinimizar.Location = new Point(this.Width - 90, 7);
            btnMinimizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimizar.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            btnCerrar = new Button();
            btnCerrar.Text = "✕";
            btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Size = new Size(40, 30);
            btnCerrar.Location = new Point(this.Width - 45, 7);
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.Click += (s, e) => this.Close();

            panelSuperior.Controls.Add(btnMinimizar);
            panelSuperior.Controls.Add(btnCerrar);
        }

        private void CrearMenu(Panel panel, string texto, int posY)
        {
            Label lblMenu = new Label();
            lblMenu.Text = texto;
            lblMenu.ForeColor = Color.WhiteSmoke;
            lblMenu.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblMenu.Size = new Size(200, 40);
            lblMenu.Location = new Point(25, posY);
            lblMenu.Cursor = Cursors.Hand;
            lblMenu.TextAlign = ContentAlignment.MiddleLeft;

            lblMenu.MouseEnter += (s, e) =>
            {
                lblMenu.ForeColor = Color.FromArgb(90, 160, 255);
            };
            lblMenu.MouseLeave += (s, e) =>
            {
                lblMenu.ForeColor = Color.WhiteSmoke;
            };

            lblMenu.Click += (s, e) => CargarSeccion(lblMenu, texto);

            panel.Controls.Add(lblMenu);
        }

        private void CargarSeccion(Label lblMenu, string texto)
        {
            panelContenido.Controls.Clear();
            Control control = null;

            switch (texto)
            {
                case "Inicio":
                    control = new UCInicioProfesor();
                    break;

                case "Estudiantes":
                    control = new UCEstudiantesProfesor();
                    break;

                case "Rendimiento":
                    control = new UCRendimientoProfesor();
                    break;

                case "Encuestas":
                    control = new UCEncuestasProfesor();
                    break;

                case "Configuración":
                    control = new UCConfigProfesor();
                    break;

                default:
                    control = new Label()
                    {
                        Text = "⚠ Sección no implementada",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                        ForeColor = Color.Gray
                    };
                    break;
            }

           
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                panelContenido.Controls.Add(control);
            }

           
            indicadorActivo.Visible = true;
            destinoY = lblMenu.Top;
            animacionIndicador.Start();
        }


        private void CerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro que quieres cerrar sesión?",
                "Confirmación", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ConexionGlobal.UsuarioActual = null;
                new FormLogin().Show();
                this.Close();
            }
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

                if (panelLateral.Left >= 0)
                {
                    panelLateral.Left = 0;
                    this.Opacity = 1;
                    animacionEntrada.Stop();
                }
            };

            this.Load += (s, e) =>
            {
                animacionEntrada.Start();
            };

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

        // Permite mover la ventana sin borde
        [DllImport("user32.dll")] public static extern void ReleaseCapture();
        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
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
