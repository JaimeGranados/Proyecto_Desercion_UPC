using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UPC.SmartRetention.UI
{
    public partial class FormUsuarioDetalle : Form
    {
        public int? UsuarioId { get; set; } // null = nuevo usuario
        private readonly string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        private TextBox txtNombre, txtApellido, txtCorreo, txtContrasena;
        private ComboBox cmbRol;
        private CheckBox chkActivo;
        private Button btnGuardar, btnCancelar;

        public FormUsuarioDetalle()
        {
            InitializeComponent();
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            // ⚙️ Configuración base del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(540, 750); // 👈 un poco más alto
            this.BackColor = Color.FromArgb(28, 29, 33);
            this.DoubleBuffered = true;
            this.Opacity = 0.98;

            // 📦 Panel principal
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(38, 39, 43),
                Padding = new Padding(35, 35, 35, 45)
            };
            this.Controls.Add(panel);

            panel.Paint += (s, e) => DibujarSombraSuave(e.Graphics, panel);
            RedondearBordes(panel, 25);

            
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80, 
                Padding = new Padding(5, 10, 0, 0)
            };
            panel.Controls.Add(headerPanel);

            Label lblTitulo = new Label
            {
                Text = "Gestión de Usuario",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblTitulo);

            // 📋 Contenedor de campos
            FlowLayoutPanel layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(0, 10, 0, 0)
            };
            panel.Controls.Add(layout);
            panel.Controls.SetChildIndex(layout, 0); 

            // 🧱 Campos
            layout.Controls.Add(CrearLabel("Nombre"));
            txtNombre = CrearTextBox();
            layout.Controls.Add(txtNombre);

            layout.Controls.Add(CrearLabel("Apellido"));
            txtApellido = CrearTextBox();
            layout.Controls.Add(txtApellido);

            layout.Controls.Add(CrearLabel("Correo institucional"));
            txtCorreo = CrearTextBox();
            layout.Controls.Add(txtCorreo);

            layout.Controls.Add(CrearLabel("Contraseña"));
            txtContrasena = CrearTextBox(true);
            layout.Controls.Add(txtContrasena);

            layout.Controls.Add(CrearLabel("Rol"));
            cmbRol = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(48, 49, 54),
                ForeColor = Color.White,
                Width = 420,
                Margin = new Padding(0, 6, 0, 12)
            };
            cmbRol.Items.AddRange(new object[] { "Administrador", "Profesor", "Estudiante" });
            layout.Controls.Add(cmbRol);

            chkActivo = new CheckBox
            {
                Text = "Usuario activo",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Checked = true,
                AutoSize = true,
                Margin = new Padding(5, 10, 0, 10)
            };
            layout.Controls.Add(chkActivo);

            // 🎛 Panel inferior de botones
            FlowLayoutPanel botonesPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Padding = new Padding(0, 15, 0, 0),
                Height = 80
            };
            panel.Controls.Add(botonesPanel);

            btnGuardar = CrearBoton("Guardar", Color.FromArgb(200, 30, 30));
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = CrearBoton("Cancelar", Color.FromArgb(70, 70, 75));
            btnCancelar.Click += (s, e) => this.Close();

            botonesPanel.Controls.Add(btnGuardar);
            botonesPanel.Controls.Add(btnCancelar);
        }

        public void CargarDatosUsuario(string nombre, string apellido, string correo, string contrasena, int idRol, bool activo)
        {
            txtNombre.Text = nombre;
            txtApellido.Text = apellido;
            txtCorreo.Text = correo;
            txtContrasena.Text = contrasena;
            cmbRol.SelectedIndex = idRol - 1; // porque tus roles empiezan en 1 en BD
            chkActivo.Checked = activo;
        }


        private Label CrearLabel(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
        }

        private TextBox CrearTextBox(bool esPassword = false)
        {
            var txt = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(48, 49, 54),
                BorderStyle = BorderStyle.FixedSingle,
                Width = 420,
                Height = 36,
                Margin = new Padding(0, 6, 0, 12),
                PasswordChar = esPassword ? '●' : '\0'
            };

            txt.GotFocus += (s, e) => txt.BackColor = Color.FromArgb(55, 56, 61);
            txt.LostFocus += (s, e) => txt.BackColor = Color.FromArgb(48, 49, 54);

            return txt;
        }

        private Button CrearBoton(string texto, Color colorFondo)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI Semibold", 11),
                ForeColor = Color.White,
                BackColor = colorFondo,
                FlatStyle = FlatStyle.Flat,
                Width = 130,
                Height = 42,
                Margin = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(colorFondo);
            btn.MouseLeave += (s, e) => btn.BackColor = colorFondo;
            return btn;
        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            // 🔍 Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                FormNotificacion.Mostrar("Por favor, completa todos los campos.", "advertencia");
                return;
            }

            // 🔹 Validar correo institucional UPC
            string correo = txtCorreo.Text.Trim().ToLower();
            correo = new string(correo.Where(c => !char.IsWhiteSpace(c)).ToArray());

            if (!Regex.IsMatch(correo, @"^[a-zA-Z0-9._%+-]+@upc\.edu$"))
            {
                FormNotificacion.Mostrar("Debes ingresar un correo institucional válido (@upc.edu).", "advertencia");
                return;
            }

            // 🔹 Validar selección de rol
            if (cmbRol.SelectedIndex == -1)
            {
                FormNotificacion.Mostrar("Selecciona un rol antes de continuar.", "advertencia");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query;

                    // ✅ Nuevo registro si UsuarioId es NULL o 0
                    if (!UsuarioId.HasValue || UsuarioId.Value == 0)
                    {
                        query = @"INSERT INTO Usuarios 
                          (Nombre, Apellido, Correo, Contrasena, FechaRegistro, IdRol, Estado)
                          VALUES (@Nombre, @Apellido, @Correo, @Contrasena, GETDATE(), @IdRol, @Estado)";
                    }
                    else
                    {
                        // ✅ Edición existente (sí se enviará @Id correctamente)
                        query = @"UPDATE Usuarios SET 
                            Nombre = @Nombre, 
                            Apellido = @Apellido, 
                            Correo = @Correo,
                            Contrasena = @Contrasena, 
                            IdRol = @IdRol, 
                            Estado = @Estado 
                          WHERE Id = @Id";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                        cmd.Parameters.AddWithValue("@Correo", correo);
                        cmd.Parameters.AddWithValue("@Contrasena", txtContrasena.Text.Trim());
                        cmd.Parameters.AddWithValue("@IdRol", cmbRol.SelectedIndex + 1);
                        cmd.Parameters.AddWithValue("@Estado", chkActivo.Checked);

                        // ✅ Agregar parámetro solo si es edición
                        if (UsuarioId.HasValue && UsuarioId.Value != 0)
                            cmd.Parameters.AddWithValue("@Id", UsuarioId.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                FormNotificacion.Mostrar("Usuario guardado correctamente.", "exito");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                FormNotificacion.Mostrar("Error al guardar el usuario: " + ex.Message, "error");
            }
        }


        private async void MostrarNotificacion(string mensaje, Color color)
        {
            Label lblToast = new Label
            {
                Text = mensaje,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = color,
                Height = 35,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 8, 0, 0),
                Visible = true
            };

            this.Controls.Add(lblToast);
            lblToast.BringToFront();

            // Animación de entrada
            for (double op = 0; op <= 1; op += 0.1)
            {
                lblToast.BackColor = Color.FromArgb((int)(255 * op), color);
                await Task.Delay(15);
            }

            await Task.Delay(2000); // Mantener visible 2 segundos

            // Animación de salida
            for (double op = 1; op >= 0; op -= 0.1)
            {
                lblToast.BackColor = Color.FromArgb((int)(255 * op), color);
                await Task.Delay(15);
            }

            this.Controls.Remove(lblToast);
        }



        private void DibujarSombraSuave(Graphics g, Control panel)
        {
            Rectangle rect = new Rectangle(5, 5, panel.Width - 10, panel.Height - 10);
            using (GraphicsPath path = RoundedRect(rect, 25))
            using (PathGradientBrush pgb = new PathGradientBrush(path))
            {
                pgb.CenterColor = Color.FromArgb(60, 0, 0, 0);
                pgb.SurroundColors = new[] { Color.Transparent };
                g.FillPath(pgb, path);
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

        private void RedondearBordes(Control control, int radio)
        {
            control.Region = new Region(RoundedRect(new Rectangle(0, 0, control.Width, control.Height), radio));
        }



    }
}
