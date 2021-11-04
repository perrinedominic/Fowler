using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class PaymentInformation
    {
        public int PaymentInfoId { get; set; }

        public int CardNumber { get; set; }

        public int SecurityCode { get; set; }

        public DateTime ExpDate { get; set; }

        public string CardProvider { get; set; }
    }
}
