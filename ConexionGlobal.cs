using System;
using UPC.SmartRetention.UI; // usa tu namespace (ajústalo si es diferente)

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
}
