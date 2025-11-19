using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace UPC.SmartRetention.UI
{
    public partial class UCEstadisticas : UserControl
    {
        Panel panelCard;
        FlowLayoutPanel panelResumen;
        Chart chartUsuarios;
        string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        public UCEstadisticas()
        {
            InitializeComponent();
            CrearInterfaz();
            CargarDatos();
        }

        private void CrearInterfaz()
        {
            // Fondo general
            this.BackColor = Color.FromArgb(44, 47, 51);
            this.Dock = DockStyle.Fill;

            // Panel principal tipo “card” centrado
            panelCard = new Panel
            {
                BackColor = Color.FromArgb(54, 57, 63),
                Anchor = AnchorStyles.None,
                Width = 1000,
                Height = 650,
                Padding = new Padding(30, 30, 30, 20)
            };
            this.Controls.Add(panelCard);

            // Centrado dinámico
            this.Resize += (s, e) =>
            {
                panelCard.Left = Math.Max(0, (this.Width - panelCard.Width) / 2);
                panelCard.Top = Math.Max(0, (this.Height - panelCard.Height) / 2);
            };

            // Panel superior de tarjetas
            panelResumen = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                AutoSize = false,
                BackColor = Color.FromArgb(54, 57, 63)
            };
            panelCard.Controls.Add(panelResumen);

            // Agregar tarjetas vacías (se llenarán al cargar datos)
            CrearTarjeta("Administradores", Color.FromArgb(200, 50, 50));
            CrearTarjeta("Profesores", Color.FromArgb(255, 165, 0));
            CrearTarjeta("Estudiantes", Color.FromArgb(0, 200, 150));

            // Gráfico
            chartUsuarios = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(54, 57, 63)
            };

            ChartArea area = new ChartArea("AreaPrincipal");
            area.BackColor = Color.FromArgb(54, 57, 63);
            area.AxisX.LabelStyle.ForeColor = Color.WhiteSmoke;
            area.AxisY.LabelStyle.ForeColor = Color.WhiteSmoke;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);
            area.AxisX.Interval = 1;
            area.AxisY.Minimum = 0;
            chartUsuarios.ChartAreas.Add(area);

            Series serie = new Series("Usuarios por rol");
            serie.ChartType = SeriesChartType.Column;
            serie.Color = Color.FromArgb(0, 180, 255);
            serie.BorderWidth = 0;
            serie["PointWidth"] = "0.4";
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            serie.LabelForeColor = Color.White;
            chartUsuarios.Series.Add(serie);

            Title titulo = new Title("Distribución de usuarios por rol",
                Docking.Top,
                new Font("Segoe UI Semibold", 14, FontStyle.Bold),
                Color.WhiteSmoke);
            chartUsuarios.Titles.Add(titulo);

            panelCard.Controls.Add(chartUsuarios);
            chartUsuarios.BringToFront();
        }

        private void CrearTarjeta(string titulo, Color colorBorde)
        {
            Panel tarjeta = new Panel
            {
                Width = 200,
                Height = 80,
                BackColor = Color.FromArgb(44, 47, 51),
                Margin = new Padding(15, 5, 15, 5)
            };

            tarjeta.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(colorBorde, 2))
                    e.Graphics.DrawRectangle(pen, 0, 0, tarjeta.Width - 1, tarjeta.Height - 1);
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Gainsboro,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 25,
                Padding = new Padding(10, 5, 0, 0)
            };

            Label lblValor = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold),
                ForeColor = colorBorde,
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Name = "lbl" + titulo.Replace(" ", "")
            };

            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblValor);
            panelResumen.Controls.Add(tarjeta);
        }

        private void CargarDatos()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT R.NombreRol, COUNT(U.Id) AS Cantidad
                FROM Roles R
                LEFT JOIN Usuarios U ON U.IdRol = R.Id
                GROUP BY R.NombreRol";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    chartUsuarios.Series[0].Points.Clear();

                    // Reiniciar valores a 0
                    foreach (Control ctrl in panelResumen.Controls)
                    {
                        if (ctrl is Panel panel)
                        {
                            foreach (Control c in panel.Controls)
                                if (c is Label lbl && lbl.Name.StartsWith("lbl"))
                                    lbl.Text = "0";
                        }
                    }

                    while (reader.Read())
                    {
                        string rol = reader["NombreRol"].ToString().Trim();
                        int cantidad = Convert.ToInt32(reader["Cantidad"]);

                        chartUsuarios.Series[0].Points.AddXY(rol, cantidad);

                        // Normalizar el nombre para que coincida con los labels
                        string key = rol.ToLower();

                        foreach (Control ctrl in panelResumen.Controls)
                        {
                            if (ctrl is Panel panel)
                            {
                                foreach (Control c in panel.Controls)
                                {
                                    if (c is Label lbl && lbl.Name.StartsWith("lbl"))
                                    {
                                        if ((key.Contains("admin") && lbl.Name.Contains("Administradores")) ||
                                            (key.Contains("prof") && lbl.Name.Contains("Profesores")) ||
                                            (key.Contains("est") && lbl.Name.Contains("Estudiantes")))
                                        {
                                            lbl.Text = cantidad.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
