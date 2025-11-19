using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormResponderEncuesta : Form
    {
        private readonly int idEncuesta;
        private readonly int idEstudiante;
        private string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";
        private FlowLayoutPanel panelPreguntas;
        private Button btnGuardar;

        // Estructura temporal para las preguntas
        private class PreguntaInfo
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public Control ControlRespuesta { get; set; }
        }

        private List<PreguntaInfo> preguntas = new List<PreguntaInfo>();

        public FormResponderEncuesta(int idEncuesta, int idEstudiante)
        {
            InitializeComponent();
            this.idEncuesta = idEncuesta;
            this.idEstudiante = idEstudiante;
            CrearInterfaz();
            CargarEncuesta();
        }

        private void CrearInterfaz()
        {
            this.Text = "Responder encuesta";
            this.Size = new Size(750, 750);
            this.BackColor = Color.FromArgb(38, 39, 43);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTitulo = new Label
            {
                Text = "Responde las siguientes preguntas",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            this.Controls.Add(lblTitulo);

            panelPreguntas = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Location = new Point(25, 60),
                Width = 690,
                Height = 580,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            this.Controls.Add(panelPreguntas);

            btnGuardar = new Button
            {
                Text = "Guardar respuestas",
                Width = 220,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 660)
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void CargarEncuesta()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 🧩 Cargar título
                    using (SqlCommand cmd = new SqlCommand("SELECT Titulo, Descripcion FROM Encuestas WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", idEncuesta);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Label lblTitulo = new Label
                                {
                                    Text = reader["Titulo"].ToString(),
                                    ForeColor = Color.White,
                                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                                    AutoSize = true,
                                    Margin = new Padding(10, 10, 10, 10)
                                };
                                panelPreguntas.Controls.Add(lblTitulo);

                                if (!Convert.IsDBNull(reader["Descripcion"]))
                                {
                                    Label lblDesc = new Label
                                    {
                                        Text = reader["Descripcion"].ToString(),
                                        ForeColor = Color.LightGray,
                                        Font = new Font("Segoe UI", 10, FontStyle.Italic),
                                        AutoSize = true,
                                        Margin = new Padding(10, 0, 10, 15)
                                    };
                                    panelPreguntas.Controls.Add(lblDesc);
                                }
                            }
                        }
                    }

                    // 🧠 Cargar preguntas
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT P.Id, P.Texto, P.Tipo, O.TextoOpcion
                          FROM Preguntas P
                          LEFT JOIN Opciones O ON O.IdPregunta = P.Id
                          WHERE P.IdEncuesta = @Id
                          ORDER BY P.Id, O.Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", idEncuesta);
                        using (var reader = cmd.ExecuteReader())
                        {
                            int idActual = -1;
                            string texto = "";
                            string tipo = "";
                            List<string> opciones = new List<string>();

                            while (reader.Read())
                            {
                                int idPregunta = Convert.ToInt32(reader["Id"]);
                                string textoOpcion = reader["TextoOpcion"] == DBNull.Value ? null : reader["TextoOpcion"].ToString();

                                if (idPregunta != idActual && idActual != -1)
                                {
                                    CrearPregunta(idActual, texto, tipo, opciones);
                                    opciones.Clear();
                                }

                                idActual = idPregunta;
                                texto = reader["Texto"].ToString();
                                tipo = TipoANombre(Convert.ToInt32(reader["Tipo"]));
                                if (!string.IsNullOrEmpty(textoOpcion))
                                    opciones.Add(textoOpcion);
                            }

                            if (idActual != -1)
                                CrearPregunta(idActual, texto, tipo, opciones);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar encuesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearPregunta(int id, string texto, string tipo, List<string> opciones)
        {
            Label lbl = new Label
            {
                Text = texto,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                AutoSize = true,
                Margin = new Padding(10, 10, 10, 5)
            };
            panelPreguntas.Controls.Add(lbl);

            Control respuestaControl = null;

            switch (tipo)
            {
                case "Texto":
                    respuestaControl = new TextBox
                    {
                        Width = 600,
                        Height = 30,
                        BackColor = Color.FromArgb(55, 56, 60),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10)
                    };
                    break;

                case "Opcion":
                    var group = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        Width = 600,
                        AutoSize = true
                    };
                    foreach (var op in opciones)
                    {
                        RadioButton rb = new RadioButton
                        {
                            Text = op,
                            ForeColor = Color.White,
                            Font = new Font("Segoe UI", 10),
                            AutoSize = true
                        };
                        group.Controls.Add(rb);
                    }
                    respuestaControl = group;
                    break;

                case "Escala":
                    var panelEscala = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        Width = 600,
                        AutoSize = true
                    };
                    for (int i = 1; i <= 5; i++)
                    {
                        RadioButton rb = new RadioButton
                        {
                            Text = i.ToString(),
                            ForeColor = Color.White,
                            Font = new Font("Segoe UI", 10),
                            AutoSize = true
                        };
                        panelEscala.Controls.Add(rb);
                    }
                    respuestaControl = panelEscala;
                    break;
            }

            if (respuestaControl != null)
            {
                respuestaControl.Margin = new Padding(10, 0, 10, 15);
                panelPreguntas.Controls.Add(respuestaControl);
                preguntas.Add(new PreguntaInfo { Id = id, Tipo = tipo, ControlRespuesta = respuestaControl });
            }
        }

        private string TipoANombre(int tipo)
        {
            if (tipo == 1) return "Opcion";
            if (tipo == 3) return "Escala";
            return "Texto";
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (var p in preguntas)
                    {
                        string respuesta = "";

                        if (p.Tipo == "Texto" && p.ControlRespuesta is TextBox txt)
                            respuesta = txt.Text.Trim();

                        else if (p.Tipo == "Opcion" && p.ControlRespuesta is FlowLayoutPanel flp)
                        {
                            var selected = flp.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                            if (selected != null) respuesta = selected.Text;
                        }

                        else if (p.Tipo == "Escala" && p.ControlRespuesta is FlowLayoutPanel flp2)
                        {
                            var selected = flp2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                            if (selected != null) respuesta = selected.Text;
                        }

                        if (!string.IsNullOrEmpty(respuesta))
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO Respuestas (IdPregunta, IdEstudiante, RespuestaTexto, FechaRespuesta) VALUES (@P, @E, @R, GETDATE())", conn))
                            {
                                cmd.Parameters.AddWithValue("@P", p.Id);
                                cmd.Parameters.AddWithValue("@E", idEstudiante);
                                cmd.Parameters.AddWithValue("@R", respuesta);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("¡Respuestas guardadas exitosamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar respuestas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
