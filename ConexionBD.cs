using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ProyectoVisual
{
    public class ConexionBD
    {
        private readonly string cadenaConexion;

        public ConexionBD()
        {
            // Leer la cadena de conexión del App.config
            cadenaConexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;
        }

        public SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            return conexion;
        }
    }
}
