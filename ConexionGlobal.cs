using System;
using UPC.SmartRetention.UI;

public static class ConexionGlobal
{
    private static ProyectoVisualEntities _context;

    public static ProyectoVisualEntities Contexto
    {
        get
        {
            if (_context == null)
                _context = new ProyectoVisualEntities();
            return _context;
        }
    }

    // Usuario logueado actualmente
    public static Usuarios UsuarioActual { get; set; }
}
