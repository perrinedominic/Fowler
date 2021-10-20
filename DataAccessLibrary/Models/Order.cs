using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    /// <summary>
    /// The class which is used to represent an Order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The id of the order.
        /// </summary>
        [Key][Required]
        [Column(TypeName = "int")]
        public int OrderId { get; set; }

        /// <summary>
        /// The date the order took place.
        /// </summary>
        [Column(TypeName = "dateTime")]
        public DateTime OrderDate { get; set; }
    }
}
