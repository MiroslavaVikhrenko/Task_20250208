using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.VisualBasic;
using System.Reflection;
using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Task_20250208
{
    internal class Program
    {
        static string connectionString = "Data Source=MIRUAHUA;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        static string connectionString2 = "Data Source=MIRUAHUA;Initial Catalog=Cars;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        static void Main(string[] args)
        {
            CreateDatabase();
            CreateTable();

            List<Car> cars = new List<Car>()
            {
                new Car(1, "Toyota1", 2016),
                new Car(2, "Honda1", 2019),
                new Car(3, "Mitsubishi1", 2017),
                new Car(4, "Subaru1", 2015),
                new Car(5, "Toyota2", 2020),
                new Car(6, "Toyota3", 2019),
                new Car(7, "Honda2", 2016),
                new Car(8, "Mitsubishi2", 2022),
                new Car(9, "Mitsubishi2", 2016),
                new Car(10, "Toyota4", 2016)
            };

            PopulateTable(cars);
            List<Car> cars2 = GetCarsByYear(2018);
            Console.ReadKey();
        }
        static void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("CREATE DATABASE Cars", connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Database created");
            }
        }
        static void CreateTable()
        {
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                connection.Open();
                string commandtext = $"""
                    CREATE TABLE [Car]
                    (
                        Id int PRIMARY KEY,
                        Model varchar(60) NOT NULL,
                        Year int NOT NULL
                    )
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Table created");
            }
        }

        static void PopulateTable(List<Car> cars)
        {
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                connection.Open();

                foreach (var car in cars)
                {
                    string commandtext = $"""
                    INSERT INTO [Car] (Id, Model, Year)
                    VALUES ({car.Id}, '{car.Model}', {car.Year} )
                    """;
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    command.ExecuteNonQuery();
                }
                
                Console.WriteLine("Table populated");
            }
        }

        static List<Car> GetCarsByYear(int search)
        {
            List <Car> cars = new List <Car>();
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                connection.Open();

                string commandtext = $"""
                    SELECT Id, Model, Year FROM [Car]
                    WHERE Year > {search}
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                //command.ExecuteNonQuery();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        string columnName3 = reader.GetName(2);
                        Console.WriteLine($"{columnName1} | {columnName2} | {columnName3}");

                        while (reader.Read())
                        {
                            object id = reader.GetValue(0);
                            object model = reader.GetValue(1);
                            object year = reader.GetValue(2);
                            Console.WriteLine($"{id} | {model} | {year}");
                            cars.Add(new Car(Convert.ToInt32(id), model.ToString(), Convert.ToInt32(year)));
                        }
                    }
                }

                Console.WriteLine("Table populated");
            }

            return cars;
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        public Car(int id, string model, int year)
        {
            Id = id; Model = model; Year = year;
        }
    }
}
