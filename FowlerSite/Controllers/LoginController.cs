using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using FowlerSite.Models;
using FowlerSite.Services;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FowlerSite.Controllers
{
    public class LoginController : Controller
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
        public LoginController(OESContext context, IConfiguration configuration)
        {
            _context = context;

            this.Configuration = configuration;

            this.connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        /// <summary>
        /// Gets the user that logged in for the user page.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user found.</returns>
        public IActionResult UserPage(int id)
        {
            Users user = new Users();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM Users Where Id = {id}";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = id;
                        user.FirstName = Convert.ToString(dataReader["FirstName"]);
                        user.LastName = Convert.ToString(dataReader["LastName"]);
                        user.Username = Convert.ToString(dataReader["Username"]);
                        user.Password = Convert.ToString(dataReader["Password"]);
                        user.EmailAddress = Convert.ToString(dataReader["EmailAddress"]);
                        user.CardNumber = Convert.ToString(dataReader["CardNumber"]);
                        user.CardExpire = Convert.ToString(dataReader["CardExpire"]);
                        user.CardCvc = Convert.ToString(dataReader["CardCVC"]);
                        user.Orders = new ListService(this.Configuration).GetOrderList(id);
                    }
                }
                connection.Close();
            }

            return View("Users/User", user);
        }

        /// <summary>
        /// Gets the admin that logged in for the user page.
        /// </summary>
        /// <param name="id">The id of the admin.</param>
        /// <returns>The found admin that logged in.</returns>
        public IActionResult AdminPage(int id)
        {
            int userId = (int)TempData.Peek("UserId");
            Login user = new Login();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM Login Where UserId = {userId}";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = Convert.ToInt32(dataReader["Id"]);
                        user.Users = new ListService(Configuration).GetUserList();
                        user.Username = Convert.ToString(dataReader["Username"]);
                        user.Password = Convert.ToString(dataReader["Password"]);
                        user.Admin = Convert.ToInt32(dataReader["Admin"]);
                        user.UserId = Convert.ToInt32(dataReader["UserId"]);
                    }
                }
                connection.Close();
            }

            return View("Admin", user);
        }

        /// <summary>
        /// The method used to add users to a login list.
        /// </summary>
        /// <param name="login">The login for the user logging in.</param>
        /// <returns>The view for successfully logging in.</returns>
        public IActionResult UserLogin(Login login)
        {
            RedirectToActionResult view = null;
            string username = (string)TempData["Username"];
            string password = (string)TempData["Password"];

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM [Login] WHERE Username = '{login.Username}' and [Password] = '{login.Password}'";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        login.Id = Convert.ToInt32(dataReader["Id"]);
                        login.Users = new ListService(Configuration).GetUserList();
                        login.Admin = Convert.ToInt32(dataReader["Admin"]);
                        login.UserId = Convert.ToInt32(dataReader["UserId"]);
                        login.Username = Convert.ToString(dataReader["Username"]);
                        login.Password = Convert.ToString(dataReader["Password"]);
                    }
                }

                TempData["UserId"] = login.UserId;
            }
            if (login.Admin == 0)
            {
                view = RedirectToAction("UserPage", "Login", new { id = login.UserId });
            }
            if (login.Admin == 1)
            {
                view = RedirectToAction("AdminPage", "Login", new { id = login.UserId });
            }

            return view;
        }

        /// <summary>
        /// Creates a login from creating an account.
        /// </summary>
        /// <param name="id">The id of the login.</param>
        /// <returns>Directs to the login view.</returns>
        public IActionResult CreateLogin(int id)
        {
            int admin = (int)TempData["Admin"];
            string username = (string)TempData["Username"];
            string password = (string)TempData["Password"];
            RedirectToActionResult result = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string loginSql = "INSERT INTO Login (Username, Password, Admin, UserId) VALUES (@Username, @Password, @Admin, @UserId)";
                using (SqlCommand command = new SqlCommand(loginSql, connection))
                {
                    command.CommandType = CommandType.Text;

                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@Username",
                        Value = username,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 450
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Password",
                        Value = password,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Admin",
                        Value = admin,
                        SqlDbType = SqlDbType.Int,
                    };
                    command.Parameters.Add(parameter);

                    int UserID = this.GetUserID(username);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@UserId",
                        Value = UserID,
                        SqlDbType = SqlDbType.Int,
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            result = RedirectToAction("Login");

            return result;
        }

        public int GetUserID(string username)
        {
            Users user = new Users();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"SELECT * FROM Users WHERE Username = '{username}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        user.Id = Convert.ToInt32(dr["Id"]);
                    }
                }
                connection.Close();
            }

            return user.Id;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Gets the information needed to update the user.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The found user.</returns>
        [HttpGet]
        public IActionResult Update(int id)
        {
            Users user = new Users();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"SELECT * FROM Users WHERE Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

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
                    }
                }

                connection.Close();
            }

            return View("Update", user);
        }

        [HttpPost]
        public IActionResult Update(Users user, int id)
        {
            RedirectToActionResult result = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"Update Users SET Username='{user.Username}', Password='{user.Password}', FirstName='{user.FirstName}', LastName='{user.LastName}', " +
                    $"EmailAddress='{user.EmailAddress}', CardNumber='{user.CardNumber}', CardExpire='{user.CardExpire}', CardCVC='{user.CardCvc}' WHERE Id={id}";
                string loginSql = $"Update Login SET Username='{user.Username}', Password='{user.Password}' WHERE UserId='{user.Id}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                using (SqlCommand command = new SqlCommand(loginSql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("AdminPage");
        }

        public IActionResult Upgrade(Users user, int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"Update Users SET Admin=1 WHERE Id='{id}'";
                string loginSql = $"Update Login SET Admin=1 WHERE UserId='{id}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                using (SqlCommand command = new SqlCommand(loginSql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("AdminPage");
        }

        /// <summary>
        /// The method for the create view.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// The method for the create admin view.
        /// </summary>
        /// <returns>The create admin view.</returns>
        public IActionResult CreateAdmin()
        {
            return View();
        }

        /// <summary>
        /// The method for the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        public IActionResult Login(Login login)
        {
            return View(login);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,FirstName,LastName,EmailAddress,Password,Address")] User user)
        {
            if (id != user.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Username))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        /// <summary>
        /// Deletes the user from the admin page.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The redirection to the admin page.</returns>
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"DELETE FROM Users WHERE Id='{id}'";
                string loginSql = $"DELETE FROM Login WHERE UserId='{id}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                using (SqlCommand command = new SqlCommand(loginSql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("AdminPage");
        }

        public IActionResult Validate(Login login)
        {
            Login user = new Login();
            RedirectToActionResult result = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"SELECT Username, Password FROM Login WHERE Username='{login.Username}' and Password='{login.Password}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            user.Username = Convert.ToString(dataReader["Username"]);
                            user.Password = Convert.ToString(dataReader["Password"]);
                        }
                    }
                    connection.Close();
                }
                if (user.Username == "" || user.Password == "" || user.Username == null || user.Password == null)
                {
                    login.ErrorMessage = "Invalid Username or Password.";
                    result = RedirectToAction("Login", login);
                }
                else
                {
                    result = RedirectToAction("UserLogin", login);
                }
            }

            return result;
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Username == id);
        }
    }
}
