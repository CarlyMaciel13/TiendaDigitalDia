using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TiendaDigitalDia.Context
{
    public class TiendaContext
    {
        public SqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["TiendaDB"].ConnectionString;
            return new SqlConnection(connStr);
        }
    }
}
