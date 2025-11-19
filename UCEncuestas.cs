using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCEncuestas : UserControl
    {
        private DataGridView dgvEncuestas;
        private TextBox txtBuscar;
        private Button btnAgregar;
        private Label lblPlaceholder;
        private string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        public UCEncuestas()
        {
            InitializeComponent();
            InicializarInterfaz();
            CargarEncuestas();
        }

        private void InicializarInterfaz()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(40, 42, 48);

            // === Panel principal tipo "tarjeta" centrado ===
            Panel panelCard = new Panel
            {
                BackColor = Color.FromArgb(54, 57, 63),
                Width = 1100,
                Height = 600,
                Padding = new Padding(20, 70, 20, 20)
            };

            // Redondear bordes y agregar sombra visual
            panelCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 15;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(panelCard.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(panelCard.Width - radius, panelCard.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, panelCard.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    using (var shadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                        g.FillPath(shadow, path);

                    using (var fill = new SolidBrush(panelCard.BackColor))
                        g.FillPath(fill, path);
                }
            };

            this.Controls.Add(panelCard);

            // Centrar dinámicamente
            this.Resize += (s, e) =>
            {
                panelCard.Left = Math.Max(0, (this.Width - panelCard.Width) / 2);
                panelCard.Top = Math.Max(0, (this.Height - panelCard.Height) / 2);
            };

            // === Título ===
            Label lblTitulo = new Label
            {
                Text = "📋 Gestión de Encuestas",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            panelCard.Controls.Add(lblTitulo);

            // === Botón agregar ===
            btnAgregar = new Button
            {
                Text = "+ Crear encuesta",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 42),
                Cursor = Cursors.Hand
            };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Location = new Point(panelCard.Width - btnAgregar.Width - 40, 25);
            btnAgregar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAgregar.Click += BtnAgregar_Click;
            btnAgregar.MouseEnter += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 150, 255);
            btnAgregar.MouseLeave += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 120, 215);
            panelCard.Controls.Add(btnAgregar);

            // === Campo de búsqueda ===
            txtBuscar = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(48, 49, 54),
                BorderStyle = BorderStyle.FixedSingle,
                Width = 400,
                Height = 36,
                Text = "Buscar encuesta..."
            };

            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar encuesta...")
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = Color.White;
                }
            };
            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = "Buscar encuesta...";
                    txtBuscar.ForeColor = Color.LightGray;
                }
            };
            txtBuscar.TextChanged += (s, e) =>
            {
                if (txtBuscar.Text != "Buscar encuesta...")
                    FiltrarEncuestas(txtBuscar.Text);
            };
            panelCard.Controls.Add(txtBuscar);
            txtBuscar.Location = new Point(25, 80);

            // === DataGridView ===
            dgvEncuestas = new DataGridView
            {
                Dock = DockStyle.None,
                Location = new Point(25, 130), // <-- bajamos un poco la tabla
                Width = panelCard.Width - 50,
                Height = panelCard.Height - 180,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.FromArgb(54, 57, 63),
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };


            dgvEncuestas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 73, 78);
            dgvEncuestas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEncuestas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvEncuestas.ColumnHeadersHeight = 45;
            dgvEncuestas.DefaultCellStyle.BackColor = Color.FromArgb(58, 59, 65);
            dgvEncuestas.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvEncuestas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvEncuestas.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvEncuestas.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvEncuestas.RowTemplate.Height = 38;
            dgvEncuestas.GridColor = Color.FromArgb(90, 90, 90);
            dgvEncuestas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(64, 67, 73);
            panelCard.Controls.Add(dgvEncuestas);

            // === Cargar encuestas ===
            CargarEncuestas();
        }

        private void CargarEncuestas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, Titulo, Descripcion, FechaCreacion, Activo FROM Encuestas ORDER BY FechaCreacion DESC", conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEncuestas.DataSource = dt;

                    if (!dgvEncuestas.Columns.Contains("Editar"))
                        AgregarBotones();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar encuestas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarBotones()
        {
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn
            {
                HeaderText = "Editar",
                Text = "✏️ Editar",
                UseColumnTextForButtonValue = true,
                Width = 80,
                Name = "Editar"
            };
            dgvEncuestas.Columns.Add(colEditar);

            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn
            {
                HeaderText = "Eliminar",
                Text = "🗑️ Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 80,
                Name = "Eliminar"
            };
            dgvEncuestas.Columns.Add(colEliminar);

            dgvEncuestas.CellClick += DgvEncuestas_CellClick;
        }

        private void DgvEncuestas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvEncuestas.Rows[e.RowIndex].Cells["Id"].Value);

            if (e.ColumnIndex == dgvEncuestas.Columns["Editar"].Index)
            {
                using (var form = new FormEncuestaDetalle(id))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        CargarEncuestas();
                }
            }
            else if (e.ColumnIndex == dgvEncuestas.Columns["Eliminar"].Index)
            {
                if (MessageBox.Show("¿Eliminar esta encuesta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    EliminarEncuesta(id);
                    CargarEncuestas();
                }
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            using (var form = new FormEncuestaDetalle())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    CargarEncuestas();
            }
        }

        private void EliminarEncuesta(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        DELETE FROM Opciones WHERE IdPregunta IN (SELECT Id FROM Preguntas WHERE IdEncuesta = @Id);
                        DELETE FROM Preguntas WHERE IdEncuesta = @Id;
                        DELETE FROM Encuestas WHERE Id = @Id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Encuesta eliminada correctamente.", "Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarEncuestas(string texto)
        {
            if (dgvEncuestas.DataSource is DataTable dt)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    dt.DefaultView.RowFilter = "";
                else
                    dt.DefaultView.RowFilter = $"Titulo LIKE '%{texto.Replace("'", "''")}%' OR Descripcion LIKE '%{texto.Replace("'", "''")}%' ";
            }
        }
    }
}
