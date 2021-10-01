using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FowlerSite.Models
{
    public class Users
    {
        /// <summary>
        /// Gets or sets the username for the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}
