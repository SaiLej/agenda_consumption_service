using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Data.OleDb;
using System.Xml.Serialization;

namespace AgendaPingoAPI
{
    class Program
    {
        static void Main(string[] args)
        {

            //Data transaction for all years
            //SqlConnection my_conn = new SqlConnection("server=172.16.2.81; database=Eg Elo;Integrated Security=False;User ID=keepfocussql;Password=Xeigai3O;");
            SqlConnection my_conn = new SqlConnection("server=localhost; database=Eg Elo;Integrated Security=SSPI");
            SqlCommand command = my_conn.CreateCommand();
            my_conn.Open();

            string reset_year = DateTime.Now.Year.ToString() + "-01" + "-01";
            System.Data.SqlClient.SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("SELECT COUNT(*) FROM [Eg Elo].[dbo].[EGWeb_Use_Month] WHERE Date < '" + reset_year + "';", my_conn);
            var result = sqlCommand.ExecuteScalar();
            my_conn.Close();

            if (Convert.ToInt32(result) == 0)
            {
                for (int year_start = 2000; year_start <= DateTime.Now.Year; year_start++)
                {
                    AgendaApiClient.GetMonthlyReadings(year_start.ToString());
                }
            }
            else
            {
                AgendaApiClient.GetMonthlyReadings();
            }
        }
    }
}
