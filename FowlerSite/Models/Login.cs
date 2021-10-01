﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FowlerSite.Models
{
    public class Login
    {
        /// <summary>
        /// Gets or sets the username for the login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the login.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email address of the login.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}
