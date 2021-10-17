using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Order
    {
    [Key][Required]
    [Column(TypeName = "int")]
    public int OrderId { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string OrderName { get; set; }

    [Column(TypeName = "dateTime")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string PaymentType { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string Status { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string CustomerName { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string CustomerPhone { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string CustomerAddress { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string CustomerEmail { get; set; }
    }
}
