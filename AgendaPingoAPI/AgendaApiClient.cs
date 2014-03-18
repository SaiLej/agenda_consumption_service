using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Data.SqlClient;

namespace AgendaPingoAPI
{
    public static class AgendaApiClient
    {
        public static int GetMonthlyReadings(string readingYear = null)
        {
            string executionTime = "Started: " + DateTime.Now.ToShortTimeString();
            string errorMessage = "";
            int year;
            int this_month = 12;
            if (readingYear == null)
            {
                year = DateTime.Now.Year;
                this_month = DateTime.Now.Month;
            }
            else
            {
                year = Convert.ToInt16(readingYear);
            }             

            // 1.Get meterlist (call service)
            string content = "";
            try
            {
                var meter_list = "http://api.keepfocus.dk/api1/meters/list.json?apikey=PpN5tzd7cu1qfLxQWKXCafxSCJSv8QbkJSZwW0EJ8gEot6wU9z";
                var syncClient = new WebClient();
                content = syncClient.DownloadString(meter_list);
            }
            catch (Exception e)
            {
                errorMessage = "Error calling service. Cannot load meters; " + e.Message;
                return -1;
            }
            
            // Create the Json serializer and parse the response
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            {
                var meter_units = (RootObject)serializer.ReadObject(ms);
                var meter_ids = from results in meter_units.data select results.meter_id;

                SqlConnection my_conn = new SqlConnection("server=localhost; database=Eg Ello;Integrated Security=SSPI");
                SqlCommand command = my_conn.CreateCommand();
                my_conn.Open();

                foreach (int current_meter_id in meter_ids)
                {

                    var monthly_consumption = "http://api.keepfocus.dk/api1/meters/"+current_meter_id+"/monthly_consumption/"+year+".json?apikey=PpN5tzd7cu1qfLxQWKXCafxSCJSv8QbkJSZwW0EJ8gEot6wU9z";
                    var syncClient_rest = new WebClient();
                    var content_rest = syncClient_rest.DownloadString(monthly_consumption);
                    DataContractJsonSerializer serializer_rest = new DataContractJsonSerializer(typeof(PingoData));
                    using (var loop = new MemoryStream(Encoding.Unicode.GetBytes(content_rest)))
                    {
                        var rest_data = (PingoData)serializer_rest.ReadObject(loop);
                        var month_rest_data = from res in rest_data.data.values select res;

                        string insertComand = "";                        
                        int i = 0;
                        foreach (float value_month in month_rest_data)
                        {
                            i++;
                            if (i <= this_month)
                            {
                                string consumption = value_month.ToString();
                                consumption = consumption.Replace(",", ".");

                                //DateTime readingDate = new DateTime(year, i, 1);
                                string readingDate = year.ToString() + "-" + i.ToString() + "-01";

                                insertComand += "INSERT INTO EGWeb_Use_Month([IDMeter], [Reading], [Use], [Units], [UnitsName], [Scale], [Date]) VALUES (" + current_meter_id + ", NULL , " + consumption + " , NULL, NULL, NULL, '" + readingDate + "');";
                            }
                        }
                        command.CommandText = insertComand;
                        command.ExecuteNonQuery();
                        Console.WriteLine("Inserted data for meter nr {0}.", current_meter_id);      
                    }
                }

                my_conn.Close();
                Console.WriteLine("Data base Connection Closed. Inserted {0} rows.", meter_ids.Count());
                executionTime += "; Ended: " + DateTime.Now.ToShortTimeString();
                Console.WriteLine(executionTime);
            }
            return 0;
        }
    }
}
