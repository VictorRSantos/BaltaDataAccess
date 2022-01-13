using System;
using System.Collections.Generic;
using System.Data;
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
            Console.Clear();

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

                // CreateCategories(connection);

                //CreateManyCategories(connection);

                //UpdateCategories(connection);

                //ListCategories(connection);


                //ExecuteProcedure(connection);

                //ExecuteGetCoursesByCategory(connection);



                // Execute Scalar

                //ExecuteScalar(connection);

                //ReadView(connection);


                // Mapeamento
                //OneToOne(connection);

                //OneToMany(connection);

                //QueryMultiple(connection);

                //SelectIn(connection);

                //Like(connection, "backend");

                Transaction(connection);

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



        static void CreateManyCategories(SqlConnection connection)
        {

            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;


            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "categoria-nova";
            category2.Description = "Categoria Nova";
            category2.Order = 9;
            category2.Summary = "Categoria";
            category2.Featured = true;



            var insertSql = @"INSERT INTO Category VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";


            var rows = connection.Execute(insertSql, new[] {
                new{

                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured

            },
             new{

                category2.Id,
                category2.Title,
                category2.Url,
                category2.Summary,
                category2.Order,
                category2.Description,
                category2.Featured
            },
               });


            System.Console.WriteLine($"{rows} linhas inseridas");
        }

        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "[spDeleteStudent]";

            var pars = new { StudentId = "2d737a54-d587-4d2d-ac60-21cb494c3a16" };

            var affectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);

            System.Console.WriteLine(affectedRows);
        }


        static void ExecuteGetCoursesByCategory(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";
            var pars = new { CategoryId = "af3407aa-11ae-4621-a2ef-2028b85507c4" };

            var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);

            foreach (var item in courses)
            {

                System.Console.WriteLine($"{item.Id} linhas afetadas");

            }
        }


        static void ExecuteScalar(SqlConnection connection)
        {

            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;


            // OBS: SELECT SCOPR_IDENTITY() => Só funciona se o campo foi Identity

            var insertSql = @"INSERT INTO Category OUTPUT INSERTED.[Id] VALUES (NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";


            var id = connection.ExecuteScalar<Guid>(insertSql, new { category.Title, category.Url, category.Summary, category.Order, category.Description, category.Featured });

            System.Console.WriteLine($"A categoria inserida foi: {id}");

        }


        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM [vwCourses]";

            var courses = connection.Query(sql);

            foreach (var item in courses)
            {
                System.Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = @"SELECT * FROM CareerItem INNER JOIN Course ON CareerItem.CourseId = Course.Id";

            var items = connection.Query<CareerItem, Course, CareerItem>(sql, (careerItem, course) =>
             {
                 careerItem.Course = course;

                 return careerItem;

             }, splitOn: "Id");

            foreach (var item in items)
            {
                System.Console.WriteLine($"{item.Title} - {item.Course.Title}");

            }
        }

        static void OneToMany(SqlConnection connection)
        {
            var sql = @"
                SELECT 
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [CareerItem].[Title]
                FROM 
                    [Career] 
                INNER JOIN 
                    [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                ORDER BY
                    [Career].[Title]";

            var careers = new List<Career>();

            var items = connection.Query<Career, CareerItem, Career>(sql, (career, item) =>
            {

                var car = careers.FirstOrDefault(x => x.Id == career.Id);

                if (car == null)
                {
                    car = career;
                    car.Items.Add(item);
                    careers.Add(car);
                }
                else
                {
                    car.Items.Add(item);

                }

                return career;

            }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                System.Console.WriteLine($"{career.Title}");

                foreach (var item in career.Items)
                {
                    System.Console.WriteLine($" - {item.Title}");
                }
            }
        }


        static void QueryMultiple(SqlConnection connection)
        {
            var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

            using (var multi = connection.QueryMultiple(query))
            {
                var categories = multi.Read<Category>();
                var courses = multi.Read<Course>();

                foreach (var item in categories)
                {
                    System.Console.WriteLine(item.Title);
                }


                foreach (var item in courses)
                {
                    System.Console.WriteLine(item.Title);
                }

            }







        }


        static void SelectIn(SqlConnection connection)
        {
            var query = @"select * from Career where [Id] IN @Id";

            var items = connection.Query<Career>(query, new
            {
                Id = new[]{
                    "4327ac7e-963b-4893-9f31-9a3b28a4e72b",
                    "e6730d1c-6870-4df3-ae68-438624e04c72"
                }
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }

        }


        static void Like(SqlConnection connection, string term)
        {
            var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";

            var items = connection.Query<Course>(query, new
            {
                exp = $"%{term}%"
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }
        }


        static void Transaction(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Minha categoria que não";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id, 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured)";

            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var rows = connection.Execute(insertSql, new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                }, transaction);

                transaction.Commit();
                // transaction.Rollback();

                Console.WriteLine($"{rows} linhas inseridas");
            }

        }
    }
}