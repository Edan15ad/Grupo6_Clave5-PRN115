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
        public int Id { get; set; }
        public int SalaId { get; set; }
        public int UsuarioId { get; set; }
        public string FechaReserva { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int Asistentes { get; set; }
        public string Menu { get; set; }
        public int PrecioTotal { get; set; }


        public Busqueda(int id, int salaId, int usuarioId, string fechaReserva, string horaInicio, string horaFin, int asistentes, string menuSeleccionado, int precioTotal)
        {
            Id = id;
            SalaId = salaId;
            UsuarioId = usuarioId;
            FechaReserva = fechaReserva;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Asistentes = asistentes;
            Menu = menuSeleccionado;
            PrecioTotal = precioTotal;

        }
    }
}
