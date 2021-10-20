﻿using DataAccessLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
	/// <summary>
	/// The class which is used to represent Order Details.
	/// </summary>
	public class OrderDetails
	{
		/// <summary>
		/// The ID of the order.
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int OrderID { get; set; }

		/// <summary>
		/// The subtotal of the order.
		/// </summary>
		[Column(TypeName = "float")]
		public decimal Sub_Total { get; set; }

		/// <summary>
		/// The total of the order
		/// </summary>
		[Column(TypeName = "float")]
		public decimal Total { get; set; }
		
		/// <summary>
		/// The id of a product.
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int ProductID { get; set; }

		/// <summary>
		/// The type of payment used for the order.
		/// </summary>
		public string PaymentType { get; set; }
	}
}
