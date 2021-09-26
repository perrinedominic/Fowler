using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int CartStatusId { get; set; }

        public  DateTime AddedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public virtual List<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }
    }
}
