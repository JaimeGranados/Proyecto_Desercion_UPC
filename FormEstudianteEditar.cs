using System;
using System.Windows.Forms;
using UPC.SmartRetention.BLL;
using UPC.SmartRetention.Models;

namespace UPC.SmartRetention.UI
{
    public partial class FormEstudianteEditar : Form
    {
        private readonly EstudianteBLL _bll = new EstudianteBLL();
        private readonly bool _modoEditar;
        private readonly Estudiante _estudianteEditar;

        public FormEstudianteEditar()
        {
            InitializeComponent();

            // Estilo base (igual que el FormLogin)
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.BackgroundImage = Properties.Resources.album_joji;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;
        }

        public FormEstudianteEditar(Estudiante est) : this()
        {
            _modoEditar = true;
            _estudianteEditar = est;
        }

        private void FormEstudianteEditar_Load(object sender, EventArgs e)
        {
            CargarProgramas();

            if (_modoEditar && _estudianteEditar != null)
            {
                lblTitulo.Text = "Editar Estudiante";
                txtNombre.Text = _estudianteEditar.NombreCompleto;
                nudSemestre.Value = _estudianteEditar.Semestre;
                nudPromedio.Value = Convert.ToDecimal(_estudianteEditar.Promedio);
                nudAsistencia.Value = Convert.ToDecimal(_estudianteEditar.Asistencia);
                cmbPrograma.SelectedIndex = _estudianteEditar.IdPrograma - 1;
            }
        }

        private void CargarProgramas()
        {
            // Temporal — luego lo traeremos desde la base de datos
            cmbPrograma.Items.Add("Ingeniería de Sistemas");
            cmbPrograma.Items.Add("Administración de Empresas");
            cmbPrograma.Items.Add("Psicología");
            cmbPrograma.SelectedIndex = 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Estudiante est = new Estudiante
            {
                IdPrograma = cmbPrograma.SelectedIndex + 1,
                NombreCompleto = txtNombre.Text.Trim(),
                Semestre = (int)nudSemestre.Value,
                Promedio = nudPromedio.Value,
                Asistencia = nudAsistencia.Value,
                EstratoSocioeconomico = 3,
                RiesgoDesercion = "Bajo"
            };

            string mensaje;
            if (_modoEditar)
            {
                est.IdEstudiante = _estudianteEditar.IdEstudiante;
                mensaje = _bll.ActualizarEstudiante(est);
            }
            else
            {
                mensaje = _bll.GuardarEstudiante(est);
            }

            MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
