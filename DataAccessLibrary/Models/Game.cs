using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Game
    {
        /// <summary>
        /// The image path to uniquely identify the image associated with the game.
        /// </summary>
        public string imagePath
        {
            get
            {
                return "~/assets/images/game-" + this.ProductID + ".jpg";
            }
        }

        /// <summary>
        /// The id that is associated with the product.
        /// </summary>
        [Key]
        public int ProductID { get; set; }

        /// <summary>
        /// The name of the game.
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }

        /// <summary>
        /// The games description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The price of the game.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// The genre of the game.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// The star rating for the game.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// The platforms that the game can be purchased on.
        /// </summary>
        public string Platforms { get; set; }
    }
}
