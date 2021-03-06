using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.DataAccess
{
    /// <summary>
    /// The class which represents the OESContext.
    /// </summary>
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

        /// <summary>
        /// The method to run when models are created.
        /// </summary>
        /// <param name="modelBuilder">The model builder information.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
                .HasKey(o => new { o.Order_ID, o.Product_ID });

            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.Order_ID, o.Payment_Info_ID });
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

        /// <summary>
        /// Gets or sets the DBSet of the orders.
        /// </summary>
        public DbSet<Order> Order { get; set; }

        /// <summary>
        /// Gets or sets the db set of the order details.
        /// </summary>
        public DbSet<OrderDetails> Order_Details { get; set; }

        /// <summary>
        /// Gets or sets the db set of the payment information.
        /// </summary>
        public DbSet<PaymentInformation> Payment_Information { get; set; }
    }
}
