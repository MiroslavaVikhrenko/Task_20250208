using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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

        }

        static void GetAvgSalaryByAge(string connectionString, int age)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    string sql = $"SELECT AVG(Salary) FROM Employees ";
                }
            }
        }
    }
}
