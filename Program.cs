using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ⚡ Precargar Entity Framework y la conexión antes de mostrar el login
            PrecargarEF();

            Application.Run(new FormLogin());
        }

        private static void PrecargarEF()
        {
            try
            {
                using (var db = new ProyectoVisualEntities())
                {
                    // Esto fuerza la compilación y abre la conexión
                    db.Usuarios.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al precargar la base de datos: " + ex.Message);
            }
        }
    }
}
