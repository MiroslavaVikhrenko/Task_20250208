using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Drawing;

namespace Task_20250213_5
{
    internal class Program
    {
        /*
         У вас есть база данных с таблицей «Employees», которая содержит информацию о сотрудниках, включая их идентификаторы, имена, возраст, должности и зарплаты. 

1) Создайте метод, который позволяет пользователю ввести возраст, 
        а затем отправляет скалярный запрос в базу данных для получения средней зарплаты 
        сотрудников старше указанного возраста, используя параметризацию.

2) Напишите запрос, который возвращает количество сотрудников для каждой должности. Используйте группировку по должностям и агрегатную функцию COUNT() для подсчета числа сотрудников на каждой должности.

3) Напишите запрос, который для каждого возраста находит максимальную зарплату среди всех сотрудников этого возраста. Используйте группировку по возрасту и агрегатную функцию MAX() для нахождения максимальной зарплаты.

4) Напишите запрос, который выполняет удаление сотрудника по его идентификатору. При выполнении запроса, используйте параметризацию.

5) Напишите запрос, который выполняет обновление зарплаты для сотрудников, идентификаторы которых входят в список переданных. При выполнении запроса, используйте параметризацию.
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

            GetAvgSalaryByAge(connectionString, 25);
            Console.WriteLine("\n**************\n");
            GroupByPosition(connectionString);
            Console.WriteLine("\n**************\n");
            FindMaxSalaryByAge(connectionString);
            Console.WriteLine("\n**************\n");
            FindById(connectionString, 33);
            Console.WriteLine("\n**************\n");
            IncreaseSalaryById(connectionString, 1);
            IncreaseSalaryById(connectionString, 5);
            IncreaseSalaryById(connectionString, 10);

        }

        static void IncreaseSalaryById(string connectionString, int id)
        {
            // Напишите запрос, который выполняет обновление зарплаты для сотрудников,
            // идентификаторы которых входят в список переданных.
            // При выполнении запроса, используйте параметризацию.

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    UPDATE [Employees]  
                    SET Salary = Salary + 100
                    WHERE Id = @id
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@id", id));
                command.ExecuteNonQuery();
                Console.WriteLine($"Salary for Employee with id = {id} has been increased by 100 CAD");
            }
        }

        static void FindById(string connectionString, int id)
        {
            //Напишите запрос, который выполняет удаление сотрудника по его идентификатору.
            //При выполнении запроса, используйте параметризацию.

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    DELETE FROM [Employees] WHERE Id = @id
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@id", id));
                command.ExecuteNonQuery();
                Console.WriteLine($"Employee with id = {id} has been deleted");
            }
        }

        static void FindMaxSalaryByAge(string connectionString)
        {
            //Напишите запрос, который для каждого возраста находит максимальную зарплату
            //среди всех сотрудников этого возраста. Используйте группировку по возрасту
            //и агрегатную функцию MAX() для нахождения максимальной зарплаты.

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT Age, MAX(Salary) AS 'Max salary' FROM Employees GROUP BY Age ORDER BY Age
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                //object avg = command.ExecuteScalar();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        Console.WriteLine($"> {columnName1} | {columnName2} ");
                        while (reader.Read())
                        {
                            int age = reader.GetInt32(0);
                            decimal max = reader.GetDecimal(1);

                            Console.WriteLine($"> {age} y.o. | {max} CAD");
                        }
                    }
                }

            }
        }
        static void GroupByPosition(string connectionString)
        {
            //Напишите запрос, который возвращает количество сотрудников для каждой должности.
            //Используйте группировку по должностям и агрегатную функцию COUNT()
            //для подсчета числа сотрудников на каждой должности.

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT Position, COUNT(Id) AS 'Number of employees' FROM Employees GROUP BY Position ORDER BY Position
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                //object avg = command.ExecuteScalar();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        Console.WriteLine($"> {columnName1} | {columnName2} ");
                        while (reader.Read())
                        {
                            string position = reader.GetString(0);
                            int num = reader.GetInt32(1);

                            Console.WriteLine($"> {position} | {num} ppl");
                        }
                    }
                }

            }
        }

        static void GetAvgSalaryByAge(string connectionString, int age)
        {
            //Создайте метод, который позволяет пользователю ввести возраст, 
            //а затем отправляет скалярный запрос в базу данных для получения средней зарплаты
        //сотрудников старше указанного возраста, используя параметризацию.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT AVG(Salary) FROM Employees WHERE Age > @age
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@age", age));
                object avg= command.ExecuteScalar();

                Console.WriteLine($"Avg salary for employees with age > {age} is {avg}");

            }
        }
    }
}
