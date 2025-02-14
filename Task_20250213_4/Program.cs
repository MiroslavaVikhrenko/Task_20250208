using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Task_20250213_4
{
    internal class Program
    {
        /*
          Создать класс «Студент», с полями: Id, Fio, Age, AverageMark. 
        Создать таблицу в базе данных, заполнить. 
        Используя получение скалярных значений, выполнить следующие действия:

Получить средний балл всех учеников.
Получить количество учеников.
Получить минимальную и максимальную оценку среди учеников.
Получить сумму оценок всего класса.
Считать всех учеников в коллекцию и вывести на экран.
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

                SqlCommand command = new SqlCommand("SELECT AVG(AverageMark) FROM Student", connection);
                object avg = command.ExecuteScalar();
                Console.WriteLine($"Avg grade is {avg}");

                command.CommandText = "SELECT COUNT(Id) FROM Student";
                object count = command.ExecuteScalar();
                Console.WriteLine($"Total: {count} students");

                command.CommandText = "SELECT MIN(AverageMark) FROM Student";
                object min = command.ExecuteScalar();
                Console.WriteLine($"Min grade is {min}");

                command.CommandText = "SELECT MAX(AverageMark) FROM Student";
                object max = command.ExecuteScalar();
                Console.WriteLine($"Max grade is {max}");

                command.CommandText = "SELECT SUM(AverageMark) FROM Student";
                object sum = command.ExecuteScalar();
                Console.WriteLine($"Sum grade is {sum}");

                List<Student> students = GetStudents(connectionString);

                foreach (Student student in students)
                {
                    Console.WriteLine($"{student.Id} | {student.Fio} | {student.Age} | {student.AverageMark}");
                }
            }
        }

        static List<Student> GetStudents(string connectionString)
        {
            List<Student> students = new List<Student>();

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandText = $"""
                    SELECT Id, Fio, Age, AverageMark
                    FROM [Student]
                    """;
                SqlCommand command = new SqlCommand(commandText, connection);

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
                            int id = reader.GetInt32(0);
                            string fio = reader.GetString(1);
                            int age = reader.GetInt32(2);
                            int avgmark = reader.GetInt32(3);

                            students.Add(new Student(id, fio, age, avgmark));  
                        }
                    }
                }

            }
            return students;
        }
    }


    public class Student
    {
        public int Id { get; set; }
        public string Fio { get; set; }
        public int Age { get; set; }
        public int AverageMark { get; set; }

        public Student(int id, string fio, int age, int avgmark)
        {
            Id = id;
            Fio = fio;
            Age = age;
            AverageMark = avgmark;
        }
    }
}
