using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class CartItem
    {
        /// <summary>
        /// Gets or sets the ItemID.
        /// </summary>
        [Key]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the CartId.
        /// </summary>
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of items in the cart.
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the date created of the item.
        /// </summary>
        public System.DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the Product ID of the item.
        /// </summary>
        [ForeignKey(nameof(Game))]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the Game in the cart.
        /// </summary>
        public virtual Game Game { get; set; }

        /// <summary>
        /// Gets or sets the cart that is being used.
        /// </summary>
        public virtual Cart Cart { get; set; }
    }
}
