using System;
using System.Data;
using System.Windows.Forms;
using UPC.SmartRetention.BLL;
using UPC.SmartRetention.Models;

namespace UPC.SmartRetention.UI
{
    public partial class FormEstudiantes : Form
    {
        private readonly EstudianteBLL _bll = new EstudianteBLL();

        public FormEstudiantes()
        {
            InitializeComponent();
        }

        private void FormEstudiantes_Load(object sender, EventArgs e)
        {
            CargarEstudiantes();
        }

        private void CargarEstudiantes()
        {
            try
            {
                dgvEstudiantes.DataSource = _bll.ListarEstudiantes();
                dgvEstudiantes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los estudiantes: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarEstudiantes();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un estudiante para eliminar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvEstudiantes.SelectedRows[0].Cells["IdEstudiante"].Value);
            string resultado = _bll.EliminarEstudiante(id);
            MessageBox.Show(resultado, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarEstudiantes();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (FormEstudianteEditar frm = new FormEstudianteEditar())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarEstudiantes();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un estudiante para editar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Estudiante est = new Estudiante
            {
                IdEstudiante = Convert.ToInt32(dgvEstudiantes.SelectedRows[0].Cells["IdEstudiante"].Value),
                NombreCompleto = dgvEstudiantes.SelectedRows[0].Cells["NombreCompleto"].Value.ToString(),
                Semestre = Convert.ToInt32(dgvEstudiantes.SelectedRows[0].Cells["Semestre"].Value),
                Promedio = Convert.ToDecimal(dgvEstudiantes.SelectedRows[0].Cells["Promedio"].Value),
                Asistencia = Convert.ToDecimal(dgvEstudiantes.SelectedRows[0].Cells["Asistencia"].Value),
                RiesgoDesercion = dgvEstudiantes.SelectedRows[0].Cells["RiesgoDesercion"].Value.ToString(),
                IdPrograma = 1 // Temporal hasta que agregues campo real
            };

            using (FormEstudianteEditar frm = new FormEstudianteEditar(est))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarEstudiantes();
            }
        }

        // ---- Métodos vacíos para eliminar errores del diseñador ----

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Si más adelante necesitas reaccionar al clic de una celda, aquí irá el código
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Si hay un botón adicional en el diseñador, déjalo vacío por ahora
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Otro botón adicional — lo llenamos cuando se sepa su función
        }
    }
}
