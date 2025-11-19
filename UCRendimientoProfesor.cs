using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCRendimientoProfesor : UserControl
    {
        private readonly string connectionString =
            "Server=localhost;Database=ProyectoVisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        private FlowLayoutPanel panelPrincipal;

        public UCRendimientoProfesor()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 242, 246);
            InicializarUI();
            CargarDashboard();
        }

        private void InicializarUI()
        {
            this.Controls.Clear();

            // HEADER
            Panel header = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = this.BackColor
            };

            Label lblTitulo = new Label()
            {
                Text = "📊 Rendimiento académico",
                Font = new Font("Segoe UI Semibold", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true
            };

            header.Resize += (s, e) =>
            {
                lblTitulo.Left = Math.Max(0, (header.Width - lblTitulo.Width) / 2);
                lblTitulo.Top = Math.Max(0, (header.Height - lblTitulo.Height) / 2);
            };

            header.Controls.Add(lblTitulo);
            this.Controls.Add(header);

            // CONTENEDOR PRINCIPAL
            panelPrincipal = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(40),
                BackColor = Color.Transparent
            };

            panelPrincipal.Resize += (s, e) => CentrarTarjetas();

            this.Controls.Add(panelPrincipal);
        }

        private void CargarDashboard()
        {
            panelPrincipal.Controls.Clear();

            int profesorId = ConexionGlobal.UsuarioActual?.Id ?? 0;
            if (profesorId == 0)
            {
                panelPrincipal.Controls.Add(CrearLabelInfo("No se detectó un profesor válido."));
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1) cursos del profesor
                List<CursoInfo> cursos = ObtenerCursos(conn, profesorId);
                if (cursos.Count == 0)
                {
                    panelPrincipal.Controls.Add(CrearLabelInfo("No tienes cursos asignados."));
                    return;
                }

                // 2) estudiantes por curso con estadística
                List<EstadisticaCurso> estadisticas = GenerarEstadisticas(conn, cursos);

                // 3) tarjeta general
                panelPrincipal.Controls.Add(CrearTarjetaGeneral(estadisticas));

                // 4) tarjetas por curso
                foreach (var est in estadisticas)
                    panelPrincipal.Controls.Add(CrearTarjetaCurso(est));
            }

            CentrarTarjetas();
        }

        private List<CursoInfo> ObtenerCursos(SqlConnection conn, int profesorId)
        {
            var lista = new List<CursoInfo>();

            using (var cmd = new SqlCommand(
                "SELECT Id, Nombre FROM Cursos WHERE IdProfesor=@p AND Activo=1", conn))
            {
                cmd.Parameters.AddWithValue("@p", profesorId);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new CursoInfo
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Nombre = r["Nombre"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        private List<EstadisticaCurso> GenerarEstadisticas(SqlConnection conn, List<CursoInfo> cursos)
        {
            var lista = new List<EstadisticaCurso>();

            foreach (var curso in cursos)
            {

                List<int> estudiantes = new List<int>();

                using (var cmd = new SqlCommand(
                    "SELECT IdEstudiante FROM Matriculas WHERE IdCurso=@c", conn))
                {
                    cmd.Parameters.AddWithValue("@c", curso.Id);

                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            estudiantes.Add(Convert.ToInt32(r["IdEstudiante"]));
                }

                if (estudiantes.Count == 0)
                {
                    lista.Add(new EstadisticaCurso
                    {
                        Curso = curso,
                        Asistencia = 0,
                        Participacion = 0,
                        EnRiesgo = 0
                    });
                    continue;
                }

                DateTime desde = DateTime.Now.AddDays(-30);
                int totalAsist = 0, presentes = 0;

                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*) AS total,
                   SUM(CASE WHEN Asistio = 1 THEN 1 ELSE 0 END) AS pres
            FROM Asistencias a
            INNER JOIN Matriculas m ON m.Id = a.IdMatricula
            WHERE m.IdCurso=@c AND a.Fecha>=@d", conn))
                {
                    cmd.Parameters.AddWithValue("@c", curso.Id);
                    cmd.Parameters.AddWithValue("@d", desde);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            totalAsist = r["total"] == DBNull.Value ? 0 : Convert.ToInt32(r["total"]);
                            presentes = r["pres"] == DBNull.Value ? 0 : Convert.ToInt32(r["pres"]);
                        }
                    }
                }

                double porcAsistenciaCurso =
                    totalAsist > 0 ? (double)presentes / totalAsist : 1.0;

                int respuestasTotales = 0;
                int encuestasActivas = 1;

                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*)
            FROM Respuestas r
            INNER JOIN Matriculas m ON m.IdEstudiante = r.IdUsuario
            WHERE m.IdCurso=@c", conn))
                {
                    cmd.Parameters.AddWithValue("@c", curso.Id);
                    respuestasTotales = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Encuestas WHERE Activo=1", conn))
                {
                    encuestasActivas = Convert.ToInt32(cmd.ExecuteScalar());
                    if (encuestasActivas <= 0) encuestasActivas = 1;
                }

                double porcParticipacionCurso =
                    (double)respuestasTotales / (estudiantes.Count * encuestasActivas);

                int riesgo = 0;

                foreach (int idEst in estudiantes)
                {
                    // --- Asistencia individual ---
                    int totEst = 0, preEst = 0;

                    using (var cmd = new SqlCommand(@"
                SELECT COUNT(*) AS total,
                       SUM(CASE WHEN Asistio = 1 THEN 1 ELSE 0 END) AS pres
                FROM Asistencias a
                INNER JOIN Matriculas m ON m.Id=a.IdMatricula
                WHERE m.IdCurso=@c AND m.IdEstudiante=@e AND a.Fecha>=@d", conn))
                    {
                        cmd.Parameters.AddWithValue("@c", curso.Id);
                        cmd.Parameters.AddWithValue("@e", idEst);
                        cmd.Parameters.AddWithValue("@d", desde);

                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                totEst = r["total"] == DBNull.Value ? 0 : Convert.ToInt32(r["total"]);
                                preEst = r["pres"] == DBNull.Value ? 0 : Convert.ToInt32(r["pres"]);
                            }
                        }
                    }

                    double porcAsistenciaEst =
                        totEst > 0 ? (double)preEst / totEst : 1.0;

                    // --- Participación individual ---
                    int respuestasEst = 0;

                    using (var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Respuestas r
                WHERE r.IdUsuario=@e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", idEst);
                        respuestasEst = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    double porcParticipacionEst =
                        (double)respuestasEst / encuestasActivas;

                    // --- Modelo 70% asistencia / 30% participación ---
                    double scoreFinal =
                        (porcAsistenciaEst * 0.70) +
                        (porcParticipacionEst * 0.30);

                    // Bajo 60% = riesgo
                    if (scoreFinal < 0.60)
                        riesgo++;
                }

       
                lista.Add(new EstadisticaCurso
                {
                    Curso = curso,
                    Asistencia = Math.Round(porcAsistenciaCurso * 100),
                    Participacion = Math.Round(porcParticipacionCurso * 100),
                    EnRiesgo = riesgo
                });
            }

            return lista;
        }


        private Panel CrearTarjetaGeneral(List<EstadisticaCurso> lst)
        {
            double asistencia = 0;
            double participacion = 0;
            int riesgo = 0;

            if (lst != null && lst.Count > 0)
            {
                asistencia = lst.Average(x => x.Asistencia);
                participacion = lst.Average(x => x.Participacion);
                riesgo = lst.Sum(x => x.EnRiesgo);
            }

            return CrearTarjeta(
                "📘 Rendimiento General",
                $"Asistencia promedio: {asistencia:0}%\n" +
                $"Participación promedio: {participacion:0}%\n" +
                $"Estudiantes en riesgo: {riesgo}",
                Color.FromArgb(0, 120, 215)
            );
        }


        private Panel CrearTarjetaCurso(EstadisticaCurso est)
        {
            return CrearTarjeta(
                $"📚 {est.Curso.Nombre}",
                $"Asistencia: {est.Asistencia}%\n" +
                $"Participación: {est.Participacion}%\n" +
                $"Riesgo: {est.EnRiesgo}",
                Color.FromArgb(255, 143, 50)
            );
        }

        private Panel CrearTarjeta(string titulo, string descripcion, Color colorBarra)
        {
            int w = 850;
            Panel card = new Panel()
            {
                Width = w,
                Height = 160,
                BackColor = Color.White,
                Padding = new Padding(20),
                Margin = new Padding(0, 20, 0, 20)
            };

            int radius = 18;
            card.Paint += (s, e) =>
            {
                card.Region = Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, card.Width, card.Height, radius, radius));
            };

            Label lblTitle = new Label()
            {
                Text = titulo,
                Font = new Font("Segoe UI Semibold", 16),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(20, 18)
            };

            Label lblDesc = new Label()
            {
                Text = descripcion,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new Point(20, 60)
            };

            Panel bar = new Panel()
            {
                Width = 6,
                Height = card.Height - 40,
                BackColor = colorBarra,
                Location = new Point(card.Width - 30, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDesc);
            card.Controls.Add(bar);

            return card;
        }

        private void CentrarTarjetas()
        {
            foreach (Control c in panelPrincipal.Controls)
            {
                int left = Math.Max(0, (panelPrincipal.ClientSize.Width - c.Width) / 2);
                c.Margin = new Padding(left, 20, 0, 20);
            }
        }

        private Label CrearLabelInfo(string txt)
        {
            return new Label()
            {
                Text = txt,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Margin = new Padding(15)
            };
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeft, int nTop, int nRight, int nBottom,
            int width, int height);

        // CLASES AUXILIARES
        private class CursoInfo
        {
            public int Id;
            public string Nombre;
        }

        private class EstadisticaCurso
        {
            public CursoInfo Curso;
            public double Asistencia;
            public double Participacion;
            public int EnRiesgo;
        }
    }
}
