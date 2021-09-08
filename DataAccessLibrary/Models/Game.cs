using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Game
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Genre { get; set; }
    }
}
