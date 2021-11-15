using DataAccessLibrary.Models;
using FowlerSite.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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

        public IEnumerable<Users> GetUserList()
        {
            List<Users> users = new List<Users>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = $"Select * from Users";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                // Filling records to datatable.
                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    users.Add(
                        new Users
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Username = Convert.ToString(dr["Username"]),
                            Password = Convert.ToString(dr["Password"]),
                            EmailAddress = Convert.ToString(dr["EmailAddress"]),
                            FirstName = Convert.ToString(dr["FirstName"]),
                            LastName = Convert.ToString(dr["LastName"]),
                            CardCvc = Convert.ToString(dr["CardCVC"]),
                            CardNumber = Convert.ToString(dr["CardNumber"]),
                            CardExpire = Convert.ToString(dr["CardExpire"]),
                            Admin = Convert.ToInt32(dr["Admin"])
                        });
                }

            }

            return users;
        }

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
        /// Method to get the users order list.
        /// </summary>
        /// <param name="ID">The ID of the customer.</param>
        /// <returns>Returns a Tuple like list using key value pair.</returns>
        public IEnumerable<(Order, OrderDetails)> GetOrders(int ID)
        {
            var orders = new List<(Order, OrderDetails)>();
            var user = new ListService(this.Configuration).GetUserList();

            // Testing variable until database more fits needs for this app
            int id = 0;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();
                string sql = $"SELECT [Order].[Order_ID], [Order].[Order_Date], [Order].[Cust_ID], [Order_Details].Total, [Order_Details].[Sub_Total], " +
                    "[Order_Details].[Product_ID], [Order_Details].[PaymentType] FROM[Order] +" +
                    $" INNER JOIN[Order_Details] ON[Order].Order_ID = [Order_Details].Order_ID WHERE Cust_ID={id}";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    orders.Add((
                        new Order
                        {
                            Order_ID = Convert.ToInt32(dr["Order_ID"]),
                            Order_Date = Convert.ToDateTime(dr["Order_Date"]),
                            Cust_ID = Convert.ToInt32(dr["Cust_ID"])
                        },
                        new OrderDetails
                        {
                            Order_ID = Convert.ToInt32(dr["Order_ID"]),
                            Product_ID = Convert.ToInt32(dr["Product_ID"]),
                            PaymentType = Convert.ToString(dr["PaymentType"])
                        }));
                }

                return orders;
            }
        }

        /// <summary>
        /// The method to get the list of orders.
        /// </summary>
        /// <returns>Returns an IEnumerable</returns>
        public IEnumerable<Order> GetOrderList()
        {
            List<Order> orders = new List<Order>();
            var users = new ListService(this.Configuration).GetUserList();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();
                string sql = "SELECT * FROM [Order]";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);
                User user = new User();

                foreach (DataRow dr in dataTable.Rows)
                {
                    orders.Add(
                        new Order
                        {
                            Order_ID = Convert.ToInt32(dr["Order_ID"]),
                            Order_Date = Convert.ToDateTime(dr["Order_Date"]),
                            Cust_ID = Convert.ToInt32(dr["Cust_ID"]),
                            User = new User{ FirstName = users.FirstOrDefault(u => u.Id == Convert.ToInt32(dr["Cust_ID"])).FirstName,
                                LastName = users.FirstOrDefault(u => u.Id == Convert.ToInt32(dr["Cust_ID"])).LastName
                            }
                        });
                }

                return orders;
            }
        }

        /// <summary>
        /// The method to get the order line or order detail.
        /// </summary>
        /// <param name="id">The id of the order.</param>
        /// <returns>Returns an IEnumerable.</returns>
        public IEnumerable<OrderDetails> GetOrderLineList(int id)
        {
            List<OrderDetails> orderLines = new List<OrderDetails>();
            var orders = new ListService(this.Configuration).GetOrderList();
            var products = new ListService(this.Configuration).GetProductList();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = $"Select * From [Order_Details] Where Order_Id={id}";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    orderLines.Add(
                        new OrderDetails
                        {
                            Order_ID = Convert.ToInt32(dr["Order_ID"]),
                            Product_ID = Convert.ToInt32(dr["Product_ID"]),
                            PaymentType = Convert.ToString(dr["PaymentType"]),
                            Order = orders.FirstOrDefault(o => o.Order_ID == Convert.ToInt32(dr["OrderID"])),
                            Game = products.First(p => p.ProductID == Convert.ToInt32(dr["Product_Id"])),
                        });
                }
            }

            return orderLines;
        }

        /// <summary>
        /// The method to get a list of products.
        /// </summary>
        /// <returns>Returns an IEnumerable.</returns>
        public IEnumerable<Game> GetProductList()
        {
            List<Game> products = new List<Game>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = $"Select * From Games";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    products.Add(
                        new Game
                        {
                            ProductID = Convert.ToInt32(dr["ProductID"]),
                            Name = Convert.ToString(dr["Name"]),
                            Description = Convert.ToString(dr["Description"]),
                            Price = Convert.ToDecimal(dr["Price"]),
                            Genre = Convert.ToString(dr["Genre"]),
                            Rating = Convert.ToInt32(dr["Rating"]),
                            Platforms = Convert.ToString(dr["Platforms"]),
                        });
                }

                return products;
            }
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
                            Platforms = Convert.ToString(dr["Platforms"]),
                            ImagePath =  Convert.ToString(dr["ImagePath"])
                        });
                }
            }

            return games;
        }
    }
}
