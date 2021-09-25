using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class CartItem
    {
        [Key]
        public string ItemId { get; set; }

        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        [ForeignKey(nameof(Game))]
        public int ProductId { get; set; }

        public virtual Game Game { get; set; }

        public virtual Cart Cart { get; set; }
    }
}
