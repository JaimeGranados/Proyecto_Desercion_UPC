using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPC.SmartRetention.UI
{
    public partial class FormAdminPanel : Form
    {
        public FormAdminPanel()
        {
            InitializeComponent();

            // Fondo general oscuro
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.DoubleBuffered = true; // Evita parpadeos visuales

            // Imagen de fondo (puedes quitarla si prefieres solo color)
            this.BackgroundImage = Properties.Resources.album_joji;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Si tienes paneles en el diseñador, aplica sus colores
            if (this.Controls.ContainsKey("panelMenu"))
            {
                this.Controls["panelMenu"].BackColor = Color.FromArgb(25, 25, 25);
            }

            if (this.Controls.ContainsKey("panelContenido"))
            {
                this.Controls["panelContenido"].BackColor = Color.FromArgb(40, 40, 40);
            }
        }
    }
}
