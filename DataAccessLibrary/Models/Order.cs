using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    /// <summary>
    /// The class which represents the order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        public User Customer { get; set; }
    }
}
