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
                            EmailAddress = Convert.ToString(dr["EmailAddress"])
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
        public IEnumerable<Users> GetUserLoginList(string username)
        {
            List<Users> users = new List<Users>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                var logins = new ListService(Configuration).GetLoginList();
                DataTable dataTable = new DataTable();

                string sql = $"Select * from Users Where Username = {username}";
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
                            FirstName = Convert.ToString(dr["FirstName"]),
                            LastName = Convert.ToString(dr["LastName"]),
                            Login = logins.Where(l => l.Username == username).FirstOrDefault()
                        });
                }

            }

            return users;
        }
    }
}
