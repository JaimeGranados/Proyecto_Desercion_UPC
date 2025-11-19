using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCMisEncuestas : UserControl
    {
        private DataGridView dgvEncuestas;
        private Button btnResponder;
        private string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        public int IdEstudiante { get; set; } // Se asignará desde el login

        public UCMisEncuestas()
        {
            InitializeComponent();
            CrearInterfaz();
            CargarEncuestasActivas();
        }

        private void CrearInterfaz()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(44, 47, 51);

            Label lblTitulo = new Label
            {
                Text = "📝 Encuestas disponibles",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 25)
            };
            this.Controls.Add(lblTitulo);

            dgvEncuestas = new DataGridView
            {
                Location = new Point(30, 80),
                Size = new Size(1000, 450),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(55, 56, 60),
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false
            };

            dgvEncuestas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 36, 40);
            dgvEncuestas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEncuestas.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55);
            dgvEncuestas.DefaultCellStyle.ForeColor = Color.White;
            dgvEncuestas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            dgvEncuestas.RowTemplate.Height = 35;

            this.Controls.Add(dgvEncuestas);

            // Botón responder
            btnResponder = new Button
            {
                Text = "Responder encuesta seleccionada",
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(300, 45),
                Location = new Point(30, 550)
            };
            btnResponder.FlatAppearance.BorderSize = 0;
            btnResponder.Click += BtnResponder_Click;
            this.Controls.Add(btnResponder);
        }

        private void CargarEncuestasActivas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT Id, Titulo, Descripcion, FechaCreacion 
                      FROM Encuestas 
                      WHERE Activo = 1
                      ORDER BY FechaCreacion DESC", conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEncuestas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar encuestas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnResponder_Click(object sender, EventArgs e)
        {
            if (dgvEncuestas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una encuesta para responder.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idEncuesta = Convert.ToInt32(dgvEncuestas.SelectedRows[0].Cells["Id"].Value);

            using (var form = new FormResponderEncuesta(idEncuesta, IdEstudiante))
            {
                form.ShowDialog();
            }
        }
    }
}
