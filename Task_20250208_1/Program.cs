using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Task_20250208_1
/*
         Создайте базу данных «Shop». В ней создайте таблицу «Phones» (опишите на основе класса): 
        Id (Identity), Manufacturer, Model, Year, Price. 

Определите следующие методы для выполнения запросов:

1) Метод заполнения данными. Добавить минимум 3 телефона (заполнение базы данных выполнить через List объектов класса). 
2) Метод считывания данных. Добавить в новую коллекцию все содержимое таблицы, используя типизацию результатов, после вывести на экран.
3) Метод удаления по идентификатору. Выполнить удаление рядка из таблицы по его первичному ключу.
4) Метод редактирования по идентификатору. Выполнить редактирования всех столбцов таблицы определенного рядка по его первичному ключу.
         */
{
    internal class Program
    {
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

            CreateDatabase(connectionString);

            string connectionString2 = configuration.GetConnectionString("Shop");

            CreateTable(connectionString2);

            List<Phone> phones = new List<Phone>()
            {
                new Phone(1, "iPhone", "16 Pro Max", 2024, 1200),
                new Phone(2, "iPhone", "15 Pro Max", 2023, 1000),
                new Phone(3, "iPhone", "15 Plus", 2023, 1100),
                new Phone(4, "iPhone", "14 Pro Max", 2022, 900),
                new Phone(5, "iPhone", "SE", 2022, 1000),
                new Phone(6, "iPhone", "13 Pro Mini", 2021, 1000),
                new Phone(7, "Samsung", "Galaxy S24", 2024, 1100),
                new Phone(8, "Samsung", "Galaxy A05", 2023, 1050),
                new Phone(9, "Samsung", "Galaxy M35 5G", 2024, 1150),
                new Phone(10, "Samsung", "A55 5G", 2024, 1100)
            };

            PopulateTable(connectionString2, phones);

            Console.ReadKey();
            
        }

        static void PopulateTable(string connectionString, List<Phone> phones)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var phone in phones)
                {
                    string commandtext = $"""
                    INSERT INTO [Phones] (Id, Manufacturer, Model, Year, Price)
                    VALUES ({phone.Id},'{phone.Manufacturer}', '{phone.Model}', {phone.Year}, {phone.Price})
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Table populated");
            }
        }

        static void CreateTable(string connectionString)
        {
            //В ней создайте таблицу «Phones» (опишите на основе класса): 
            //Id(Identity), Manufacturer, Model, Year, Price.

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    CREATE TABLE [Phones]
                    (
                        Id int PRIMARY KEY,
                        Manufacturer nvarchar(60) NOT NULL,
                        Model nvarchar(60) NOT NULL,
                        Year int NOT NULL,
                        Price int NOT NULL
                    )
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Table created");
            }
        }

        static void CreateDatabase(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("CREATE DATABASE Shop", connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Database created");
            }
        }
    }

    public class Phone
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Price { get; set; }

        public Phone(int id, string manufacturer, string model, int year, int price)
        {
            Id = id; Manufacturer = manufacturer; Model = model; Year = year; Price = price;
        }
    }
}
