using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics.Contracts;

namespace Task_20250216_6
{
    internal class Program
    {
        /*
         Реализовать добавление товара в таблицу «Products». 
        Сразу же после добавления, получить идентификатор товара и добавить его в коллекцию 
        List<Products>. Использовать выходной параметр и функцию SCOPE_IDENTITY().
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

            List<Product> products = new List<Product>();
            products.Add(AddNewProduct(connectionString, "table", 100));
            products.Add(AddNewProduct(connectionString, "chair", 80));
            products.Add(AddNewProduct(connectionString, "cabinet", 200));
            products.Add(AddNewProduct(connectionString, "bed", 250));

            PrintProducts(products);
        }

        static void PrintProducts(List<Product> products)
        {
            Console.WriteLine("All products:");
            foreach (Product product in products)
            {
                Console.WriteLine(product.ToString());
            }
        }

        static Product AddNewProduct(string connectionString, string name, int price)
        {
            string sqlExpression = "INSERT INTO Products ([Name], Price) VALUES (@name, @price);" +
                "SET @id=SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@price", price));
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output 
                };
                command.Parameters.Add(idParam);
                command.ExecuteNonQuery();
                Console.WriteLine($"New productId: {idParam.Value}");
                return new Product { Name = name, Price = price, ProductId = (int)idParam.Value };
            }
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int ProductId { get; set; }
        public override string ToString() 
        {
            return $"ID {ProductId}, Name: {Name}, Price: {Price} CAD";
        }
    }
}
