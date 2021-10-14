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
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public string Address { get; }

        public Game Game { get; set; }

        public Order Order { get; set; }


    }
}
