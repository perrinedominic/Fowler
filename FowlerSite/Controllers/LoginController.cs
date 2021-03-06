using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using FowlerSite.Models;
using FowlerSite.Services;
using Microsoft.AspNetCore.Http;
using BC = BCrypt.Net.BCrypt;
using System.Collections.Generic;

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
        /// Gets or sets the message tempdata.
        /// </summary>
        [TempData]
        public string Message { get; set; }

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
            string UserId = Request.Cookies["UserId"];
            if (Convert.ToInt32(UserId) > 0)
            {
                id = Convert.ToInt32(UserId);
            }
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
            string userId = Request.Cookies["UserId"];
            Login user = new Login();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM Login Where UserId = {Convert.ToInt32(userId)}";
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

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM [Login] WHERE Username = '{login.Username}'";
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
                this.SetCookie("UserId", Convert.ToString(login.UserId), 14);
                this.SetCookie("Admin", Convert.ToString(0), 14);
                this.SetCookie("Admin", Convert.ToString(login.Admin), 14);
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

        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
                Response.Cookies.Append(key, value, option);
            }
            else
            {
                option.Expires = DateTime.Now.AddMinutes(60);
                Response.Cookies.Append(key, value, option);
            }
        }

        public void RemoveCookie(string key)
        {
            Response.Cookies.Delete(key);
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
            string passwordHash = BC.HashPassword(password);
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
                        Value = passwordHash,
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
        public IActionResult Create(Users user)
        {
            return View(user);
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
            string UserId = Request.Cookies["UserId"];
            RedirectToActionResult view = null;

            if (UserId != null)
            {
                view = RedirectToAction("UserPage", "Login", UserId);
                return view;
            }
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
                string sql = $"SELECT Username, Password FROM Login WHERE Username='{login.Username}'";

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
                    if (user.Password != null)
                    {
                        BC.Verify(login.Password, user.Password);
                        if (user.Username == "" || user.Password == "" || user.Username == null || user.Password == null || !BC.Verify(login.Password, user.Password))
                        {
                            login.ErrorMessage = "Invalid Username or Password.";
                            result = RedirectToAction("Login", login);
                        }
                        else
                        {
                            result = RedirectToAction("UserLogin", login);
                        }
                    }
                    else
                    {
                        login.ErrorMessage = "Invalid Username or Password.";
                        result = RedirectToAction("Login", login);
                    }
                }
                
            }

            return result;
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Username == id);
        }

        /// <summary>
        /// If the user is logged in, redirect to user page.
        /// </summary>
        /// <returns>The redirected action.</returns>
        public IActionResult DirectUser()
        {
            string UserId = Request.Cookies["UserId"];
            int id = Convert.ToInt32(UserId);
            if (id > 0)
            {
                return RedirectToAction("UserPage", id);
            }
            else
            {
                return RedirectToAction("Login", new Login());
            }
        }



        public IActionResult Logout()
        {
            this.RemoveCookie("UserId");
            this.RemoveCookie("Admin");
            Message = "You have successfully logged out.";
            return RedirectToAction("Login");
        }
    }
}
