using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public int Order_ID { get; set; }

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
		public int Product_ID { get; set; }

		/// <summary>
		/// The type of payment used for the order.
		/// </summary>
		public string PaymentType { get; set; }

		/// <summary>
		/// The Game that is on the order line.
		/// </summary>
		public Game Game { get; set; }

		/// <summary>
		/// The order line the Order is a part of.
		/// </summary>
		public Order Order { get; set; }
	}
}
