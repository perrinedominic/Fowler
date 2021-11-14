using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FowlerSite.Models
{
    /// <summary>
    /// The class used to represent the users.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets or sets the users id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the users first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the users last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets hte uesr login.
        /// </summary>
        public Login Login { get; set; }

        /// <summary>
        /// Gets or sets whether the user is an admin.
        /// </summary>
        public int Admin { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card expiration date.
        /// </summary>
        public string CardExpire { get; set; }

        /// <summary>
        /// Gets or sets the users orders.
        /// </summary>
        public IEnumerable<DataAccessLibrary.Models.Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the card cvc.
        /// </summary>
        public string CardCvc { get; set; }

        /// <summary>
        /// Overrides the original tostring method.
        /// </summary>
        /// <returns>The first and last name of the user.</returns>
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
