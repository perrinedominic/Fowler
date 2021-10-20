using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.DataAccess
{
    public class OESContext: DbContext
    {
        /// <summary>
        /// Initializes a new instance of the OESContext class.
        /// </summary>
        /// <param name="options">The options that the constructor uses.</param>
        public OESContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
                .HasKey(o => new { o.OrderID, o.ProductID });
        }

        /// <summary>
        /// Gets or sets the DbSet of the games.
        /// </summary>
        public DbSet<Game> Games { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the Shopping cart items.
        /// </summary>
        public DbSet<CartItem> ShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets the DbSet of the shopping cart.
        /// </summary>
        public DbSet<Cart> ShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets the DbSet of the Users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
