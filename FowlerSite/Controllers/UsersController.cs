﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.DataAccess;
using FowlerSite.Models;
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
        public IActionResult CreateUser(Users user, Login login)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, EmailAddress, Admin) Values (@Username, @Password, @FirstName, @LastName, @EmailAddress, @Admin)";
                string loginSql = "INSERT INTO Login (Username, Password, Admin) VALUES (@Username, @Password, @Admin)";

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
                }

                using (SqlCommand command = new SqlCommand(loginSql, connection))
                {
                    command.CommandType = CommandType.Text;

                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@Username",
                        Value = login.Username,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 450
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Password",
                        Value = login.Password,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Admin",
                        Value = login.Admin,
                        SqlDbType = SqlDbType.Int,
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Login", "Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}