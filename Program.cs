using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                InicializarBaseDeDatos();
                MessageBox.Show("✅ Base de datos inicializada correctamente (tablas de encuestas creadas o verificadas).",
                    "SmartRetention", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al inicializar la base de datos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Run(new FormLogin());
        }

        static void InicializarBaseDeDatos()
        {
            string connectionString = "Server=localhost;Database=Proyectovisual;User Id=sa;Password=NuevaContraseñaFuerte123!;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string script = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Encuestas' AND xtype='U')
                CREATE TABLE Encuestas (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Titulo NVARCHAR(200) NOT NULL,
                    Descripcion NVARCHAR(500),
                    FechaCreacion DATETIME DEFAULT GETDATE(),
                    Activo BIT DEFAULT 1
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Preguntas' AND xtype='U')
                CREATE TABLE Preguntas (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    IdEncuesta INT NOT NULL FOREIGN KEY REFERENCES Encuestas(Id) ON DELETE CASCADE,
                    Texto NVARCHAR(300) NOT NULL,
                    Tipo INT NOT NULL -- 1=opcion multiple, 2=text, 3=escala 1-5
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Opciones' AND xtype='U')
                CREATE TABLE Opciones (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    IdPregunta INT NOT NULL FOREIGN KEY REFERENCES Preguntas(Id) ON DELETE CASCADE,
                    TextoOpcion NVARCHAR(200) NOT NULL
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Respuestas' AND xtype='U')
                CREATE TABLE Respuestas (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    IdPregunta INT NOT NULL FOREIGN KEY REFERENCES Preguntas(Id) ON DELETE CASCADE,
                    IdUsuario INT NULL,
                    RespuestaTexto NVARCHAR(500),
                    FechaRespuesta DATETIME DEFAULT GETDATE()
                );
                ";

                using (SqlCommand cmd = new SqlCommand(script, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
