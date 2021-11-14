using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.DataAccess;
using FowlerSite.Models;
using FowlerSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FowlerSite.Controllers
{
    public class UsersController : Controller
    {
        /// <summary>
        /// The string used to set the default connection for the database.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// The context for the database of users.
        /// </summary>
        private readonly OESContext _context;

        /// <summary>
        /// Initialzes a new instance of the users controller class.
        /// </summary>
        /// <param name="context">The context for the users.</param>
        /// <param name="configuration">The default configuration for database connection.</param>
        public UsersController(OESContext context, IConfiguration configuration)
        {
            _context = context;

            Configuration = configuration;

            this.connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Creates the new user.
        /// </summary>
        /// <param name="user">The user being created.</param>
        /// <returns>Redirects to the login function once the user has been created.</returns>
        [HttpPost]
        public IActionResult CreateUser(Users user)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, EmailAddress, Admin) Values (@Username, @Password, @FirstName, @LastName, @EmailAddress, @Admin)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Add the parameters to the db.
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@Username",
                        Value = user.Username,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 450
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Password",
                        Value = user.Password,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@FirstName",
                        Value = user.FirstName,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@LastName",
                        Value = user.LastName,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@EmailAddress",
                        Value = user.EmailAddress,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Admin",
                        Value = user.Admin,
                        SqlDbType = SqlDbType.Int,
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    TempData["Admin"] = user.Admin;
                    TempData["Username"] = user.Username;
                    TempData["Password"] = user.Password;
                }
            }

            return RedirectToAction("ReadUser");
        }

        public Users ReadUser(int id)
        {
            string username = (string)TempData["Username"];
            Users user = new Users();
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM Users WHERE Id = {id}";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = Convert.ToInt32(dataReader["Id"]);
                        user.FirstName = Convert.ToString(dataReader["FirstName"]);
                        user.LastName = Convert.ToString(dataReader["LastName"]);
                        user.EmailAddress = Convert.ToString(dataReader["EmailAddress"]);
                        user.Username = Convert.ToString(dataReader["Username"]);
                        user.Password = Convert.ToString(dataReader["Password"]);
                        user.CardNumber = Convert.ToString(dataReader["CardNumber"]);
                        user.CardExpire = Convert.ToString(dataReader["CardExpire"]);
                        user.CardCvc = Convert.ToString(dataReader["CardCVC"]);
                        user.Orders = new ListService(this.Configuration).GetOrderList();
                    }
                }
                connection.Close();
            }

            return user;
        }

        public IActionResult UpdateCard(int id)
        {
            Users user = new Users();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"Select * From Users Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.CardNumber = Convert.ToString(dataReader["CardNumber"]);
                        user.CardExpire = Convert.ToString(dataReader["CardExpire"]);
                        user.CardCvc = Convert.ToString(dataReader["CardCvc"]);
                    }
                }
                connection.Close();
            }
            return View("../Login/Users/UpdateCard", user);
        }

        [HttpPost]
        public IActionResult UpdateCard(Users user, int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"Update Users SET CardNumber = '{user.CardNumber}', CardExpire = '{user.CardExpire}', CardCVC = '{user.CardCvc}' WHERE Id='{id}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("UserPage", "Login", new { id });
        }

        public IActionResult Index(int id)
        {
            Users user = this.ReadUser(id);
            return View(user);
        }
    }
}