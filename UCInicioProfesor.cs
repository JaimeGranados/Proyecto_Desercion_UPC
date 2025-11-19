using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCInicioProfesor : UserControl
    {
        private readonly string connectionString =
            "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        public UCInicioProfesor()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 242, 246);
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            this.Controls.Clear();

            Panel main = new Panel { Dock = DockStyle.Fill, BackColor = this.BackColor };
            this.Controls.Add(main);

            FlowLayoutPanel centrado = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 10)
            };

            main.Resize += (s, e) =>
            {
                centrado.Left = Math.Max(0, (main.Width - centrado.Width) / 2);
                centrado.Top = Math.Max(0, (main.Height - centrado.Height) / 6);
            };

            main.Controls.Add(centrado);

            Label lblBienvenida = new Label
            {
                Text = "Bienvenido, " + (ConexionGlobal.UsuarioActual?.Nombre ?? "Profesor"),
                Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true
            };
            centrado.Controls.Add(lblBienvenida);

            centrado.Controls.Add(new Label
            {
                Text = DateTime.Now.ToString("dddd dd MMMM yyyy").ToUpper(),
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 25)
            });

            // MÉTRICAS
            int profesorId = ConexionGlobal.UsuarioActual?.Id ?? 0;

            int cursosAsignados = SafeIntQuery(
                "SELECT COUNT(*) FROM Cursos WHERE IdProfesor = @p AND Activo = 1",
                new SqlParameter("@p", profesorId));

            int totalEstudiantes = SafeIntQuery(
                "SELECT COUNT(*) FROM Usuarios WHERE IdRol = 3 AND Estado = 1");

            int encuestasPendientes = SafeIntQuery(
                "SELECT COUNT(*) FROM Encuestas WHERE Activo = 1");

            int estudiantesRiesgo = CalcularRiesgo(profesorId);

            // TARJETAS
            centrado.Controls.Add(CrearCard("📘 Cursos asignados", $"{cursosAsignados} cursos activos"));
            centrado.Controls.Add(CrearCard("👥 Total estudiantes", $"{totalEstudiantes} estudiantes"));
            centrado.Controls.Add(CrearCard("⚠️ Alertas recientes", $"{estudiantesRiesgo} estudiantes en riesgo"));
            centrado.Controls.Add(CrearCard("📝 Encuestas pendientes", $"{encuestasPendientes} por responder"));
        }

        // ========================================================
        //    MODELO B — Cálculo de riesgo (correcto a tu BD)
        // ========================================================
        private int CalcularRiesgo(int profesorId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✔ 1) Cursos del profesor
                    var cursos = new List<int>();
                    using (var cmd = new SqlCommand("SELECT Id FROM Cursos WHERE IdProfesor = @p", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", profesorId);
                        using (var r = cmd.ExecuteReader())
                            while (r.Read()) cursos.Add((int)r["Id"]);
                    }
                    if (cursos.Count == 0) return 0;

                    // ✔ 2) Estudiantes matriculados en esos cursos
                    var estudiantes = new List<int>();
                    using (var cmd = new SqlCommand(
                        $"SELECT DISTINCT IdEstudiante FROM Matriculas WHERE IdCurso IN ({string.Join(",", cursos)})", conn))
                    {
                        using (var r = cmd.ExecuteReader())
                            while (r.Read()) estudiantes.Add((int)r["IdEstudiante"]);
                    }
                    if (estudiantes.Count == 0) return 0;

                    // ✔ 3) Asistencias (usando IdMatricula porque tu tabla NO tiene IdEstudiante)
                    DateTime desde = DateTime.Now.AddDays(-30);

                    var asistencias = new Dictionary<int, (int total, int pres)>();

                    using (var cmd = new SqlCommand(
                        $@"SELECT m.IdEstudiante,
                                 COUNT(*) total,
                                 SUM(CASE WHEN a.Asistio = 1 THEN 1 ELSE 0 END) pres
                           FROM Asistencias a
                           INNER JOIN Matriculas m ON m.Id = a.IdMatricula
                           WHERE a.Fecha >= @d
                             AND m.IdEstudiante IN ({string.Join(",", estudiantes)})
                           GROUP BY m.IdEstudiante", conn))
                    {
                        cmd.Parameters.AddWithValue("@d", desde);

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                asistencias[(int)r["IdEstudiante"]] =
                                    ((int)r["total"], (int)r["pres"]);
                            }
                        }
                    }

                    // ✔ 4) Respuestas (IdUsuario = estudiante)
                    int encuestasActivas = SafeIntQuery("SELECT COUNT(*) FROM Encuestas WHERE Activo = 1");

                    var respuestas = new Dictionary<int, int>();
                    using (var cmd = new SqlCommand(
                        @"SELECT IdUsuario AS IdEstudiante, COUNT(*) cnt
                          FROM Respuestas
                          GROUP BY IdUsuario", conn))
                    {
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                                respuestas[(int)r["IdEstudiante"]] = (int)r["cnt"];
                    }

                    // ✔ 5) Evaluación de riesgo
                    int riesgo = 0;
                    foreach (int est in estudiantes)
                    {
                        double porcAsist = 1.0;

                        if (asistencias.ContainsKey(est))
                        {
                            var a = asistencias[est];
                            if (a.total > 0)
                                porcAsist = (double)a.pres / a.total;
                        }

                        bool riesgoAsist = porcAsist < 0.60;

                        bool riesgoResp = false;
                        if (encuestasActivas > 0)
                        {
                            int resp = respuestas.ContainsKey(est) ? respuestas[est] : 0;
                            riesgoResp = ((double)resp / encuestasActivas) < 0.50;
                        }

                        if (riesgoAsist || riesgoResp)
                            riesgo++;
                    }

                    return riesgo;
                }
            }
            catch
            {
                return 0;
            }
        }

        private int SafeIntQuery(string sql, params SqlParameter[] p)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (p != null) cmd.Parameters.AddRange(p);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }

        // ========================================================
        // TARJETA VISUAL
        // ========================================================
        private Panel CrearCard(string titulo, string desc)
        {
            Panel card = new Panel
            {
                Size = new Size(420, 120),
                BackColor = Color.White,
                Margin = new Padding(0, 8, 0, 8)
            };

            int r = 20;
            card.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, card.Width, card.Height, r, r));

            card.Controls.Add(new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI Semibold", 15),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(20, 18)
            });

            card.Controls.Add(new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                Location = new Point(22, 60)
            });

            return card;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);
    }
}
