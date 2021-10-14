using DataAccessLibrary.Models;
using FowlerSite.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FowlerSite.Services
{
    public class ListService : IListService
    {
        /// <summary>
        /// Sets the default connection string.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the List Service class. 
        /// </summary>
        /// <param name="configuration">the configuration for the class.</param>
        public ListService(IConfiguration configuration)
        {
            Configuration = configuration;

            this.connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the product list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Login> GetLoginList()
        {
            List<Login> logins = new List<Login>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = $"Select * from Login";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                // Filling records to datatable.
                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    logins.Add(
                        new Login
                        {
                            Username = Convert.ToString(dr["Username"]),
                            Password = Convert.ToString(dr["Password"]),
                        });
                }

            }

            return logins;
        }

        /// <summary>
        /// Gets the user login list.
        /// </summary>
        /// <param name="username">The username for the user.</param>
        /// <returns></returns>
        public IEnumerable<Users> GetUserLoginList(int id)
        {
            List<Users> users = new List<Users>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                var logins = new ListService(Configuration).GetLoginList();
                DataTable dataTable = new DataTable();

                string sql = $"Select * from Users Where Id = {id}";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                // Filling records to datatable.
                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    users.Add(
                        new Users
                        {
                            Username = Convert.ToString(dr["Username"]),
                            Password = Convert.ToString(dr["Password"]),
                            EmailAddress = Convert.ToString(dr["EmailAddress"]),
                            FirstName = Convert.ToString(dr["FirstName"]),
                            LastName = Convert.ToString(dr["LastName"]),
                            
                            Login = logins.Where(l => l.Id == id).FirstOrDefault()
                        });
                }
            }

            return users;
        }

        /// <summary>
        /// Returns a list of games that can be used as an IEnumerable.
        /// </summary>
        /// <returns>Returns a list of games.</returns>
        public IEnumerable<Game> GetGames()
        {
            List<Game> games = new List<Game>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = "Select * from Games";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    games.Add(
                        new Game
                        {
                            ProductID = Convert.ToInt32(dr["ProductID"]),
                            Name = Convert.ToString(dr["Name"]),
                            Description = Convert.ToString(dr["Description"]),
                            Price = Convert.ToDecimal(dr["Price"]),
                            Genre = Convert.ToString(dr["Genre"]),
                            Rating = Convert.ToInt32(dr["Rating"]),
                            Platforms = Convert.ToString(dr["Platforms"])
                        });
                }
            }

            return games;
        }
    }
}
