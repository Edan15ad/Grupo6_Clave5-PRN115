using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Clave5_Grupo6
{
    public class Busqueda
    {
        // Propiedades de la clase
        public int Id { get; set; }            // ID de la reserva
        public int SalaId { get; set; }         // ID de la sala
        public int UsuarioId { get; set; }      // ID del usuario
        public string NombreUsuario { get; set; } // Nombre del usuario
        public string FechaReserva { get; set; } // Fecha de la reserva
        public string HoraInicio { get; set; }  // Hora de inicio de la reserva
        public string HoraFin { get; set; }     // Hora de fin de la reserva
        public int Asistentes { get; set; }     // Número de asistentes
        public string Menu { get; set; }        // Menú seleccionado
        public int PrecioTotal { get; set; }    // Precio total de la reserva

        // Constructor original (sin nombre de usuario)
        public Busqueda(int id, int salaId, int usuarioId, string fechaReserva, string horaInicio, string horaFin, int asistentes, string menuSeleccionado, int precioTotal)
        {
            Id = id;
            SalaId = salaId;
            UsuarioId = usuarioId;
            NombreUsuario = null; // Valor nulo por defecto cuando no se proporciona nombre de usuario
            FechaReserva = fechaReserva;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Asistentes = asistentes;
            Menu = menuSeleccionado;
            PrecioTotal = precioTotal;
        }

        // Nuevo constructor que acepta el nombre del usuario además del ID
        public Busqueda(int id, int salaId, int usuarioId, string usuarioNombre, string fechaReserva, string horaInicio, string horaFin, int asistentes, string menuSeleccionado, int precioTotal)
        {
            Id = id;
            SalaId = salaId;
            UsuarioId = usuarioId;
            NombreUsuario = usuarioNombre; // Asigna el nombre del usuario
            FechaReserva = fechaReserva;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Asistentes = asistentes;
            Menu = menuSeleccionado;
            PrecioTotal = precioTotal;
        }
    }
}
