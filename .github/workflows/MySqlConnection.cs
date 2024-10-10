using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Clave5_Grupo6;

namespace Clave5_Grupo6
{

    public class Conexion
    {
        private string connectionString = "server=localhost;database=clave5_grupodetrabajobd;user=root;password=root;port=3306;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
   


