using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Wam.Service
{
  public  class DBHelper
    {
        public static MySqlConnection GetConn() {

            MySqlConnection conn = new MySqlConnection("Server=localhost;Port=3306;DataBase=world;user=root;pwd=yzw@123;Allow User Variables=True;");
            conn.Open();
            return conn;
        }

    }


}
