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
            this.Text = "Panel de Administración";
        }

        private void FormAdminPanel_Load(object sender, EventArgs e)
        {
            // Aquí podrías abrir un dashboard o el FormEstudiantes
        }
    }

}
