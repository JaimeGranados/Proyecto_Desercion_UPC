using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class UCEstudiantesProfesor : UserControl
    {
        // Ajusta si tu conexión es diferente (usa la misma que el resto del proyecto)
        private readonly string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

        private FlowLayoutPanel pnlContenedor;
        private TextBox txtBuscar;
        private Label lblPlaceholderBuscar;

        public UCEstudiantesProfesor()
        {
            InitializeComponent(); // si el diseñador lo requiere
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 242, 246);
            InicializarUI();
            CargarEstudiantes();
        }

        private void InicializarUI()
        {
            this.Controls.Clear();

            // Contenedor principal
            Panel main = new Panel { Dock = DockStyle.Fill, BackColor = this.BackColor };
            this.Controls.Add(main);

            // Header (encabezado)
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = this.BackColor
            };

            // Título centrado
            Label lblTitulo = new Label
            {
                Text = "👨‍🎓 Estudiantes asociados",
                Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true
            };
            // colocar título centrado dentro del header
            lblTitulo.Location = new Point((header.Width - lblTitulo.Width) / 2, (header.Height - lblTitulo.Height) / 2);
            header.Controls.Add(lblTitulo);
            // reajustar posición al redimensionar header
            header.Resize += (s, e) =>
            {
                lblTitulo.Left = Math.Max(16, (header.ClientSize.Width - lblTitulo.Width) / 2);
                lblTitulo.Top = Math.Max(8, (header.ClientSize.Height - lblTitulo.Height) / 2);
            };

            // Caja de búsqueda a la derecha del header (alineada)
            Panel searchHolder = new Panel
            {
                Width = 360,
                Height = 40,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            // posicionar al lado derecho con margen
            searchHolder.Location = new Point(header.ClientSize.Width - searchHolder.Width - 24, (header.Height - searchHolder.Height) / 2);
            header.Controls.Add(searchHolder);
            header.Resize += (s, e) =>
            {
                searchHolder.Left = Math.Max(16, header.ClientSize.Width - searchHolder.Width - 24);
            };

            // TextBox buscar
            txtBuscar = new TextBox
            {
                Width = searchHolder.Width - 16,
                Height = 30,
                Location = new Point(8, 5),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(255, 255, 255),
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Placeholder label sobre el textbox (mejor control visual)
            lblPlaceholderBuscar = new Label
            {
                Text = "Buscar estudiante...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Location = txtBuscar.Location,
                Size = txtBuscar.Size
            };

            // Eventos para placeholder manual
            txtBuscar.GotFocus += (s, e) =>
            {
                lblPlaceholderBuscar.Visible = false;
            };
            txtBuscar.LostFocus += (s, e) =>
            {
                lblPlaceholderBuscar.Visible = string.IsNullOrWhiteSpace(txtBuscar.Text);
            };
            // Si hacen click en el placeholder, foco al textbox
            lblPlaceholderBuscar.Click += (s, e) => txtBuscar.Focus();

            // Búsqueda: filtra las tarjetas (por nombre)
            txtBuscar.TextChanged += (s, e) =>
            {
                if (lblPlaceholderBuscar.Visible) return; // evitar filtrar si placeholder visible
                FiltrarTarjetas(txtBuscar.Text.Trim());
            };

            searchHolder.Controls.Add(txtBuscar);
            searchHolder.Controls.Add(lblPlaceholderBuscar);

            main.Controls.Add(header);

            // Contenedor centrado con scroll vertical (debajo del header)
            pnlContenedor = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                AutoScroll = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(40),
                BackColor = Color.Transparent
            };

            // Al redimensionar, reajustamos márgenes para centrar los cards
            pnlContenedor.Resize += (s, e) => CenterCardsHorizontally();

            main.Controls.Add(pnlContenedor);
        }

        // Filtra tarjetas según texto (busca en nombre y apellido)
        private void FiltrarTarjetas(string texto)
        {
            try
            {
                texto = (texto ?? "").ToLowerInvariant();
                foreach (Control c in pnlContenedor.Controls)
                {
                    if (!(c is Panel)) continue;
                    // el primer control es el label de nombre en la estructura que usamos
                    var lblName = c.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Style.HasFlag(FontStyle.Bold));
                    if (lblName == null)
                    {
                        c.Visible = true;
                        continue;
                    }
                    string name = lblName.Text.ToLowerInvariant();
                    c.Visible = string.IsNullOrEmpty(texto) || name.Contains(texto);
                }
            }
            catch
            {
                // no bloquear UI
            }
        }

        private void CargarEstudiantes()
        {
            pnlContenedor.SuspendLayout();
            pnlContenedor.Controls.Clear();

            try
            {
                // 1) Obtener cursos del profesor actual
                int profesorId = ConexionGlobal.UsuarioActual?.Id ?? 0;
                if (profesorId == 0)
                {
                    pnlContenedor.Controls.Add(CrearInfoLabel("No se ha detectado profesor logueado."));
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Obtener lista de ids de cursos del profesor
                    var cursosProfesor = new List<int>();
                    using (var cmd = new SqlCommand("SELECT Id FROM Cursos WHERE IdProfesor = @p AND Activo = 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", profesorId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read()) cursosProfesor.Add(Convert.ToInt32(r["Id"]));
                        }
                    }

                    if (cursosProfesor.Count == 0)
                    {
                        pnlContenedor.Controls.Add(CrearInfoLabel("No hay cursos asignados."));
                        return;
                    }

                    // 2) Obtener estudiantes matriculados en esos cursos (distinct)
                    var estudiantes = new List<EstudianteInfo>();
                    string cursosIn = string.Join(",", cursosProfesor.Select(x => x.ToString()));

                    // Protección: si por alguna razón la lista estuviera vacía, evitamos el SQL inválido
                    if (string.IsNullOrWhiteSpace(cursosIn))
                    {
                        pnlContenedor.Controls.Add(CrearInfoLabel("No hay cursos válidos."));
                        return;
                    }

                    using (var cmd = new SqlCommand($@"
                        SELECT DISTINCT u.Id AS IdUsuario, u.Nombre, u.Apellido, m.Id AS IdMatricula, m.IdCurso, c.Nombre AS CursoNombre, m.FechaMatricula
                        FROM Matriculas m
                        INNER JOIN Usuarios u ON u.Id = m.IdEstudiante
                        LEFT JOIN Cursos c ON c.Id = m.IdCurso
                        WHERE m.IdCurso IN ({cursosIn})", conn))
                    {
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                int idUsuario = Convert.ToInt32(r["IdUsuario"]);
                                // Buscar si ya existe el estudiante para agregar curso a su lista
                                var est = estudiantes.FirstOrDefault(x => x.IdUsuario == idUsuario);
                                if (est == null)
                                {
                                    est = new EstudianteInfo
                                    {
                                        IdUsuario = idUsuario,
                                        Nombre = r["Nombre"]?.ToString(),
                                        Apellido = r["Apellido"]?.ToString(),
                                        Cursos = new List<CursoMat>(),
                                    };
                                    estudiantes.Add(est);
                                }

                                // Agregar curso/matricula
                                int idMat = r["IdMatricula"] == DBNull.Value ? 0 : Convert.ToInt32(r["IdMatricula"]);
                                int idCurso = r["IdCurso"] == DBNull.Value ? 0 : Convert.ToInt32(r["IdCurso"]);
                                string nombreCurso = r["CursoNombre"] == DBNull.Value ? "Curso" : r["CursoNombre"].ToString();
                                DateTime fechaMat = r["FechaMatricula"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["FechaMatricula"]);

                                // Evita duplicados de la misma matricula
                                if (!est.Cursos.Any(c => c.IdMatricula == idMat && c.IdCurso == idCurso))
                                    est.Cursos.Add(new CursoMat { IdMatricula = idMat, IdCurso = idCurso, NombreCurso = nombreCurso, FechaMatricula = fechaMat });
                            }
                        }
                    }

                    if (estudiantes.Count == 0)
                    {
                        pnlContenedor.Controls.Add(CrearInfoLabel("No hay estudiantes matriculados en sus cursos."));
                        return;
                    }

                    // 3) Precalcular encuestas activas totales (para medir respuesta ratio)
                    int encuestasActivas = SafeIntQuery(conn, "SELECT COUNT(*) FROM Encuestas WHERE Activo = 1");

                    // 4) Detectar si columnas existen
                    bool asist_tiene_IdEstudiante = ColumnExists(conn, "Asistencias", "IdEstudiante");
                    bool asist_tiene_IdMatricula = ColumnExists(conn, "Asistencias", "IdMatricula");
                    bool resp_tiene_IdEstudiante = ColumnExists(conn, "Respuestas", "IdEstudiante");
                    bool resp_tiene_IdUsuario = ColumnExists(conn, "Respuestas", "IdUsuario");
                    bool resp_tiene_IdMatricula = ColumnExists(conn, "Respuestas", "IdMatricula");

                    // construimos lista de IdEstudiante y lista de IdMatricula para consultas (evitar "IN ()")
                    var listaEstIds = estudiantes.Select(e => e.IdUsuario).Distinct().Where(x => x > 0).ToList();
                    var listaMatIds = estudiantes.SelectMany(e => e.Cursos.Select(c => c.IdMatricula)).Where(x => x > 0).Distinct().ToList();

                    // Asistencias últimos 30 días
                    DateTime desde = DateTime.Now.AddDays(-30);
                    var asistMap = new Dictionary<int, (int total, int pres)>(); // key = IdEstudiante

                    if (asist_tiene_IdEstudiante && listaEstIds.Count > 0)
                    {
                        string ids = string.Join(",", listaEstIds);
                        using (var cmd = new SqlCommand($@"
                                SELECT IdEstudiante, COUNT(*) AS total, SUM(CASE WHEN Asistio = 1 THEN 1 ELSE 0 END) AS pres
                                FROM Asistencias
                                WHERE Fecha >= @d AND IdEstudiante IN ({ids})
                                GROUP BY IdEstudiante", conn))
                        {
                            cmd.Parameters.AddWithValue("@d", desde);
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    int idE = Convert.ToInt32(r["IdEstudiante"]);
                                    int t = Convert.ToInt32(r["total"]);
                                    int p = Convert.ToInt32(r["pres"]);
                                    asistMap[idE] = (t, p);
                                }
                            }
                        }
                    }
                    else if (asist_tiene_IdMatricula && listaMatIds.Count > 0)
                    {
                        string ids = string.Join(",", listaMatIds);
                        using (var cmd = new SqlCommand($@"
                                SELECT m.IdEstudiante, COUNT(*) AS total, SUM(CASE WHEN a.Asistio = 1 THEN 1 ELSE 0 END) AS pres
                                FROM Asistencias a
                                INNER JOIN Matriculas m ON m.Id = a.IdMatricula
                                WHERE a.Fecha >= @d AND a.IdMatricula IN ({ids})
                                GROUP BY m.IdEstudiante", conn))
                        {
                            cmd.Parameters.AddWithValue("@d", desde);
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    int idE = Convert.ToInt32(r["IdEstudiante"]);
                                    int t = Convert.ToInt32(r["total"]);
                                    int p = Convert.ToInt32(r["pres"]);
                                    asistMap[idE] = (t, p);
                                }
                            }
                        }
                    }

                    // Respuestas por estudiante (puede estar en Respuestas.IdEstudiante o Respuestas.IdUsuario o por IdMatricula)
                    var respMap = new Dictionary<int, int>(); // idEst -> count
                    if (resp_tiene_IdEstudiante && listaEstIds.Count > 0)
                    {
                        string ids = string.Join(",", listaEstIds);
                        using (var cmd = new SqlCommand($@"
                                SELECT IdEstudiante, COUNT(*) AS cnt
                                FROM Respuestas
                                WHERE IdEstudiante IN ({ids})
                                GROUP BY IdEstudiante", conn))
                        {
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read()) respMap[Convert.ToInt32(r["IdEstudiante"])] = Convert.ToInt32(r["cnt"]);
                            }
                        }
                    }
                    else if (resp_tiene_IdUsuario && listaEstIds.Count > 0)
                    {
                        string ids = string.Join(",", listaEstIds);
                        using (var cmd = new SqlCommand($@"
                                SELECT IdUsuario, COUNT(*) AS cnt
                                FROM Respuestas
                                WHERE IdUsuario IN ({ids})
                                GROUP BY IdUsuario", conn))
                        {
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read()) respMap[Convert.ToInt32(r["IdUsuario"])] = Convert.ToInt32(r["cnt"]);
                            }
                        }
                    }
                    else if (resp_tiene_IdMatricula && listaMatIds.Count > 0)
                    {
                        string ids = string.Join(",", listaMatIds);
                        using (var cmd = new SqlCommand($@"
                                SELECT m.IdEstudiante, COUNT(*) AS cnt
                                FROM Respuestas resp
                                INNER JOIN Matriculas m ON m.Id = resp.IdMatricula
                                WHERE resp.IdMatricula IN ({ids})
                                GROUP BY m.IdEstudiante", conn))
                        {
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read()) respMap[Convert.ToInt32(r["IdEstudiante"])] = Convert.ToInt32(r["cnt"]);
                            }
                        }
                    }

                    // 5) Construir tarjetas por estudiante (opción C: una tarjeta por estudiante, con lista de cursos)
                    foreach (var est in estudiantes)
                    {
                        // asistencia total y porcentaje
                        int totalAsist = 0, presAsist = 0;
                        if (asistMap.ContainsKey(est.IdUsuario))
                        {
                            var t = asistMap[est.IdUsuario];
                            totalAsist = t.total;
                            presAsist = t.pres;
                        }
                        double porcAsistencia = totalAsist > 0 ? (double)presAsist / totalAsist : 1.0;

                        // respuestas
                        int respCnt = respMap.ContainsKey(est.IdUsuario) ? respMap[est.IdUsuario] : 0;

                        // riesgo (modelo B): asistencia < 0.60 OR respuestas/encuestasActivas < 0.5
                        bool riesgoAsistencia = porcAsistencia < 0.60;
                        bool riesgoEncuestas = false;
                        if (encuestasActivas > 0)
                        {
                            double porcResp = (double)respCnt / encuestasActivas;
                            riesgoEncuestas = porcResp < 0.50;
                        }
                        bool enRiesgo = riesgoAsistencia || riesgoEncuestas;

                        // Crear tarjeta: encabezado con nombre + lista de cursos debajo en un panel interno
                        Panel card = CrearCardEstudiante(est, porcAsistencia, respCnt, enRiesgo);

                        // centrar horizontalmente (margen izquierdo)
                        CenterCardMargin(card);

                        pnlContenedor.Controls.Add(card);
                    }
                } // conn
            }
            catch (Exception ex)
            {
                // Mostrar mensaje y no romper UI
                pnlContenedor.Controls.Add(CrearInfoLabel("Error cargando estudiantes: " + ex.Message));
            }
            finally
            {
                pnlContenedor.ResumeLayout();
            }
        }

        // Ajusta margen izquierdo para centrar la tarjeta dentro del pnlContenedor
        private void CenterCardMargin(Control card)
        {
            try
            {
                int containerWidth = pnlContenedor.ClientSize.Width - pnlContenedor.Padding.Left - pnlContenedor.Padding.Right;
                if (containerWidth <= 0) containerWidth = pnlContenedor.Width;
                int left = Math.Max(0, (containerWidth - card.Width) / 2);
                // dejamos 12px de separación superior por defecto
                card.Margin = new Padding(left, 8, 0, 8);
            }
            catch
            {
                card.Margin = new Padding(0, 8, 0, 8);
            }
        }

        // Reajusta todas las tarjetas al redimensionar el panel
        private void CenterCardsHorizontally()
        {
            try
            {
                foreach (Control c in pnlContenedor.Controls)
                {
                    if (c is Panel)
                        CenterCardMargin(c);
                }
            }
            catch { /* no bloquear UI */ }
        }

        // Crea la tarjeta de estudiante (con lista de cursos) - tamaño grande, nombre centrado
        private Panel CrearCardEstudiante(EstudianteInfo est, double porcAsistencia, int respuestas, bool enRiesgo)
        {
            // Ancho fijo — el FlowLayoutPanel lo respeta
            int cardWidth = 900;
            int cardHeight = 300;

            Panel card = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                BackColor = Color.White,
                Margin = new Padding(0, 20, 0, 20),
                Padding = new Padding(16)
            };

            int r = 14;
            card.Paint += (s, e) =>
            {
                try
                {
                    card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, r, r));
                }
                catch { }
            };

            // --- 1) NOMBRE CENTRADO PERO MÁS ABAJO ---
            Label lblName = new Label
            {
                Text = $"{est.Nombre} {est.Apellido}  (ID: {est.IdUsuario})",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = cardWidth - 45,
                Height = 30,
                Location = new Point(16, 8)
            };


            // Lo bajamos y lo centramos
            lblName.Location = new Point(
                (card.Width - lblName.Width) / 2,
                25  // ← AQUI CONTROLAS QUE TAN ABAJO SALE EL NOMBRE
            );

            card.Controls.Add(lblName);


            // --- 2) CURSOS PEGADOS A LA IZQUIERDA ---
            FlowLayoutPanel cursosPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(25, lblName.Bottom + 15),  // ← pegado a la izquierda
                Width = 500
            };

            // agregar cursos
            foreach (var c in est.Cursos)
            {
                string fecha = c.FechaMatricula == DateTime.MinValue ? "" : $" ({c.FechaMatricula:dd/MM/yyyy})";
                cursosPanel.Controls.Add(new Label
                {
                    Text = $"• {c.NombreCurso}{fecha}",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(70, 70, 70)
                });
            }

            card.Controls.Add(cursosPanel);


            // 3) STATS (derecha)
            Panel stats = new Panel
            {
                Width = 280,
                Height = 135,
                Location = new Point(cardWidth - 16 - 280, 70),
                BackColor = Color.Transparent
            };

            stats.Controls.Add(new Label
            {
                Text = $"Asistencia: {Math.Round(porcAsistencia * 100)}%",
                AutoSize = true,
                Location = new Point(0, 10),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            stats.Controls.Add(new Label
            {
                Text = $"Respuestas: {respuestas}",
                AutoSize = true,
                Location = new Point(0, 40),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            stats.Controls.Add(new Label
            {
                Text = enRiesgo ? "Riesgo: Sí" : "Riesgo: No",
                AutoSize = true,
                Location = new Point(0, 70),
                Font = new Font("Segoe UI Semibold", 10),
                ForeColor = enRiesgo ? Color.FromArgb(200, 60, 60) : Color.FromArgb(60, 140, 60)
            });

            card.Controls.Add(stats);

            // 4) BOTÓN (derecha abajo)
            Button btnDetalles = new Button
            {
                Text = "Ver detalles",
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(cardWidth - 16 - 110, cardHeight - 44)
            };
            btnDetalles.FlatAppearance.BorderSize = 0;

            btnDetalles.Click += (s, e) =>
            {
                MessageBox.Show(
                    $"Detalles de {est.Nombre} {est.Apellido}\nCursos: {est.Cursos.Count}",
                    "Detalles",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            card.Controls.Add(btnDetalles);

            return card;
        }

        private Label CrearInfoLabel(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(20)
            };
        }

        // Comprueba si existe columna en tabla
        private bool ColumnExists(SqlConnection conn, string tableName, string columnName)
        {
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @table AND COLUMN_NAME = @col", conn))
                {
                    cmd.Parameters.AddWithValue("@table", tableName);
                    cmd.Parameters.AddWithValue("@col", columnName);
                    int v = Convert.ToInt32(cmd.ExecuteScalar());
                    return v > 0;
                }
            }
            catch { return false; }
        }

        // Safe COUNT(*) usando conexión ya abierta
        private int SafeIntQuery(SqlConnection conn, string sql, params SqlParameter[] ps)
        {
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
                    object r = cmd.ExecuteScalar();
                    if (r == null || r == DBNull.Value) return 0;
                    return Convert.ToInt32(r);
                }
            }
            catch { return 0; }
        }

        // IMPORT - crea region redondeada
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        // ----- helper classes -----
        private class EstudianteInfo
        {
            public int IdUsuario;
            public string Nombre;
            public string Apellido;
            public List<CursoMat> Cursos;
        }

        private class CursoMat
        {
            public int IdMatricula;
            public int IdCurso;
            public string NombreCurso;
            public DateTime FechaMatricula;
        }
    }
}
