using System;
using System.Linq;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BaltaDataAccess
{

    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=DESKTOP\\SQLEXPRESS;Database=balta;User ID=sa;Password=123456;TrustServerCertificate=True";



            using (var connection = new SqlConnection(connectionString))
            {

                #region Conexao Via ADO.NET
                // Console.WriteLine("Conectado");
                // connection.Open();

                // using (var command = new SqlCommand())
                // {
                //      command.Connection = connection;
                //      command.CommandType = System.Data.CommandType.Text;
                //      command.CommandText = "SELECT Id, Title FROM Category";

                //      var reader = command.ExecuteReader();

                //      while (reader.Read())
                //      {
                //           System.Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
                //     } 


                // }
                #endregion

                UpdateCategories(connection);
                ListCategories(connection);
                // CreateCategories(connection);




            }


        }


        static void ListCategories(SqlConnection connection)
        {
            // SELECT
            var categories = connection.Query<Category>("SELECT Id, Title FROM Category");
            foreach (var item in categories)
            {
                System.Console.WriteLine($"{item.Id} - {item.Title}");
            }

        }


        static void CreateCategories(SqlConnection connection)
        {

            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;



            var insertSql = @"INSERT INTO Category VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";


            var rows = connection.Execute(insertSql, new { category.Id, category.Title, category.Url, category.Summary, category.Order, category.Description, category.Featured });
            System.Console.WriteLine($"{rows} linhas inseridas");
        }


        static void UpdateCategories(SqlConnection connection)
        {

            var updateQuery = @"UPDATE Category SET Title = @Title Where Id = @Id";

            var rows = connection.Execute(updateQuery, new { Id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"), title = "Frontend 2021" });

            System.Console.WriteLine($"{rows} registros atualizados");

        }
    }
}
