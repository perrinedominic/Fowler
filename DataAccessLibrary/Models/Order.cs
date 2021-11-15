using System;
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
        /// Gets or sets the id of the related payment information.
        /// </summary>
        [Column(TypeName = "int")][Required]
        public int? Payment_Info_ID { get; set; }
    }
}
