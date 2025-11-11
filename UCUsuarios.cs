using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCUsuarios : UserControl
    {
        Panel panelCard;
        DataGridView dgvUsuarios;
        Button btnAgregar;

        public UCUsuarios()
        {
            InitializeComponent();
            CrearInterfaz();
            this.DoubleBuffered = true;
        }

        // ⚙️ Tu conexión local
        string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        private void CrearInterfaz()
        {
            this.BackColor = Color.FromArgb(44, 47, 51);
            this.Dock = DockStyle.Fill;

            panelCard = new Panel();
            panelCard.BackColor = Color.FromArgb(54, 57, 63);
            panelCard.Dock = DockStyle.Fill;
            panelCard.Padding = new Padding(25, 70, 25, 25);
            panelCard.DoubleBuffered(true);
            panelCard.Paint += DibujarTarjetaConSombra;

            // ➕ Botón agregar
            btnAgregar = new Button();
            btnAgregar.Text = "+ Agregar usuario";
            btnAgregar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAgregar.ForeColor = Color.White;
            btnAgregar.BackColor = Color.FromArgb(0, 120, 215);
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Size = new Size(160, 42);
            btnAgregar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAgregar.Location = new Point(panelCard.Width - btnAgregar.Width - 25, 15);
            btnAgregar.Cursor = Cursors.Hand;
            btnAgregar.Click += BtnAgregar_Click;
            btnAgregar.MouseEnter += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 150, 255);
            btnAgregar.MouseLeave += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 120, 215);

            panelCard.Resize += (s, e) =>
            {
                btnAgregar.Location = new Point(panelCard.ClientSize.Width - btnAgregar.Width - 25, 15);
            };

            // 📋 DataGridView
            dgvUsuarios = new DataGridView();
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.BackgroundColor = panelCard.BackColor;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.AllowUserToResizeRows = false;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Margin = new Padding(0, 10, 0, 0);
            dgvUsuarios.StandardTab = true;

            // 🖌 Encabezado
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 73, 78);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUsuarios.ColumnHeadersHeight = 45;
            dgvUsuarios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 🖌 Celdas
            dgvUsuarios.DefaultCellStyle.BackColor = panelCard.BackColor;
            dgvUsuarios.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvUsuarios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvUsuarios.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsuarios.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvUsuarios.RowTemplate.Height = 40;
            dgvUsuarios.GridColor = Color.FromArgb(90, 90, 90);
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(64, 67, 73);

            dgvUsuarios.CellContentClick += dgvUsuarios_CellContentClick;

            // 🚫 Evita la excepción y los saltos de color
            dgvUsuarios.CellEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgvUsuarios.ClearSelection();
                    dgvUsuarios.Rows[e.RowIndex].Selected = true;
                }
            };

            dgvUsuarios.SelectionChanged += (s, e) =>
            {
                if (dgvUsuarios.Focused && dgvUsuarios.SelectedCells.Count > 0)
                {
                    dgvUsuarios.ClearSelection();
                }
            };

            // Agregar controles
            panelCard.Controls.Add(btnAgregar);
            panelCard.Controls.Add(dgvUsuarios);
            this.Controls.Add(panelCard);

            // Cargar datos
            CargarUsuariosDesdeBD();
        }

        private void DibujarTarjetaConSombra(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            int radius = 14;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath shadowPath = RoundedRect(new Rectangle(6, 6, panel.Width - 12, panel.Height - 12), radius))
            using (PathGradientBrush pgb = new PathGradientBrush(shadowPath))
            {
                pgb.CenterColor = Color.FromArgb(100, 0, 0, 0);
                pgb.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                e.Graphics.FillPath(pgb, shadowPath);
            }

            using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, panel.Width, panel.Height), radius))
            {
                using (SolidBrush background = new SolidBrush(panel.BackColor))
                    e.Graphics.FillPath(background, path);

                panel.Region = new Region(path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Agregar nuevo usuario (función en desarrollo)", "Usuarios");
            using (var form = new FormUsuarioDetalle())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarUsuariosDesdeBD();
                }
            }
        }

        private void CargarUsuariosDesdeBD()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    U.Id AS ID,
                    CONCAT(U.Nombre, ' ', U.Apellido) AS NombreCompleto,
                    U.Correo,
                    R.NombreRol AS Rol,
                    U.Estado
                FROM Usuarios U
                INNER JOIN Roles R ON U.IdRol = R.Id";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsuarios.DataSource = dt;

                    // 🎨 Ajustes visuales
                    dgvUsuarios.Columns["ID"].Width = 60;
                    dgvUsuarios.Columns["NombreCompleto"].HeaderText = "Nombre completo";
                    dgvUsuarios.Columns["Correo"].Width = 220;
                    dgvUsuarios.Columns["Rol"].Width = 140;
                    dgvUsuarios.Columns["Estado"].Width = 100;

                    // 🧩 Añadir botones de acción si no existen
                    if (!dgvUsuarios.Columns.Contains("Editar"))
                    {
                        DataGridViewButtonColumn btnEditar = new DataGridViewButtonColumn();
                        btnEditar.HeaderText = "";
                        btnEditar.Name = "Editar";
                        btnEditar.Text = "✏️ Editar";
                        btnEditar.UseColumnTextForButtonValue = true;
                        btnEditar.FlatStyle = FlatStyle.Flat;
                        btnEditar.DefaultCellStyle.BackColor = Color.FromArgb(40, 167, 69);
                        btnEditar.DefaultCellStyle.ForeColor = Color.White;
                        dgvUsuarios.Columns.Add(btnEditar);
                    }

                    if (!dgvUsuarios.Columns.Contains("Eliminar"))
                    {
                        DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
                        btnEliminar.HeaderText = "";
                        btnEliminar.Name = "Eliminar";
                        btnEliminar.Text = "🗑 Eliminar";
                        btnEliminar.UseColumnTextForButtonValue = true;
                        btnEliminar.FlatStyle = FlatStyle.Flat;
                        btnEliminar.DefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69);
                        btnEliminar.DefaultCellStyle.ForeColor = Color.White;
                        dgvUsuarios.Columns.Add(btnEliminar);
                    }

                    dgvUsuarios.ClearSelection();
                    dgvUsuarios.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error de conexión",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Obtiene el ID del usuario
            int idUsuario = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["ID"].Value);

            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Editar")
            {
                EditarUsuario(idUsuario);
            }
            else if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                EliminarUsuario(idUsuario);
            }
        }

        private void EditarUsuario(int idUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Usuarios WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", idUsuario);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // ✅ Crear el formulario y pasar el ID correctamente
                        var form = new FormUsuarioDetalle
                        {
                            Text = "Editar usuario",
                            UsuarioId = idUsuario // << ESTA LÍNEA ES CLAVE
                        };

                        // ✅ Cargar los datos en los controles
                        form.Controls["txtNombre"].Text = reader["Nombre"].ToString();
                        form.Controls["txtApellido"].Text = reader["Apellido"].ToString();
                        form.Controls["txtCorreo"].Text = reader["Correo"].ToString();
                        form.Controls["txtContrasena"].Text = reader["Contrasena"].ToString();

                        int idRol = Convert.ToInt32(reader["IdRol"]);
                        if (form.Controls["cmbRol"] is ComboBox combo)
                            combo.SelectedIndex = idRol - 1;

                        if (form.Controls["chkActivo"] is CheckBox chk)
                            chk.Checked = Convert.ToBoolean(reader["Estado"]);

                        reader.Close();

                        if (form.ShowDialog() == DialogResult.OK)
                            CargarUsuariosDesdeBD();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void EliminarUsuario(int idUsuario)
        {
            var confirm = MessageBox.Show("¿Seguro que deseas eliminar este usuario?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Usuarios WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", idUsuario);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Usuario eliminado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }

    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(control, enable, null);
        }
    }
}
