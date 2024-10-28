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
            public List<Busqueda> BuscarPorid(int Id)
            {

                List<Busqueda> reservas = new List<Busqueda>();
                try
                {
                    using (MySqlConnection conn = new Conexion().GetConnection())
                    {
                        conn.Open(); // Asegúrate de abrir la conexión

                       
                        string sql = @"SELECT r.reservaId, r.salaId, r.fechaReserva, r.horaInicio, r.horaFin, 
                                  r.asistentes, IFNULL(r.menuSeleccionado, ''), IFNULL(r.totalPrecio, 0), 
                                  u.nombreUsuario 
                           FROM reservas r 
                           INNER JOIN usuarios u ON r.usuarioId = u.usuarioId 
                           WHERE r.reservaId = @reservaId";

                        MySqlCommand comando = new MySqlCommand(sql, conn);
                        comando.Parameters.AddWithValue("@reservaId", Id); // Uso de parámetro para evitar inyección SQL

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {

                            while (lector.Read())
                            {
                                int reservaId = lector.GetInt32(0);
                                int salaIdDb = lector.GetInt32(1);
                                DateTime fechaReserva = lector.GetDateTime(2);
                                TimeSpan horaInicio = lector.GetTimeSpan(3);
                                TimeSpan horaFin = lector.GetTimeSpan(4);
                                int asistentes = lector.GetInt32(5);
                                string menuSeleccionado = lector.GetString(6);
                                int precioTotal = lector.GetInt32(7);
                                string nombreUsuario = lector.GetString(8); // Asegúrate de que el índice es correcto y estás obteniendo el nombre del cliente

                                var reserva = new Busqueda(reservaId, salaIdDb, 0, nombreUsuario, fechaReserva.ToString("yyyy-MM-dd"),
                                                           horaInicio.ToString(@"hh\:mm"), horaFin.ToString(@"hh\:mm"),
                                                           asistentes, menuSeleccionado, Convert.ToInt32(precioTotal));

                                reservas.Add(reserva);
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

            // Nuevo método para buscar por salaId
            public List<Busqueda> BuscarPorSala(int salaId)
            {
                List<Busqueda> reservas = new List<Busqueda>();
                try
                {
                    using (MySqlConnection conn = new Conexion().GetConnection())
                    {
                        conn.Open(); // Asegúrate de abrir la conexión

                      
                        string sql = @"SELECT r.reservaId, r.salaId, r.fechaReserva, r.horaInicio, r.horaFin, 
                                  r.asistentes, IFNULL(r.menuSeleccionado, ''), IFNULL(r.totalPrecio, 0), 
                                  u.nombreUsuario 
                           FROM reservas r 
                           INNER JOIN usuarios u ON r.usuarioId = u.usuarioId 
                           WHERE r.salaId = @SalaId";

                        MySqlCommand comando = new MySqlCommand(sql, conn);
                        comando.Parameters.AddWithValue("@SalaId", salaId); // Uso de parámetro para evitar inyección SQL

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {

                            while (lector.Read())
                            {
                                int reservaId = lector.GetInt32(0);
                                int salaIdDb = lector.GetInt32(1);
                                DateTime fechaReserva = lector.GetDateTime(2);
                                TimeSpan horaInicio = lector.GetTimeSpan(3);
                                TimeSpan horaFin = lector.GetTimeSpan(4);
                                int asistentes = lector.GetInt32(5);
                                string menuSeleccionado = lector.GetString(6);
                                int precioTotal = lector.GetInt32(7);
                                string nombreUsuario = lector.GetString(8); // Asegúrate de que el índice es correcto y estás obteniendo el nombre del cliente

                                var reserva = new Busqueda(reservaId, salaIdDb, 0, nombreUsuario, fechaReserva.ToString("yyyy-MM-dd"),
                                                           horaInicio.ToString(@"hh\:mm"), horaFin.ToString(@"hh\:mm"),
                                                           asistentes, menuSeleccionado, Convert.ToInt32(precioTotal));

                                reservas.Add(reserva);
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

            public List<Busqueda> BuscarPorCliente(string nombreCliente)
            {
                List<Busqueda> listaReservas = new List<Busqueda>();

                string query = @"SELECT r.reservaId, r.salaId, r.usuarioId, u.nombreUsuario, r.fechaReserva, r.horaInicio, r.horaFin, r.asistentes, r.menuSeleccionado, r.totalPrecio 
                     FROM reservas r 
                     INNER JOIN usuarios u ON r.usuarioId = u.usuarioId 
                     WHERE u.nombreUsuario LIKE @NombreCliente";

                Conexion conexion = new Conexion();
                using (MySqlConnection connection = conexion.GetConnection())
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NombreCliente", "%" + nombreCliente + "%"); // Coincidencia parcial

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Verificar y manejar posibles valores nulos
                                int reservaId = reader.IsDBNull(reader.GetOrdinal("reservaId")) ? 0 : reader.GetInt32(reader.GetOrdinal("reservaId"));
                                int salaId = reader.IsDBNull(reader.GetOrdinal("salaId")) ? 0 : reader.GetInt32(reader.GetOrdinal("salaId"));
                                int usuarioId = reader.IsDBNull(reader.GetOrdinal("usuarioId")) ? 0 : reader.GetInt32(reader.GetOrdinal("usuarioId"));
                                string nombreUsuario = reader.IsDBNull(reader.GetOrdinal("nombreUsuario")) ? "" : reader.GetString(reader.GetOrdinal("nombreUsuario")); // Asegúrate de que este sea el nombre de la columna
                                string fechaReserva = reader.IsDBNull(reader.GetOrdinal("fechaReserva")) ? "" : reader["fechaReserva"].ToString();
                                string horaInicio = reader.IsDBNull(reader.GetOrdinal("horaInicio")) ? "" : reader["horaInicio"].ToString();
                                string horaFin = reader.IsDBNull(reader.GetOrdinal("horaFin")) ? "" : reader["horaFin"].ToString();
                                int asistentes = reader.IsDBNull(reader.GetOrdinal("asistentes")) ? 0 : reader.GetInt32(reader.GetOrdinal("asistentes"));
                                string menuSeleccionado = reader.IsDBNull(reader.GetOrdinal("menuSeleccionado")) ? "" : reader.GetString(reader.GetOrdinal("menuSeleccionado"));
                                decimal precioTotal = reader.IsDBNull(reader.GetOrdinal("totalPrecio")) ? 0 : reader.GetDecimal(reader.GetOrdinal("totalPrecio"));

                                // Crear objeto Busqueda incluyendo el nombre de usuario
                                listaReservas.Add(new Busqueda(
                                    reservaId,
                                    salaId,
                                    usuarioId,
                                    nombreUsuario, // Aquí se asigna el nombre del usuario
                                    fechaReserva,
                                    horaInicio,
                                    horaFin,
                                    asistentes,
                                    menuSeleccionado,
                                    Convert.ToInt32(precioTotal)
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al buscar por cliente: " + ex.Message);
                    }
                }

                return listaReservas;
            }

            public List<Busqueda> BuscarPorFecha(DateTime fechaReserva)
            {
                List<Busqueda> listaReservas = new List<Busqueda>();

                string query = @"SELECT r.reservaId, r.salaId, r.usuarioId, u.nombreUsuario, r.fechaReserva, r.horaInicio, r.horaFin, r.asistentes, r.menuSeleccionado, r.totalPrecio 
                     FROM reservas r 
                     INNER JOIN usuarios u ON r.usuarioId = u.usuarioId 
                     WHERE r.fechaReserva = @FechaReserva";

                Conexion conexion = new Conexion();
                using (MySqlConnection connection = conexion.GetConnection())
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FechaReserva", fechaReserva);

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Verificar y manejar posibles valores nulos
                                int reservaId = reader.IsDBNull(reader.GetOrdinal("reservaId")) ? 0 : reader.GetInt32(reader.GetOrdinal("reservaId"));
                                int salaId = reader.IsDBNull(reader.GetOrdinal("salaId")) ? 0 : reader.GetInt32(reader.GetOrdinal("salaId"));
                                int usuarioId = reader.IsDBNull(reader.GetOrdinal("usuarioId")) ? 0 : reader.GetInt32(reader.GetOrdinal("usuarioId"));
                                string nombreUsuario = reader.IsDBNull(reader.GetOrdinal("nombreUsuario")) ? "" : reader.GetString(reader.GetOrdinal("nombreUsuario"));
                                string horaInicio = reader.IsDBNull(reader.GetOrdinal("horaInicio")) ? "" : reader["horaInicio"].ToString();
                                string horaFin = reader.IsDBNull(reader.GetOrdinal("horaFin")) ? "" : reader["horaFin"].ToString();
                                int asistentes = reader.IsDBNull(reader.GetOrdinal("asistentes")) ? 0 : reader.GetInt32(reader.GetOrdinal("asistentes"));
                                string menuSeleccionado = reader.IsDBNull(reader.GetOrdinal("menuSeleccionado")) ? "" : reader.GetString(reader.GetOrdinal("menuSeleccionado"));
                                decimal precioTotal = reader.IsDBNull(reader.GetOrdinal("totalPrecio")) ? 0 : reader.GetDecimal(reader.GetOrdinal("totalPrecio"));

                                listaReservas.Add(new Busqueda(
                                    reservaId,
                                    salaId,
                                    usuarioId,
                                    nombreUsuario, // Asignamos el nombre de usuario al constructor
                                    fechaReserva.ToString("yyyy-MM-dd"), // Convertimos la fecha a string
                                    horaInicio,
                                    horaFin,
                                    asistentes,
                                    menuSeleccionado,
                                    Convert.ToInt32(precioTotal)
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al buscar por fecha: " + ex.Message);
                    }
                }

                return listaReservas;
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
                        //Busqueda reserva = new BusquedaRepositorio().(id);
                        List<Busqueda> reservas = new BusquedaRepositorio().BuscarPorid(id);
                        if (reservas != null)
                        {
                            DgvReservas.AutoGenerateColumns = true;
                            foreach (var reserva in reservas)
                            {
                                // Crea el arreglo de datos con las propiedades de la reserva
                                object[] data =
                                {
                                    reserva.Id,
                                    reserva.SalaId,
                                    reserva.NombreUsuario,
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
                else if (CboCriterioBusqueda.SelectedIndex == 0) // Por Cliente (Usuario)
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
                                    reserva.NombreUsuario,
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
                else if (CboCriterioBusqueda.SelectedIndex == 2) // Por Cliente (Usuario)
                {
                    // Búsqueda por cliente (nombre o apellido)
                    string nombreCliente = TxtValorBuscar.Text.Trim();

                    if (!string.IsNullOrEmpty(nombreCliente))
                    {
                        List<Busqueda> reservas = new BusquedaRepositorio().BuscarPorCliente(nombreCliente);
                        if (reservas.Count > 0)
                        {
                            DgvReservas.AutoGenerateColumns = true;
                            foreach (var reserva in reservas)
                            {
                                object[] data =
                                {
                            reserva.Id,
                            reserva.SalaId,
                            reserva.NombreUsuario,
                            reserva.FechaReserva,
                            reserva.HoraInicio,
                            reserva.HoraFin,
                            reserva.Asistentes,
                            reserva.Menu,
                            reserva.PrecioTotal
                        };

                                DgvReservas.Rows.Add(data);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron reservas para ese cliente.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor ingresa el nombre o apellido del cliente.", "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (CboCriterioBusqueda.SelectedIndex == 3) // Por Fecha
                {
                    DateTime fechaBusqueda;
                    if (DateTime.TryParse(TxtValorBuscar.Text, out fechaBusqueda))
                    {
                        // Búsqueda por fecha
                        List<Busqueda> reservas = new BusquedaRepositorio().BuscarPorFecha(fechaBusqueda);
                        if (reservas.Count > 0)
                        {
                            DgvReservas.AutoGenerateColumns = true;
                            foreach (var reserva in reservas)
                            {
                                object[] data =
                                {
                            reserva.Id,
                            reserva.SalaId,
                            reserva.NombreUsuario,
                            reserva.FechaReserva,
                            reserva.HoraInicio,
                            reserva.HoraFin,
                            reserva.Asistentes,
                            reserva.Menu,
                            reserva.PrecioTotal
                        };

                                DgvReservas.Rows.Add(data);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron reservas para esa fecha.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor ingresa una fecha válida (formato: dd/mm/aaaa).", "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}
