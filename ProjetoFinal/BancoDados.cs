using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Horazon_Bank__projetoFinal
{
    public static class Database
    {
     
        public static readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=HorizonBank;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}