using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clave5_Grupo6;
using MySql.Data.MySqlClient;
using Dapper;

namespace Clave5_Grupo6
{

        public class Usuario
        {
            public int UsuarioId { get; set; }
            public string NombreUsuario { get; set; }

            private Conexion conexion = new Conexion();

            // Crear usuario
            public void AgregarUsuario()
            {
                using (MySqlConnection connection = conexion.GetConnection())
                {
                    string query = "INSERT INTO usuarios (nombreUsuario) VALUES (@NombreUsuario);";
                    connection.Execute(query, this);
                }
            }

            // Buscar usuario por nombre
            public static Usuario BuscarUsuario(string nombre)
            {
                using (MySqlConnection connection = new Conexion().GetConnection())
                {
                    string query = "SELECT * FROM usuarios WHERE nombreUsuario = @Nombre LIMIT 1;";
                    return connection.QueryFirstOrDefault<Usuario>(query, new { Nombre = nombre });
                }
            }
        }
    }
