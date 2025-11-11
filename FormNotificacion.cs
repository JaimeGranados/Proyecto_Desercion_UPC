using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UPC.SmartRetention.UI
{
    public partial class FormNotificacion : Form
    {
        private int tiempoVisible = 3000; // milisegundos
        private Timer timer;
        private string tipo;

        public FormNotificacion(string mensaje, string tipo)
        {
            InitializeComponent();
            this.tipo = tipo;
            ConfigurarEstilo(tipo, mensaje);
        }

        private void ConfigurarEstilo(string tipo, string mensaje)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(350, 80);
            this.TopMost = true;
            this.Opacity = 0.92;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 30);

            Label lblMensaje = new Label
            {
                Text = mensaje,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Padding = new Padding(15, 0, 0, 0)
            };
            this.Controls.Add(lblMensaje);

            Color colorBorde;

            switch (tipo)
            {
                case "éxito":
                    colorBorde = Color.FromArgb(40, 167, 69);
                    break;
                case "advertencia":
                    colorBorde = Color.FromArgb(255, 193, 7);
                    break;
                case "error":
                    colorBorde = Color.FromArgb(220, 53, 69);
                    break;
                default:
                    colorBorde = Color.FromArgb(0, 123, 255);
                    break;
            }

            Panel borde = new Panel
            {
                BackColor = colorBorde,
                Dock = DockStyle.Left,
                Width = 10
            };
            this.Controls.Add(borde);

            // posición inferior derecha (sin molestar la vista)
            var pantalla = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(pantalla.Width - this.Width - 20, pantalla.Height - this.Height - 20);

            // temporizador
            timer = new Timer();
            timer.Interval = tiempoVisible;
            timer.Tick += (s, e) => this.CerrarAnimado();
            timer.Start();
        }

        private async void CerrarAnimado()
        {
            timer.Stop();
            for (double i = 0.9; i >= 0.0; i -= 0.1)
            {
                this.Opacity = i;
                await System.Threading.Tasks.Task.Delay(30);
            }
            this.Close();
        }

        public static void Mostrar(string mensaje, string tipo = "info")
        {
            var noti = new FormNotificacion(mensaje, tipo);
            noti.Show();
        }
    }
}
