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

            Configuration = configuration;

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
        /// The method used to add users to a login list.
        /// </summary>
        /// <returns>The view for successfully logging in.</returns>
        public IActionResult UserLogin()
        {
            List<Login> userList = new List<Login>();
            string username = null;
            string password = null;
            string emailAddress = null;
            ViewResult result = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Users";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Login user = new Login();

                        user.Username = Convert.ToString(dataReader["UserName"]);
                        user.EmailAddress = Convert.ToString(dataReader["EmailAddress"]);
                        user.Password = Convert.ToString(dataReader["Password"]);
                        userList.Add(user);
                        if (user.Username == "Admin")
                        {
                            username = user.Username;
                        }
                        if (user.Password == "PWD")
                        {
                            password = user.Password;
                        }
                        if (user.EmailAddress == "admin@email.com")
                        {
                            emailAddress = user.EmailAddress;
                        }
                    }
                }
                connection.Close();
            }
            if (username == "Admin" && password == "PWD" && emailAddress == "admin@email.com")
            {
                result = View("Admin");
            }
            else
            {
                result = View("User");
            }

            return result;
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
        /// Creates the new user.
        /// </summary>
        /// <param name="user">The user being created.</param>
        /// <returns>Redirects to the login function once the user has been created.</returns>
        [HttpPost]
        public IActionResult CreateUser(Login user)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = "INSERT INTO Users (Username, Password, EmailAddress) Values (@Username, @Password , @EmailAddress)";

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
                        ParameterName = "@EmailAddress",
                        Value = user.EmailAddress,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return View("Login");
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
