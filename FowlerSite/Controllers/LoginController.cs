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
                    }
                }
                connection.Close();
            }

            return View("User");
        }

        public IActionResult AdminPage(int id)
        {
            return View();
        }

        /// <summary>
        /// The method used to add users to a login list.
        /// </summary>
        /// <returns>The view for successfully logging in.</returns>
        public IActionResult UserLogin()
        {
            Login login = new Login();
            RedirectToActionResult view = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Login";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        login.Id = Convert.ToInt32(dataReader["Id"]);
                        login.Username = Convert.ToString(dataReader["UserName"]);
                        login.Users = new ListService(Configuration).GetUserLoginList(login.Id);
                        login.Password = Convert.ToString(dataReader["Password"]);
                        login.Admin = Convert.ToInt32(dataReader["Admin"]);
                        login.UserId = Convert.ToInt32(dataReader["UserId"]);
                    }
                }
                connection.Close();
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

        public IActionResult CreateLogin(int id)
        {
            int admin = (int)TempData["Admin"];
            string username = (string)TempData["Username"];
            string password = (string)TempData["Password"];

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

                    parameter = new SqlParameter
                    {
                        ParameterName = "@UserId",
                        Value = id,
                        SqlDbType = SqlDbType.Int,
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Login");
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

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,FirstName,LastName,EmailAddress,Password,Address")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Username == id);
        }
    }
}
