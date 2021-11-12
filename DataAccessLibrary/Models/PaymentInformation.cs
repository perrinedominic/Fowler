using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class PaymentInformation
    {
        [Key]
        public int Payment_Info_Id { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string Card_Number { get; set; }

        [Column(TypeName = "varchar(16)")]
        public int Security_Code { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime Expiration_Date { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string Card_Provider { get; set; }
    }
}
