using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clave5_Grupo6;
using MySql.Data.MySqlClient;
using Dapper;
using System.Windows.Forms;

namespace Clave5_Grupo6
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public int SalaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int NumeroAsistentes { get; set; }
        public string MenuSeleccionado { get; set; }


        private Conexion conexion = new Conexion();
 

        // Crear reserva
        public void AgregarReserva()
        {
            using (MySqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open(); // Asegurarte de abrir la conexión
                    string query = @"
                INSERT INTO reservas (salaId, usuarioId, fechaReserva, horaInicio, horaFin, asistentes, menuSeleccionado) 
                VALUES (@SalaId, @UsuarioId, @Fecha, @HoraInicio, @HoraFin, @NumeroAsistentes, @MenuSeleccionado)";
                   // connection.Execute(query, this);
                    connection.Execute(query, new { SalaId = this.SalaId, UsuarioId = this.UsuarioId, Fecha = this.Fecha, HoraInicio = this.HoraInicio, HoraFin = this.HoraFin, NumeroAsistentes = this.NumeroAsistentes, MenuSeleccionado = this.MenuSeleccionado });

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error de MySQL: " + ex.Message); // Capturar cualquier error de MySQL
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inesperado: " + ex.Message); // Capturar otros errores
                }
            }
        }

        // Leer todas las reservas
        // public static List<Reserva> ObtenerReservas()
        //{
        //  using (MySqlConnection connection = new Conexion().GetConnection())
        //{
        // string query = "SELECT * FROM reservas"; // Asegúrate de que incluye todas las columnas necesarias
        //     return connection.Query<Reserva>(query).AsList();
        //  }
        //}

        public static List<Reserva> ObtenerReservas()
        {
            List<Reserva> reservas = new List<Reserva>();
            string query = "SELECT reservaId, salaId, usuarioId, fechaReserva, horaInicio, horaFin, asistentes, menuSeleccionado FROM reservas";

            using (MySqlConnection connection = new Conexion().GetConnection())
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var reserva = new Reserva
                    {
                        ReservaId = reader.GetInt32("reservaId"),
                        SalaId = reader.GetInt32("salaId"),
                        UsuarioId = reader.GetInt32("usuarioId"),
                        Fecha = reader.GetDateTime("fechaReserva"),
                        HoraInicio = reader.GetTimeSpan("horaInicio"),
                        HoraFin = reader.GetTimeSpan("horaFin"),
                        NumeroAsistentes = reader.GetInt32("asistentes"),  // <-- Verifica que el valor sea leído correctamente
                        MenuSeleccionado = reader.GetString("menuSeleccionado")
                    };
                    reservas.Add(reserva);
                }
                reader.Close();
            }
            return reservas;
        }

        // Actualizar reserva
        public void ActualizarReserva()
        {
            using (MySqlConnection connection = conexion.GetConnection())
            {
                string query = @"
                UPDATE reservas 
                SET salaId = @SalaId, 
                    usuarioId = @UsuarioId, 
                    fecha = @Fecha, 
                    horaInicio = @HoraInicio, 
                    horaFin = @HoraFin, 
                    numeroAsistentes = @NumeroAsistentes, 
                    menuSeleccionado = @MenuSeleccionado 
                WHERE reservaId = @ReservaId";
                connection.Execute(query, this);
            }
        }

        // Eliminar reserva
        public void EliminarReserva()
        {
            string connectionString = "server=localhost;database=clave5_grupodetrabajobd;user=root;password=root;port=3306;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta SQL para eliminar la reserva
                    string query = "DELETE FROM reservas WHERE reservaId = @ReservaId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReservaId", this.ReservaId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al eliminar la reserva: {ex.Message}");
                }
            }
        }
    }
}
