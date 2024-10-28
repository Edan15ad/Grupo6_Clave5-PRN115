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



namespace Clave5_Grupo6
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Clase para manejar la búsqueda
        public class BusquedaRepositorio
        {
            // Método para buscar por reservaId
            public Busqueda Buscar(int id)
            {
                Busqueda reserva = null;
                try
                {
                    using (MySqlConnection conn = new Conexion().GetConnection())
                    {
                        conn.Open(); // Asegúrate de abrir la conexión

                        // Nueva consulta alineada con la estructura de la tabla
                        string sql = "SELECT reservaId, salaId, fechaReserva, horaInicio, horaFin, asistentes, IFNULL(menuSeleccionado, ''), IFNULL(totalPrecio, 0) " +
                                     "FROM reservas WHERE reservaId = @Id";

                        MySqlCommand comando = new MySqlCommand(sql, conn);
                        comando.Parameters.AddWithValue("@Id", id); // Uso de parámetro para evitar inyección SQL

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.HasRows)
                            {
                                if (lector.Read())
                                {
                                    // Leer los valores desde la base de datos
                                    int reservaId = lector.GetInt32(0);  // Obtiene el 'reservaId'
                                    int salaId = lector.GetInt32(1);     // Obtiene el 'salaId'

                                    // Conversión de fecha y hora desde la base de datos
                                    DateTime fechaReserva = lector.GetDateTime(2);  // Obtiene la 'fechaReserva' como DateTime
                                    TimeSpan horaInicio = lector.GetTimeSpan(3);    // Obtiene la 'horaInicio' como TimeSpan
                                    TimeSpan horaFin = lector.GetTimeSpan(4);       // Obtiene la 'horaFin' como TimeSpan

                                    // Lee otros campos
                                    int asistentes = lector.GetInt32(5);        // Obtiene la lista de asistentes como texto
                                    string menuSeleccionado = lector.GetString(6);  // Maneja nulos en el campo 'menuSeleccionado'
                                    decimal precioTotal = lector.GetDecimal(7);     // Obtiene el 'totalPrecio'

                                    // Crear una instancia de Busqueda con los datos obtenidos
                                    reserva = new Busqueda(reservaId, salaId, id,
                                                          fechaReserva.ToString("yyyy-MM-dd"), // Convertir la fecha a string
                                                          horaInicio.ToString(@"hh\:mm"),     // Convertir el TimeSpan a string en formato hh:mm
                                                          horaFin.ToString(@"hh\:mm"),        // Convertir el TimeSpan a string en formato hh:mm
                                                          asistentes, menuSeleccionado,
                                                          Convert.ToInt32(precioTotal));      // Convertir el total a int si es necesario
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return reserva;
            }

            // Nuevo método para buscar por salaId
            public List<Busqueda> BuscarPorSala(int salaId)
            {
                List<Busqueda> reservas = new List<Busqueda>();
                try
                {
                    using (MySqlConnection conn = new Conexion().GetConnection())
                    {
                        conn.Open(); // Asegúrate de abrir la conexión

                        string sql = "SELECT reservaId, salaId, fechaReserva, horaInicio, horaFin, asistentes, IFNULL(menuSeleccionado, ''), IFNULL(totalPrecio, 0) " +
                                     "FROM reservas WHERE salaId = @SalaId";

                        MySqlCommand comando = new MySqlCommand(sql, conn);
                        comando.Parameters.AddWithValue("@SalaId", salaId); // Uso de parámetro para evitar inyección SQL

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read()) // Cambiamos a while para leer múltiples reservas
                            {
                                int reservaId = lector.GetInt32(0);
                                int salaIdDb = lector.GetInt32(1);

                                DateTime fechaReserva = lector.GetDateTime(2);
                                TimeSpan horaInicio = lector.GetTimeSpan(3);
                                TimeSpan horaFin = lector.GetTimeSpan(4);

                                int asistentes = lector.GetInt32(5);
                                string menuSeleccionado = lector.GetString(6);
                                decimal precioTotal = lector.GetDecimal(7);

                                // Crear una instancia de Busqueda con los datos obtenidos
                                var reserva = new Busqueda(reservaId, salaIdDb, 0, // Se asigna 0 al usuarioId, ya que no se busca
                                                            fechaReserva.ToString("yyyy-MM-dd"),
                                                            horaInicio.ToString(@"hh\:mm"),
                                                            horaFin.ToString(@"hh\:mm"),
                                                            asistentes, menuSeleccionado,
                                                            Convert.ToInt32(precioTotal));
                                reservas.Add(reserva); // Agregar a la lista
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return reservas;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            DgvReservas.Rows.Clear(); // Limpia el DataGridView al inicio

            if (!String.IsNullOrEmpty(TxtValorBuscar.Text))
            {
                if (CboCriterioBusqueda.SelectedIndex == 0) // Por Id
                {
                    int id;
                    if (int.TryParse(TxtValorBuscar.Text, out id)) // Verifica si el valor es un número válido
                    {
                        // Realiza la búsqueda
                        Busqueda reserva = new BusquedaRepositorio().Buscar(id);
                        if (reserva != null)
                        {
                            // Asegúrate de que las columnas existan en el DataGridView o que AutoGenerateColumns sea true
                            DgvReservas.AutoGenerateColumns = true;

                            // Crea el arreglo de datos con las propiedades de la reserva
                            object[] data =
                            {
                                reserva.Id,
                                reserva.SalaId,
                                reserva.UsuarioId,
                                reserva.FechaReserva,
                                reserva.HoraInicio,
                                reserva.HoraFin,
                                reserva.Asistentes,
                                reserva.Menu,
                                reserva.PrecioTotal
                            };

                            // Agrega los datos al DataGridView
                            DgvReservas.Rows.Add(data);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró ninguna reserva con ese ID.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor ingresa un ID válido.", "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (CboCriterioBusqueda.SelectedIndex == 1) // Por Sala
                {
                    int salaId;
                    if (int.TryParse(TxtValorBuscar.Text, out salaId)) // Verifica si el valor es un número válido
                    {
                        // Realiza la búsqueda por sala
                        List<Busqueda> reservas = new BusquedaRepositorio().BuscarPorSala(salaId);
                        if (reservas.Count > 0)
                        {
                            DgvReservas.AutoGenerateColumns = true;
                            foreach (var reserva in reservas)
                            {
                                // Crea el arreglo de datos con las propiedades de la reserva
                                object[] data =
                                {
                                    reserva.Id,
                                    reserva.SalaId,
                                    reserva.UsuarioId,
                                    reserva.FechaReserva,
                                    reserva.HoraInicio,
                                    reserva.HoraFin,
                                    reserva.Asistentes,
                                    reserva.Menu,
                                    reserva.PrecioTotal
                                };

                                // Agrega los datos al DataGridView
                                DgvReservas.Rows.Add(data);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron reservas en esa sala.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor ingresa un ID de sala válido.", "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}
