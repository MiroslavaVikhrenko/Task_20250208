using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Task_20250216_7
{
    internal class Program
    {
        /*
         Определите таблицу «Записи», минимум 5 колонок. Определите хранимые следующие процедуры:

1) Для вставки новой записи в таблицу.
2) Для редактирования записи по Id. 
3) Для получения записи по Id. 

Выполните вставку 3 записей. Выполните получение любой из записей.
         */
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connection opened");
            }

            CreatedSPs(connectionString);
            AddRecord(connectionString, "study", "practice C#", "sql", "good");
            AddRecord(connectionString, "study", "practice SQL", "sp", "good");
            AddRecord(connectionString, "study", "practice Azure", "db", "good");

            UpdateMoodById(connectionString, "bad", 1);

            GetRecordById(connectionString, 1);
        }

        static void GetRecordById(string connectionString, int id)
        {
            string sqlExpression = "sp_GetRecordsById";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@id", id));
                //command.ExecuteNonQuery();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine($"{reader.GetName(0)} | {reader.GetName(1)} | {reader.GetName(2)} |" +
                            $" {reader.GetName(3)} | {reader.GetName(4)}");
                        while (reader.Read())
                        {
                            object obj1 = reader.GetValue(0);
                            object obj2 = reader.GetValue(1);
                            object obj3 = reader.GetValue(2);
                            object obj4 = reader.GetValue(3);
                            object obj5 = reader.GetValue(4);
                            Console.WriteLine($"{obj1} | {obj2} | {obj3} | {obj4} | {obj5}");
                        }
                    }
                }
            }
        }
        static void UpdateMoodById(string connectionString, string updatedMood, int id)
        {
            string sqlExpression = "sp_UpdateRecordsById";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@updatedMood", updatedMood));
                command.Parameters.Add(new SqlParameter("@id", id));
                command.ExecuteNonQuery();
                Console.WriteLine($"Record updated");
            }
        }

        static void AddRecord(string connectionString, string area, string description, string note, string mood)
        {
            string sqlExpression = "sp_InsertRecord";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@area", area));
                command.Parameters.Add(new SqlParameter("@description", description));
                command.Parameters.Add(new SqlParameter("@note", note));
                command.Parameters.Add(new SqlParameter("@mood", mood));
                command.ExecuteNonQuery();
                Console.WriteLine($"Record added");
            }
        }

        static void CreatedSPs(string connectionString)
        {
            string proc1 = @"CREATE PROCEDURE [dbo].[sp_InsertRecord]
                                @area NVARCHAR(30),
                                @description NVARCHAR(80),
                                @note NVARCHAR(30),
                                @mood NVARCHAR(30)

                            AS
                                INSERT INTO Records (Area, [Description], [Note], [Mood])
                                VALUES (@area, @description, @note, @mood)
   
                                SELECT SCOPE_IDENTITY()
                            GO";

            string proc2 = @"CREATE PROCEDURE [dbo].[sp_UpdateRecordsById]
                                  @updatedMood NVARCHAR(30) out,
                                  @id INT out
                                AS                               
                                    UPDATE Records SET Mood = @updatedMood WHERE Id = @id";
            string proc3 = @"CREATE PROCEDURE [dbo].[sp_GetRecordsById]
                                    @id INT out
                                AS
                                    SELECT * FROM Records WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(proc1, connection);
                command.ExecuteNonQuery();
                command.CommandText = proc2;
                command.ExecuteNonQuery();
                command.CommandText = proc3;
                command.ExecuteNonQuery();
                Console.WriteLine("Stored procedures added to db");
            }
        }
    }
}
