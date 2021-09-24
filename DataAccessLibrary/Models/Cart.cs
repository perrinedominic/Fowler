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

        public int ProductId { get; set; }

        public int CartStatusId { get; set; }

        public  DateTime AddedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public List<Game> Game;
    }
}
