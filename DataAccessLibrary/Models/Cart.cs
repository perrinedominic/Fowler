using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Cart
    {
        /// <summary>
        /// Gets or sets the CartId.
        /// </summary>
        [Key]
        public int CartId { get; set; }

        /// <summary>
        /// Gets or sets the cart status.
        /// </summary>
        public int CartStatusId { get; set; }

        /// <summary>
        /// Gets or sets the added on time.
        /// </summary>
        public  DateTime AddedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on time.
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the cart items.
        /// </summary>
        public virtual List<CartItem> CartItems { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the subtotal.
        /// </summary>
        public decimal Subtotal
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public decimal Total { get; set; }
    }
}
