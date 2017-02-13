using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Entity;
using System.Data;

namespace Wam.Service
{
  public  class UserService
    {
        public static IEnumerable<User> GetUserList() {

            using (IDbConnection conn= DBHelper.GetConn())
            {
                return conn.Query<User>("select * from user");
            }
        }

        private static IEnumerable<User> GetUserList2()
        {

            using (IDbConnection conn = DBHelper.GetConn())
            {
                return conn.Query<User>("select * from user");
            }
        }

    }
}
