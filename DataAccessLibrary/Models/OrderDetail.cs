using DataAccessLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
	public class OrderDetail
	{
		[Required]
		[Column(TypeName = "int")]
		public int OrderID { get; set; }

		[Required]
		[Column(TypeName = "int")]
		public int ProductID { get; set; }

		[Column(TypeName = "float")]
		public decimal Price { get; set; }

		[Column(TypeName = "int")]
		public int Quantity { get; set; }
	}
}
