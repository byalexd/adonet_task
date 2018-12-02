using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;

namespace adonet_practice
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        static void Main(string[] args)
        {
            Console.WriteLine("Get trip name");
            string tripname = Console.ReadLine();
            Console.WriteLine("IsDone true/false");
            bool isdone = bool.Parse(Console.ReadLine());
            Console.WriteLine("add userinfoid");
            int userinfoid = Int32.Parse(Console.ReadLine());

            AddTrip(tripname, isdone, userinfoid);
            GetTrips();
            ReturnScheme();
            Console.ReadKey();
        }
        public static void GetTrips()
        {
            string sqlExpressions = "Select * from Trip";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressions, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine($"{reader.GetName(0)}\t{reader.GetName(1)}\t{reader.GetName(2)}\n");
                    while (reader.Read())
                    {
                        object id = reader.GetValue(0);
                        object name = reader.GetValue(1);
                        object isdone = reader.GetValue(2);

                        Console.WriteLine($"{id}\t{name}\t{isdone}");
                    }

                }
                reader.Close();
            }
        }

        public static void AddTrip(string tripname, bool isdone, int userinfo)
        {
            string sqlExpression = "addtrips";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command  = new SqlCommand(sqlExpression, connection);

                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = tripname
                };

                command.Parameters.Add(nameParam);

                SqlParameter isdoneParam = new SqlParameter
                {
                    ParameterName = "@isdone",
                    Value = isdone
                };

                command.Parameters.Add(isdoneParam);

                SqlParameter userinfoid = new SqlParameter
                {
                    ParameterName = "@userinfoid",
                    Value = userinfo
                };

                command.Parameters.Add(userinfoid);

                var result = command.ExecuteNonQuery();

                Console.WriteLine($"Returned id is {result}");
            }
        }

        public static void ReturnScheme()
        {
            var dbreader = new DatabaseReader(connectionString, SqlType.SqlServer);
            var schema = dbreader.ReadAll();

            foreach (var table in schema.Tables)
            {
                Console.WriteLine("Table" + table.Name);

                foreach (var column in table.Columns)
                {
                    Console.Write("\tColumn " + column.Name + "\t" + column.DataType.TypeName);
                    if (column.DataType.IsString) Console.Write("(" + column.Length + ")");
                    if (column.IsPrimaryKey) Console.Write("\tPrimary key");
                    if (column.IsForeignKey) Console.Write("\tForeign key to " + column.ForeignKeyTable.Name);
                    Console.WriteLine("");
                }
            }
        }
    }
}
