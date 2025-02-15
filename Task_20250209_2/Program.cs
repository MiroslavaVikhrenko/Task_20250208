

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Task_20250209_2
{
    internal class Program
    {
        /*
         Создайте приложение, которое позволит пользователю подключиться и отключиться от базы данных «Овощи и фрукты». В случае успешного подключения выводите информационное сообщение. 
        Если подключение было неуспешным, сообщите об ошибке. 
        Приложение может быть консольным или с UI интерфейсом.
         */

        static string connectionString = "Data Source=MIRUAHUA;Initial Catalog=VegetablesFruit;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        static void Main(string[] args)
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //IConfiguration configuration = builder.Build();
            //string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connection opened");
            }

            Combine();
            UpdateTable("veg1", "orange", 200, false);
            //PrintAllInfo();
            //PrintNames();
            //PrintColors();

            //string maxCal = $""" SELECT MAX(Cal) FROM [Items]""";
            //Console.WriteLine($"Max cal is {GetStats(maxCal)} cal");

            //string minCal = $""" SELECT MIN(Cal) FROM [Items]""";
            //Console.WriteLine($"Min cal is {GetStats(minCal)} cal");

            //string avgCal = $""" SELECT AVG(Cal) FROM [Items]""";
            //Console.WriteLine($"Avg cal is {GetStats(avgCal)} cal");

            //string vegNum = $"""SELECT COUNT([Name]) FROM Items WHERE Type = 0""";
            //Console.WriteLine($"There are {GetStats(vegNum)} vegetables");

            //string fruNum = $"""SELECT COUNT([Name]) FROM Items WHERE Type = 1""";
            //Console.WriteLine($"There are {GetStats(fruNum)} fruits");

            //string greenNum = $"""SELECT COUNT([Name]) FROM Items WHERE Color = 'green'""";
            //Console.WriteLine($"There are {GetStats(greenNum)} green items");

            //GetNumbersByColors();

            //string lessThanCal = $"""SELECT [Name], [Color], [Cal], [Type] FROM [Items] WHERE Cal < 100""";
            //Console.WriteLine("\n***************************\n");
            //Console.WriteLine("Items with less than 100 cal:");
            //DisplayByCondition(lessThanCal);
            //Console.WriteLine("\n***************************\n");

            //string moreThanCal = $"""SELECT [Name], [Color], [Cal], [Type] FROM [Items] WHERE Cal > 100""";
            //Console.WriteLine("\n***************************\n");
            //Console.WriteLine("Items with more than 100 cal:");
            //DisplayByCondition(moreThanCal);
            //Console.WriteLine("\n***************************\n");

            //string betweenCal = $"""SELECT [Name], [Color], [Cal], [Type] FROM [Items] WHERE Cal > 40 AND Cal < 200""";
            //Console.WriteLine("\n***************************\n");
            //Console.WriteLine("Items with cal between than 40 and 200:");
            //DisplayByCondition(betweenCal);
            //Console.WriteLine("\n***************************\n");

            //string yellowAndRed = $"""SELECT [Name], [Color], [Cal], [Type] FROM [Items] WHERE Color = 'yellow' OR Color = 'red'""";
            //Console.WriteLine("\n***************************\n");
            //Console.WriteLine("Yellow and red items:");
            //DisplayByCondition(yellowAndRed);
            //Console.WriteLine("\n***************************\n");

            Console.ReadKey();
        }

        static void UpdateTable(string name, string color, int cal, bool type )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    INSERT INTO Items ([Name], Color, Cal, Type)
                    VALUES (@name, @color, @cal, @type)
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@color", color));
                command.Parameters.Add(new SqlParameter("@cal", cal));
                command.Parameters.Add(new SqlParameter("@type", type));
                command.ExecuteNonQuery();
            }
        }

        static void Combine()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT COUNT([Name]) AS 'Count', MIN(Cal) AS 'Min',
                    MAX(Cal) AS 'Max', AVG(Cal) AS 'Avg', SUM(Cal) AS 'Sum'
                    FROM Items 
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        string columnName3 = reader.GetName(2);
                        string columnName4 = reader.GetName(3);
                        string columnName5 = reader.GetName(4);
                        Console.WriteLine($"> {columnName1} | {columnName2} | {columnName3} | {columnName4} | {columnName5}");
                        while (reader.Read())
                        {
                            int count = reader.GetInt32(0);
                            int min = reader.GetInt32(1);
                            int max = reader.GetInt32(2);
                            int avg = reader.GetInt32(3);
                            int sum = reader.GetInt32(4);
                            Console.WriteLine($"> {count} | {min} | {max} | {avg} | {sum}");
                        }
                    }
                }
            }

            static void DisplayByCondition(string commandtext)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            string columnName1 = reader.GetName(0);
                            string columnName2 = reader.GetName(1);
                            string columnName3 = reader.GetName(2);
                            string columnName4 = reader.GetName(3);
                            Console.WriteLine($"> {columnName1} | {columnName2} | {columnName3} | {columnName4}");
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                string color = reader.GetString(1);
                                int cal = reader.GetInt32(2);
                                bool type = reader.GetBoolean(3);
                                if (type)
                                {
                                    Console.WriteLine($"> {name} | {color} | {cal} cal | Type: F");
                                }
                                else
                                {
                                    Console.WriteLine($"> {name} | {color} | {cal} cal | Type: V");
                                }
                            }
                        }
                    }
                }
            }
            static void GetNumbersByColors()
            {
                Console.WriteLine("\n***************************\n");
                Console.WriteLine("All colors by numbers");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string commandtext = $"""
                    SELECT Color, COUNT([Name]) FROM Items GROUP BY Color ORDER BY COUNT([Name]) DESC
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string color = reader.GetString(0);
                                int num = reader.GetInt32(1);
                                Console.WriteLine($"> {color} : {num} items");
                            }
                        }
                    }
                }
                Console.WriteLine("\n***************************\n");
            }
            static int GetStats(string commandtext)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int stats = reader.GetInt32(0);
                                return stats;
                            }
                        }
                    }
                }
                throw new Exception();
            }
            static void PrintColors()
            {
                Console.WriteLine("\n***************************\n");
                Console.WriteLine("All colors");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string commandtext = $"""
                    SELECT [Color] FROM [Items]
                    GROUP BY Color
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string color = reader.GetString(0);
                                Console.WriteLine($"> {color}");
                            }
                        }
                    }
                }
                Console.WriteLine("\n***************************\n");
            }
            static void PrintNames()
            {
                Console.WriteLine("\n***************************\n");
                Console.WriteLine("All names from table");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string commandtext = $"""
                    SELECT [Name], [Type] FROM [Items]
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                bool type = reader.GetBoolean(1);
                                if (type)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine($"> {name} | Type: F");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {name} | Type: V");
                                    Console.ResetColor();
                                }

                            }
                        }
                    }

                }
                Console.WriteLine("\n***************************\n");
            }
            static void PrintAllInfo()
            {
                Console.WriteLine("\n***************************\n");
                Console.WriteLine("All items from table");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string commandtext = $"""
                    SELECT [Name], [Color], [Cal], [Type] FROM [Items]
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            string columnName1 = reader.GetName(0);
                            string columnName2 = reader.GetName(1);
                            string columnName3 = reader.GetName(2);
                            string columnName4 = reader.GetName(3);
                            Console.WriteLine($"> {columnName1} | {columnName2} | {columnName3} | {columnName4}");

                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                string color = reader.GetString(1);
                                int cal = reader.GetInt32(2);
                                bool type = reader.GetBoolean(3);
                                if (type)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine($"> {name} | {color} | {cal} cal | Type: F");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {name} | {color} | {cal} cal | Type: V");
                                    Console.ResetColor();
                                }
                            }
                        }
                    }

                }
                Console.WriteLine("\n***************************\n");
            }
        }
    }
}
