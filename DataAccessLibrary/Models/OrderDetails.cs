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
		public decimal ItemPrice { get; set; }

		/// <summary>
		/// The total of the order
		/// </summary>
		[Column(TypeName = "int")]
		public decimal Quantity { get; set; }
		
		/// <summary>
		/// The id of a the related cart.
		/// </summary>
		[Column(TypeName = "int")]
		public int Product_ID { get; set; }
	}
}
