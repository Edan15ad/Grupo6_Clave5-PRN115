using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;



namespace Clave5_Grupo6
{
    public partial class Form1 : Form
    {
        private List<Reserva> reservas;
        public Form1()
        {
            InitializeComponent();
            CargarMenus();
            CargarReservas();
        }
        private void CargarMenus()
        {
            // Ejemplo de menús
            cmbMenu.Items.Add("Menú 1");
            cmbMenu.Items.Add("Menú 2");
            cmbMenu.Items.Add("Menú 3");
        }

        private void CargarReservas()
        {
            reservas = Reserva.ObtenerReservas();
            dgvReservas.DataSource = reservas;

            // Renombrar las columnas
            dgvReservas.Columns["ReservaId"].HeaderText = "Reserva";
            dgvReservas.Columns["SalaId"].HeaderText = "Sala";
            dgvReservas.Columns["UsuarioId"].HeaderText = "Usuario";
            dgvReservas.Columns["Fecha"].HeaderText = "Fecha";
            dgvReservas.Columns["HoraInicio"].HeaderText = "Hora de Inicio";
            dgvReservas.Columns["HoraFin"].HeaderText = "Hora de Fin";
            dgvReservas.Columns["NumeroAsistentes"].HeaderText = "Asistentes";
            dgvReservas.Columns["MenuSeleccionado"].HeaderText = "Menú";
        }

        public class SalaItem
        {
            public int SalaId { get; set; }
            public string NombreSala { get; set; }

            public override string ToString()
            {
                return NombreSala; // Esto asegura que el ComboBox muestre el nombre de la sala
            }
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos vacíos
                if (
                    string.IsNullOrWhiteSpace(txtCliente.Text) ||
                    string.IsNullOrWhiteSpace(txtAsistentes.Text) ||
                    cmbMenu.SelectedItem == null ||
                    cmbSalas.SelectedItem == null)  // Validar también la selección de sala
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                // Obtener o crear el usuario
                string nombreCliente = txtCliente.Text;
                Usuario usuarioExistente = Usuario.BuscarUsuario(nombreCliente);

                int usuarioId;

                if (usuarioExistente != null)
                {
                    // Si el usuario ya existe, usar su ID
                    usuarioId = usuarioExistente.UsuarioId;
                }
                else
                {
                    // Si el usuario no existe, crear un nuevo usuario
                    Usuario nuevoUsuario = new Usuario
                    {
                        NombreUsuario = nombreCliente
                    };
                    nuevoUsuario.AgregarUsuario();

                    // Buscar el nuevo ID del usuario
                    usuarioId = Usuario.BuscarUsuario(nombreCliente).UsuarioId;
                }

                // Obtener SalaId del ComboBox
                int salaIdSeleccionada = ((SalaItem)cmbSalas.SelectedItem).SalaId; // <-- Aquí es donde va el código

                // Crear una nueva reserva
                var nuevaReserva = new Reserva
                {
                    SalaId = salaIdSeleccionada,
                    UsuarioId = usuarioId,
                    Fecha = dateTimePickerFecha.Value.Date, // Asegurar que se pase solo la fecha
                    HoraInicio = TimePickerHoraInicio.Value.TimeOfDay, // Asegurar que se pase el tiempo correctamente
                    HoraFin = TimePickerHoraFin.Value.TimeOfDay,
                    NumeroAsistentes = int.Parse(txtAsistentes.Text),
                    MenuSeleccionado = cmbMenu.SelectedItem.ToString(),
                };

                // Llama al método para agregar la reserva
                nuevaReserva.AgregarReserva();

                // Cargar las reservas en el DataGridView
                CargarReservas();

                // Limpiar los campos del formulario
                LimpiarCampos();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Error de formato: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al agregar la reserva: " + ex.Message);
            }
        }

        private void LimpiarCampos()
        {
            // Limpia todos los campos del formulario
            cmbSalas.SelectedIndex = -1; // Reinicia el ComboBox
            txtCliente.Clear();
            txtAsistentes.Clear();
            cmbMenu.SelectedIndex = -1; // Reinicia el ComboBox
            dateTimePickerFecha.Value = DateTime.Now; // Reinicia la fecha al valor actual
            TimePickerHoraInicio.Value = DateTime.Now; // Reinicia la hora de inicio
            TimePickerHoraFin.Value = DateTime.Now; // Reinicia la hora de fin
        }

        private void btnConectarBase_Click(object sender, EventArgs e)
        {
            // Cadena de conexión a la base de datos
            string connectionString = "server=localhost;database=clave5_grupodetrabajobd;user=root;password=root;port=3306;";

            // Intentar la conexión
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Abre la conexión
                    MessageBox.Show("Conexión exitosa a la base de datos.");

                    // Llamar a la función para cargar las salas en el ComboBox
                    CargarSalas(conn);  // Pasar la conexión abierta a la función
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
        }

