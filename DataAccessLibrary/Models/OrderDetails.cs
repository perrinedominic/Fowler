using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    /// <summary>
    /// The class representing the order details.
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the subtotal.
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Gets or sets the Games.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public Order Order { get; set; }


    }
}
