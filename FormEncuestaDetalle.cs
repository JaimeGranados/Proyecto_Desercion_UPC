using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormEncuestaDetalle : Form
    {
        private int? encuestaId;
        private string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        private TextBox txtTitulo;
        private TextBox txtDescripcion;
        private Button btnAgregarPregunta;
        private FlowLayoutPanel panelPreguntas;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormEncuestaDetalle()
        {
            InitializeComponent();
            InicializarInterfaz();
        }

        public FormEncuestaDetalle(int id) : this()
        {
            encuestaId = id;
            CargarDatosEncuesta(id);
        }

        private void InicializarInterfaz()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(820, 700);
            this.Text = encuestaId.HasValue ? "Editar encuesta" : "Crear encuesta";

            var main = new Panel { Dock = DockStyle.Fill, Padding = new Padding(18), BackColor = Color.FromArgb(38, 39, 43) };
            this.Controls.Add(main);

            Label lblTitulo = new Label { Text = "Título", ForeColor = Color.White, Font = new Font("Segoe UI", 9), AutoSize = true };
            txtTitulo = new TextBox { Width = 740, Font = new Font("Segoe UI", 10), BackColor = Color.FromArgb(48, 49, 54), ForeColor = Color.White };

            Label lblDesc = new Label { Text = "Descripción (opcional)", ForeColor = Color.White, Font = new Font("Segoe UI", 9), AutoSize = true, Top = 50 };
            txtDescripcion = new TextBox { Width = 740, Height = 70, Multiline = true, Font = new Font("Segoe UI", 10), BackColor = Color.FromArgb(48, 49, 54), ForeColor = Color.White };

            btnAgregarPregunta = new Button
            {
                Text = "+ Agregar pregunta",
                Width = 180,
                Height = 36,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAgregarPregunta.FlatAppearance.BorderSize = 0;
            btnAgregarPregunta.Click += (s, e) => AgregarPreguntaPanel();

            panelPreguntas = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 420,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(0, 150, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(70, 70, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            var topFlow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 140, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            topFlow.Controls.Add(lblTitulo);
            topFlow.Controls.Add(txtTitulo);
            topFlow.Controls.Add(lblDesc);
            topFlow.Controls.Add(txtDescripcion);

            var btnRow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, FlowDirection = FlowDirection.LeftToRight };
            btnRow.Controls.Add(btnAgregarPregunta);
            btnRow.Padding = new Padding(0, 8, 0, 8);

            var bottomRow = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
            bottomRow.Controls.Add(btnGuardar);
            bottomRow.Controls.Add(btnCancelar);

            main.Controls.Add(panelPreguntas);
            main.Controls.Add(bottomRow);
            main.Controls.Add(btnRow);
            main.Controls.Add(topFlow);
        }

        private void AgregarPreguntaPanel(string textoPregunta = "", string tipo = "Texto", IEnumerable<string> opciones = null)
        {
            Panel p = new Panel
            {
                Width = panelPreguntas.ClientSize.Width - 25,
                Height = 120,
                BackColor = Color.FromArgb(44, 47, 51),
                Margin = new Padding(6)
            };

            TextBox txtPregunta = new TextBox
            {
                Width = p.Width - 180,
                Text = textoPregunta,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(48, 49, 54),
                ForeColor = Color.White,
                Location = new Point(10, 10)
            };

            ComboBox cmbTipo = new ComboBox
            {
                Width = 140,
                Location = new Point(p.Width - 160, 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbTipo.Items.AddRange(new object[] { "Texto", "Opcion", "Escala" });
            cmbTipo.SelectedItem = tipo;

            FlowLayoutPanel panelOpciones = new FlowLayoutPanel
            {
                Location = new Point(10, 40),
                Width = p.Width - 20,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                WrapContents = false,
                Visible = (tipo == "Opcion")
            };

            Button btnAddOpt = new Button
            {
                Text = "+",
                Width = 30,
                Height = 26,
                Location = new Point(p.Width - 200, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddOpt.FlatAppearance.BorderSize = 0;
            btnAddOpt.Click += (s, e) => AgregarOpcionALabel(panelOpciones, "");

            if (opciones != null)
            {
                foreach (string opt in opciones)
                    AgregarOpcionALabel(panelOpciones, opt);
            }

            Button btnEliminar = new Button
            {
                Text = "Eliminar",
                Width = 80,
                Height = 28,
                Location = new Point(p.Width - 90, 40),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += (s, e) => { panelPreguntas.Controls.Remove(p); p.Dispose(); };

            cmbTipo.SelectedIndexChanged += (s, e) =>
            {
                panelOpciones.Visible = (cmbTipo.SelectedItem.ToString() == "Opcion");
            };

            p.Controls.Add(txtPregunta);
            p.Controls.Add(cmbTipo);
            p.Controls.Add(panelOpciones);
            p.Controls.Add(btnAddOpt);
            p.Controls.Add(btnEliminar);

            panelPreguntas.Controls.Add(p);
        }

        private void AgregarOpcionALabel(FlowLayoutPanel panel, string texto)
        {
            Panel optPanel = new Panel
            {
                Width = 260,
                Height = 30,
                Margin = new Padding(4),
                BackColor = Color.FromArgb(60, 63, 69)
            };

            TextBox txtOpt = new TextBox
            {
                Text = texto,
                Width = 200,
                Height = 24,
                Location = new Point(4, 3),
                BackColor = Color.FromArgb(48, 49, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            Button btnRem = new Button
            {
                Text = "x",
                Width = 30,
                Height = 26,
                Location = new Point(208, 2),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRem.FlatAppearance.BorderSize = 0;
            btnRem.Click += (s, e) => { panel.Controls.Remove(optPanel); optPanel.Dispose(); };

            optPanel.Controls.Add(txtOpt);
            optPanel.Controls.Add(btnRem);
            panel.Controls.Add(optPanel);
        }

        private string TipoANombre(int tipo)
        {
            if (tipo == 1) return "Opcion";
            if (tipo == 2) return "Texto";
            if (tipo == 3) return "Escala";
            return "Texto";
        }

        private int NombreATipo(string name)
        {
            if (name == "Opcion") return 1;
            if (name == "Texto") return 2;
            if (name == "Escala") return 3;
            return 2;
        }

        private void CargarDatosEncuesta(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Titulo, Descripcion FROM Encuestas WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitulo.Text = reader["Titulo"].ToString();
                            txtDescripcion.Text = reader["Descripcion"] == DBNull.Value ? "" : reader["Descripcion"].ToString();
                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT P.Id, P.Texto, P.Tipo, O.TextoOpcion
                      FROM Preguntas P
                      LEFT JOIN Opciones O ON O.IdPregunta = P.Id
                      WHERE P.IdEncuesta = @Id
                      ORDER BY P.Id, O.Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        int currentPreguntaId = -1;
                        string textoPregunta = "";
                        int tipoPregunta = 2;
                        List<string> opciones = new List<string>();

                        while (reader.Read())
                        {
                            int idPregunta = Convert.ToInt32(reader["Id"]);
                            string texto = reader["Texto"].ToString();
                            int tipo = Convert.ToInt32(reader["Tipo"]);
                            string textoOpcion = reader["TextoOpcion"] == DBNull.Value ? null : reader["TextoOpcion"].ToString();

                            if (idPregunta != currentPreguntaId)
                            {
                                if (currentPreguntaId != -1)
                                    AgregarPreguntaPanel(textoPregunta, TipoANombre(tipoPregunta), opciones);

                                currentPreguntaId = idPregunta;
                                textoPregunta = texto;
                                tipoPregunta = tipo;
                                opciones = new List<string>();
                                if (!string.IsNullOrEmpty(textoOpcion))
                                    opciones.Add(textoOpcion);
                            }
                            else if (!string.IsNullOrEmpty(textoOpcion))
                                opciones.Add(textoOpcion);
                        }

                        if (currentPreguntaId != -1)
                            AgregarPreguntaPanel(textoPregunta, TipoANombre(tipoPregunta), opciones);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar encuesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var preguntas = new List<Tuple<string, int, List<string>>>();

            foreach (Control c in panelPreguntas.Controls)
            {
                if (c is Panel pnl)
                {
                    TextBox txt = pnl.Controls.OfType<TextBox>().FirstOrDefault();
                    ComboBox cmb = pnl.Controls.OfType<ComboBox>().FirstOrDefault();
                    FlowLayoutPanel panelOpt = pnl.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();

                    if (txt == null || cmb == null) continue;

                    string textoP = txt.Text.Trim();
                    if (string.IsNullOrEmpty(textoP))
                    {
                        MessageBox.Show("Cada pregunta debe tener texto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string tipoName = cmb.SelectedItem.ToString();
                    int tipo = NombreATipo(tipoName);
                    List<string> opts = new List<string>();

                    if (tipo == 1 && panelOpt != null)
                    {
                        foreach (Control oc in panelOpt.Controls)
                        {
                            TextBox t = oc.Controls.OfType<TextBox>().FirstOrDefault();
                            if (t != null && !string.IsNullOrWhiteSpace(t.Text))
                                opts.Add(t.Text.Trim());
                        }

                        if (opts.Count == 0)
                        {
                            MessageBox.Show("Agrega al menos una opción a las preguntas tipo 'Opcion'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    preguntas.Add(new Tuple<string, int, List<string>>(textoP, tipo, opts));
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        int idEncuesta;

                        if (!encuestaId.HasValue || encuestaId.Value == 0)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO Encuestas (Titulo, Descripcion, FechaCreacion, Activo) VALUES (@T, @D, GETDATE(), 1); SELECT SCOPE_IDENTITY();",
                                conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@T", txtTitulo.Text.Trim());
                                cmd.Parameters.AddWithValue("@D", string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                                idEncuesta = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else
                        {
                            idEncuesta = encuestaId.Value;
                            using (SqlCommand cmd = new SqlCommand("UPDATE Encuestas SET Titulo=@T, Descripcion=@D WHERE Id=@Id", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@T", txtTitulo.Text.Trim());
                                cmd.Parameters.AddWithValue("@D", string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                                cmd.Parameters.AddWithValue("@Id", idEncuesta);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand("DELETE FROM Opciones WHERE IdPregunta IN (SELECT Id FROM Preguntas WHERE IdEncuesta=@Id)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@Id", idEncuesta);
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("DELETE FROM Preguntas WHERE IdEncuesta=@Id", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@Id", idEncuesta);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        foreach (var p in preguntas)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO Preguntas (IdEncuesta, Texto, Tipo) VALUES (@IdE, @T, @Tipo); SELECT SCOPE_IDENTITY();", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@IdE", idEncuesta);
                                cmd.Parameters.AddWithValue("@T", p.Item1);
                                cmd.Parameters.AddWithValue("@Tipo", p.Item2);
                                int idPregunta = Convert.ToInt32(cmd.ExecuteScalar());

                                if (p.Item2 == 1)
                                {
                                    foreach (string opt in p.Item3)
                                    {
                                        using (SqlCommand cmdOpt = new SqlCommand("INSERT INTO Opciones (IdPregunta, TextoOpcion) VALUES (@IdP, @TO)", conn, tx))
                                        {
                                            cmdOpt.Parameters.AddWithValue("@IdP", idPregunta);
                                            cmdOpt.Parameters.AddWithValue("@TO", opt);
                                            cmdOpt.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Encuesta guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar encuesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