        private void CargarSalas(MySqlConnection conn)
        {
            try
            {
                string query = "SELECT salaId, nombreSala FROM salas";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                // Limpiar el ComboBox antes de cargar nuevos datos
                cmbSalas.Items.Clear();

                while (reader.Read())
                {
                    // Agregar instancias de SalaItem en lugar de cadenas
                    cmbSalas.Items.Add(new SalaItem
                    {
                        SalaId = Convert.ToInt32(reader["salaId"]),
                        NombreSala = reader["nombreSala"].ToString()
                    });
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las salas: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvReservas.SelectedRows.Count > 0)
            {
                var reservaSeleccionada = (Reserva)dgvReservas.SelectedRows[0].DataBoundItem;

                // Obtener SalaId del ComboBox
                int salaIdSeleccionada = ((SalaItem)cmbSalas.SelectedItem).SalaId;


                // Otras actualizaciones
                reservaSeleccionada.UsuarioId = int.Parse(txtCliente.Text);
                reservaSeleccionada.Fecha = dateTimePickerFecha.Value;
                reservaSeleccionada.HoraInicio = TimePickerHoraInicio.Value.TimeOfDay;
                reservaSeleccionada.HoraFin = TimePickerHoraFin.Value.TimeOfDay;
                reservaSeleccionada.NumeroAsistentes = int.Parse(txtAsistentes.Text);
                reservaSeleccionada.MenuSeleccionado = cmbMenu.SelectedItem.ToString();

                reservaSeleccionada.ActualizarReserva();
                CargarReservas();
                LimpiarCampos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                // Pedir al usuario que ingrese el ID de la reserva a eliminar
                string inputId = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID de la reserva que desea eliminar:",
                                                                            "Eliminar Reserva",
                                                                            "");

                if (string.IsNullOrWhiteSpace(inputId))
                {
                    MessageBox.Show("Por favor, ingrese un ID válido.");
                    return;
                }

                int reservaId;
                if (!int.TryParse(inputId, out reservaId))
                {
                    MessageBox.Show("El ID de la reserva debe ser un número entero.");
                    return;
                }

                // Verificar si la lista de reservas está cargada
                if (reservas == null || reservas.Count == 0)
                {
                    MessageBox.Show("No hay reservas cargadas.");
                    return;
                }

                // Buscar si existe una reserva con el ID proporcionado
                var reservaSeleccionada = reservas.FirstOrDefault(r => r.ReservaId == reservaId);
                if (reservaSeleccionada == null)
                {
                    MessageBox.Show($"No se encontró una reserva con el ID {reservaId}.");
                    return;
                }

                // Confirmación antes de eliminar
                DialogResult result = MessageBox.Show($"¿Estás seguro de que deseas eliminar la reserva con ID {reservaId}?",
                                                      "Confirmar eliminación",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Eliminar la reserva de la base de datos
                    reservaSeleccionada.EliminarReserva();

                    // Eliminar la reserva del DataGridView y la lista local
                    reservas.Remove(reservaSeleccionada);
                    CargarReservas();

                    MessageBox.Show("Reserva eliminada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al eliminar la reserva: {ex.Message}");
            }
        }
    

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAsignarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                // Solicitar el nombre del usuario
                string nombreUsuario = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el nombre del usuario:", "Asignar ID a Usuario", "");

                if (string.IsNullOrWhiteSpace(nombreUsuario))
                {
                    MessageBox.Show("No se ha ingresado un nombre válido.");
                    return;
                }

                // Buscar si el usuario ya existe
                Usuario usuario = Usuario.BuscarUsuario(nombreUsuario);

                if (usuario != null)
                {
                    // Si el usuario existe, mostrar su ID
                    MessageBox.Show($"El usuario '{nombreUsuario}' ya existe con el ID {usuario.UsuarioId}.");
                }
                else
                {
                    // Si el usuario no existe, crear uno nuevo y asignar ID
                    Usuario nuevoUsuario = new Usuario
                    {
                        NombreUsuario = nombreUsuario
                    };
                    nuevoUsuario.AgregarUsuario();

                    // Buscar el usuario recién creado para obtener su ID
                    usuario = Usuario.BuscarUsuario(nombreUsuario);

                    if (usuario != null)
                    {
                        MessageBox.Show($"Se ha creado el usuario '{nombreUsuario}' con el ID {usuario.UsuarioId}.");
                    }
                    else
                    {
                        MessageBox.Show("Error al crear el usuario.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvReservas.AllowUserToAddRows = false;
            dgvReservas.ReadOnly = true;
          

        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            // Crear una instancia del Form2
            Form2 form2 = new Form2();

            // Mostrar el nuevo formulario
            form2.Show();
        }
    }
}
