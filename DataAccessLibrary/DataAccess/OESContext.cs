using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.DataAccess
{
    public class OESContext: DbContext
    {
        public OESContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
        public DbSet<Cart> ShoppingCart { get; set; }
    }
}
