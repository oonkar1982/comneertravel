using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTravel.API.Helpers
{
    public class DbHelper
    {
        public static SqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["UserDBConnection"].ConnectionString;
            return new SqlConnection(connStr);
        }
    }
}