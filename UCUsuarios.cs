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
        string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        public UCUsuarios()
        {
            InitializeComponent();
            CrearInterfaz();
            this.DoubleBuffered = true;
        }

        private void CrearInterfaz()
        {
            // Fondo general
            this.BackColor = Color.FromArgb(44, 47, 51);
            this.Dock = DockStyle.Fill;

            // panelCard centrado con tamaño razonable
            panelCard = new Panel
            {
                BackColor = Color.FromArgb(54, 57, 63),
                Anchor = AnchorStyles.None,
                Width = 1100,
                Height = 600,
                Padding = new Padding(20, 20, 20, 20)
            };
            panelCard.Paint += DibujarTarjetaConSombra;
            this.Controls.Add(panelCard);

            // Mantener centrado cuando se cambia el tamaño del parent
            this.Resize += (s, e) =>
            {
                panelCard.Left = Math.Max(0, (this.Width - panelCard.Width) / 2);
                panelCard.Top = Math.Max(0, (this.Height - panelCard.Height) / 2);
            };

            // ====== HEADER SUPERIOR ======
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(54, 57, 63),
                Padding = new Padding(5, 10, 5, 10)
            };
            panelCard.Controls.Add(header);

            // Botón Agregar
            btnAgregar = new Button
            {
                Text = "+ Agregar usuario",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 42),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Left,
                Margin = new Padding(10, 0, 10, 0)
            };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Click += BtnAgregar_Click;
            btnAgregar.MouseEnter += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 150, 255);
            btnAgregar.MouseLeave += (s, e) => btnAgregar.BackColor = Color.FromArgb(0, 120, 215);
            header.Controls.Add(btnAgregar);

            // ====== Barra de búsqueda ======
            TextBox txtBuscar = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(48, 49, 54),
                BorderStyle = BorderStyle.FixedSingle,
                Width = 300,
                Height = 36,
                Text = "Buscar usuario...",
                Dock = DockStyle.Right,
                Margin = new Padding(10)
            };

            // Placeholder simulado
            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar usuario...")
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = Color.White;
                }
            };
            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = "Buscar usuario...";
                    txtBuscar.ForeColor = Color.LightGray;
                }
            };
            header.Controls.Add(txtBuscar);

            // ====== DataGridView ======
            dgvUsuarios = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = panelCard.BackColor,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                EnableHeadersVisualStyles = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(20, 20, 20, 20)
            };

            panelCard.Controls.Add(dgvUsuarios);
            dgvUsuarios.BringToFront();

            // Evento de filtrado en tiempo real
            txtBuscar.TextChanged += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar usuario...") return;

                if (dgvUsuarios != null && dgvUsuarios.DataSource is DataTable dt)
                {
                    string filtro = txtBuscar.Text.Trim().Replace("'", "''");
                    dt.DefaultView.RowFilter = string.IsNullOrEmpty(filtro)
                        ? string.Empty
                        : $"NombreCompleto LIKE '%{filtro}%' OR Correo LIKE '%{filtro}%' OR Rol LIKE '%{filtro}%'";
                }
            };

            // Cargar datos
            CargarUsuariosDesdeBD();
        }


        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            using (var form = new FormUsuarioDetalle())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog(this) == DialogResult.OK)
                    CargarUsuariosDesdeBD();
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
                }

                // ✅ Evita duplicados de columnas personalizadas
                if (!dgvUsuarios.Columns.Contains("Editar"))
                {
                    DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn
                    {
                        Name = "Editar",
                        HeaderText = "",
                        Text = "Editar",
                        UseColumnTextForButtonValue = true,
                        FlatStyle = FlatStyle.Flat,
                        Width = 90
                    };
                    dgvUsuarios.Columns.Add(colEditar);
                }

                if (!dgvUsuarios.Columns.Contains("Eliminar"))
                {
                    DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn
                    {
                        Name = "Eliminar",
                        HeaderText = "",
                        Text = "Eliminar",
                        UseColumnTextForButtonValue = true,
                        FlatStyle = FlatStyle.Flat,
                        Width = 90
                    };
                    dgvUsuarios.Columns.Add(colEliminar);
                }

                // ✅ Forzar a que las columnas de botones sean editables
                dgvUsuarios.Columns["Editar"].ReadOnly = false;
                dgvUsuarios.Columns["Eliminar"].ReadOnly = false;

                // ✅ Volver a vincular el evento (por si el DataGridView se regeneró)
                dgvUsuarios.CellContentClick -= dgvUsuarios_CellContentClick;
                dgvUsuarios.CellContentClick += dgvUsuarios_CellContentClick;

                dgvUsuarios.ClearSelection();
                dgvUsuarios.CurrentCell = null;
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
            var idCell = dgvUsuarios.Rows[e.RowIndex].Cells["ID"];
            if (idCell?.Value == null) return;
            int idUsuario = Convert.ToInt32(idCell.Value);

            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Editar")
                EditarUsuario(idUsuario);
            else if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Eliminar")
                EliminarUsuario(idUsuario);
        }

        private void dgvUsuarios_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = dgvUsuarios.Columns[e.ColumnIndex];
            if (col == null) return;

            if (col.Name == "Editar" || col.Name == "Eliminar")
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = e.CellBounds;
                rect.Inflate(-6, -8);

                Color colorBoton = col.Name == "Editar"
                    ? Color.FromArgb(0, 120, 215)
                    : Color.FromArgb(200, 50, 50);

                using (SolidBrush brush = new SolidBrush(colorBoton))
                    e.Graphics.FillRoundedRectangle(brush, rect, 6);

                string texto = col.Name == "Editar" ? "Editar" : "Eliminar";
                TextRenderer.DrawText(e.Graphics, texto, new Font("Segoe UI", 9, FontStyle.Bold),
                    rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void EditarUsuario(int idUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idUsuario);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return;

                        string nombre = reader["Nombre"].ToString();
                        string apellido = reader["Apellido"].ToString();
                        string correo = reader["Correo"].ToString();
                        string contrasena = reader["Contrasena"].ToString();
                        int idRol = Convert.ToInt32(reader["IdRol"]);
                        bool estado = Convert.ToBoolean(reader["Estado"]);

                        using (var form = new FormUsuarioDetalle())
                        {
                            form.UsuarioId = idUsuario;
                            form.CargarDatosUsuario(nombre, apellido, correo, contrasena, idRol, estado);
                            if (form.ShowDialog(this) == DialogResult.OK)
                                CargarUsuariosDesdeBD();
                        }
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
            if (MessageBox.Show("¿Seguro que deseas eliminar este usuario?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id = @Id", conn);
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

        private void DibujarTarjetaConSombra(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            int radius = 14;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath shadowPath = RoundedRect(new Rectangle(6, 6, panel.Width - 12, panel.Height - 12), radius))
            using (PathGradientBrush pgb = new PathGradientBrush(shadowPath))
            {
                pgb.CenterColor = Color.FromArgb(100, 0, 0, 0);
                pgb.SurroundColors = new[] { Color.Transparent };
                e.Graphics.FillPath(pgb, shadowPath);
            }

            using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, panel.Width, panel.Height), radius))
            using (SolidBrush brush = new SolidBrush(panel.BackColor))
                e.Graphics.FillPath(brush, path);
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
    }

    // Extensión para dibujo redondeado
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int arc = radius * 2;
                path.AddArc(bounds.Left, bounds.Top, arc, arc, 180, 90);
                path.AddArc(bounds.Right - arc, bounds.Top, arc, arc, 270, 90);
                path.AddArc(bounds.Right - arc, bounds.Bottom - arc, arc, arc, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - arc, arc, arc, 90, 90);
                path.CloseFigure();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }
        }
    }
}
