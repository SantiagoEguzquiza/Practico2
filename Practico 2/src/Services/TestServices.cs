

using Common;
using System;
using System.Data.SqlClient;

namespace Services
{
    public class TestServices
    {
        public static void TestConnection()
        {
            try
            {
                using (var context = new SqlConnection(Parameters.ConnectionString))
                {
                    context.Open();
                    Console.WriteLine("Sql Connection Succesfull");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Sql Server: {ex.Message} ");
            }



        }
    }
}
