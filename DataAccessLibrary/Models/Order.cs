using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        public int Order_ID { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public DateTime Order_Date { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        public int Cust_ID { get; set; }

        /// <summary>
        /// The user that made the order.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The Order details attatched to the order.
        /// </summary>
        public IEnumerable<OrderDetails> Lines;

    }
}
