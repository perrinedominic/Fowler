using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    /// <summary>
    /// The class which represents Payment information.
    /// </summary>
    public class PaymentInformation
    {
        /// <summary>
        /// The Id of the payment information.
        /// </summary>
        [Key]
        public int Payment_Info_Id { get; set; }

        /// <summary>
        /// The credit card numbe used for payment.
        /// </summary>
        [Column(TypeName = "varchar(16)")]
        public string Card_Number { get; set; }

        /// <summary>
        /// The security code of the credit card used.
        /// </summary>
        [Column(TypeName = "varchar(16)")]
        public int Security_Code { get; set; }
        
        /// <summary>
        /// The expiration date of the credit card used.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime Expiration_Date { get; set; }

        /// <summary>
        /// The credit card company.
        /// </summary>
        [Column(TypeName = "varchar(16)")]
        public string Card_Provider { get; set; }
    }
}
