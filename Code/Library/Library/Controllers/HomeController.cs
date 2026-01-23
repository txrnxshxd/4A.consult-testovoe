using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ActionResult<BooksPagination>> Index(int page = 1, int pageSize = 10)
        {
            ViewBag.Page = page;

            string sqlExpression = "dbo.sp_SelectMany";

            List<Book> books = new List<Book>();
            BooksPagination booksWithPagination = new();

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                SqlCommand command = new(sqlExpression, connection);


                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Page", page);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        booksWithPagination.Pages = Math.Ceiling(Convert.ToDouble(reader.GetInt32(0)) / pageSize);
                    }


                    if (await reader.NextResultAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string xmlData = reader.GetString(0);

                            XDocument xdoc = XDocument.Parse(xmlData);

                            foreach (var book in xdoc.Root.Elements("Book"))
                            {
                                books.Add(new Book()
                                {
                                    Id = (int?)book.Element("Id") ?? 0,
                                    Name = (string?)book.Element("Name") ?? string.Empty,
                                    Author = (string?)book.Element("Author") ?? string.Empty,
                                    Year = (int?)book.Element("Year") ?? 0,
                                    Category = (string?)book.Element("Category") ?? string.Empty,
                                });
                            }
                        }

                        booksWithPagination.Books = books;
                    }

                    return View(booksWithPagination);
                }

            }

        }

        public async Task<ActionResult<BookDetails>> Details(int id)
        {
            try
            {
                return View(await selectOneBook(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public IActionResult Insert()
        {
            return View();
        }

        public async Task<ActionResult<BookDetails>> Edit(int id)
        {

            ViewBag.Id = id;

            try
            {
                return View(await selectOneBook(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookDetails>> EditPOST(BookDetails book)
        {
            string sqlExpression = "dbo.sp_EditBook";

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                SqlCommand command = new(sqlExpression, connection);


                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", book.Id);
                command.Parameters.AddWithValue("@Name", book.Name);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@Year", book.Year);
                command.Parameters.AddWithValue("@Category", book.Category);
                command.Parameters.AddWithValue("@Contents", book.Contents);

                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public async Task<ActionResult<BookDetails>> InsertPOST(BookDetails book)
        {
            string sqlExpression = "dbo.sp_InsertBook";

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                SqlCommand command = new(sqlExpression, connection);

                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", book.Name);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@Year", book.Year);
                command.Parameters.AddWithValue("@Category", book.Category);
                command.Parameters.AddWithValue("@Contents", book.Contents);

                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Index");
            }
            ;
        }

        public async Task<ActionResult> Delete(int id)
        {
            string sqlExpression = "dbo.sp_DeleteBook";

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                SqlCommand command = new(sqlExpression, connection);

                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("Index");
                }
            }
            ;
        }

        public async Task<BookDetails> selectOneBook(int id)
        {
            string sqlExpression = "dbo.sp_SelectOne";

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                SqlCommand command = new(sqlExpression, connection);

                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader.HasRows)
                        {
                            return new BookDetails()
                            {
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Author = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                Year = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : null,
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : string.Empty,
                                Contents = reader["Contents"] != DBNull.Value ? reader["Contents"].ToString() : string.Empty
                            };
                        }
                    }
                }

                throw new Exception(message: $" нига с Id: {id} не найдена");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
