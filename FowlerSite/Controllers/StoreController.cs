using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using FowlerSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace FowlerSite.Controllers
{
    /// <summary>
    /// The class which is used to represent a Store controller.
    /// </summary>
    public class StoreController : Controller
    {
        /// <summary>
        /// The string that establishes the database connection.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the StoreController class.
        /// </summary>
        /// <param name="context">The context used by the controller.</param>
        /// <param name="logger">The logger that is used by the controller.</param>
        public StoreController(OESContext context, ILogger<StoreController> logger, IConfiguration configuration)
        {
            _db = context;
            _logger = logger;
            this.Configuration = configuration;
            this.connectionString = Configuration["ConnectionStrings:DefaultConnection"];

        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart id.
        /// </summary>
        public int ShoppingCartId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of a cart item.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public Login login { get; set; }

        /// <summary>
        /// Gets or sets the cart.
        /// </summary>
        public Cart Cart { get; set; }

        
        /// <summary>
        /// Gets or sets the subtotal for the cart.
        /// </summary>
        public decimal Subtotal
        {
            get
            {
                if (Cart != null && Cart.Subtotal > 0)
                {
                    return Cart.Subtotal;
                } else
                {
                    return 0;
                }
            }
            set { }
        }

        /// <summary>
        /// The current database context.
        /// </summary>
        private OESContext _db;

        /// <summary>
        /// The Key for the cart session.
        /// </summary>
        public const string CartSessionKey = "CartId";

        /// <summary>
        /// The logger for the controller.
        /// </summary>
        private readonly ILogger<StoreController> _logger;

        [TempData]
        public string Message { get; set; }

        /// <summary>
        /// Gets the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        [Route("pay")]
        public Task<dynamic> Pay(Models.Payment payment)
        {
            return MakePayment.PayAsync(payment.CardNumber, payment.Month, payment.Year, payment.Cvc, payment.Value);
        }

        /// <summary>
        /// Stores the cart.
        /// </summary>
        /// <param name="id">The cart id.</param>
        /// <returns>The view of the shopping cart items.</returns>
        public IActionResult StoreCart(int id)
        {
            var userId = GetUserID();
            int cardId = 1;
            if (TempData.Peek("UserId") != null)
            {
                cardId = _db.ShoppingCart.Where(x => x.UserId == userId).Select(x => x.CartId).FirstOrDefault();
            }

            if (id.ToString().StartsWith("-"))
            {
                this.Remove(id);
            }
            else
            {
                AddToCart(id);
            }

            IEnumerable < CartItem > items = _db.ShoppingCartItems.Include(x => x.Game).Where(x => x.CartId == cardId).ToList();
            return View(items);
        }

        //[HttpPost]
        //public ActionResult StoreCart(int quantity)
        //{
        //    IEnumerable <CartItem> cartItems = GetCartItems();
        //    foreach(CartItem c in cartItems)
        //    {

        //    }
        //}

        /// <summary>
        /// Navigates to the store checkout page.
        /// </summary>
        /// <returns>Returns the view for the store checkout.</returns>
        public IActionResult StoreCheckout()
        {
            int cardId = 1;
            IEnumerable<CartItem> items = _db.ShoppingCartItems.Include(x => x.Game).Where(x => x.CartId == cardId).ToList();
            
            return View(items);
        }

        /// <summary>
        /// Navigates to the store product page.
        /// </summary>
        /// <returns>The view for the store product page.</returns>
        public IActionResult StoreProduct()
        {
            return View();
        }

        /// <summary>
        /// Navigates to the store catalog page.
        /// </summary>
        /// <returns>The view for the store catalog page.</returns>
        public IActionResult StoreCatalog()
        {
            return View();
        }

        public IActionResult Update(IFormCollection fc)
        {
            string[] quantities = fc["quantity"];
            List<CartItem> items = GetCartItems(1);
            for(int i = 0; i < items.Count; i++)
            {
                items[i].Quantity = Convert.ToInt32(quantities[i]);
                foreach(CartItem c in _db.ShoppingCartItems)
                {
                    if (c.ProductId == items[i].ProductId)
                    {
                        c.Quantity = items[i].Quantity;
                    }
                }
                _db.SaveChanges();
            }
            return Redirect("StoreCart/0");
        }

        /// <summary>
        /// Adds a product to the shopping cart by creating a shopping cart if there is none.
        /// If there is already a shopping cart, the cart will be selected based on ID. The product
        /// will then be added to that table and the view.
        /// </summary>
        /// <param name="productId">The id of the selected product.</param>
        /// <returns>The int id of the product.</returns>
        public int AddToCart(int productId)
        {
            // Retrieve the product from the database.
            var userId = GetUserID();

            var cardId = 0;

            // Create a cart if there is not already one assigned to the user.
            if(_db.ShoppingCart.Any(x => x.UserId == userId) == false)
            {
                var cart = new Cart
                {
                    AddedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    UserId = userId,
                    Subtotal = 0,
                    Total = Subtotal * .055m + Subtotal
                };

                _db.ShoppingCart.Add(cart);

                _db.SaveChanges();

                cardId = cart.CartId;

                this.Cart = cart;
            }
            else
            {
                cardId = _db.ShoppingCart.Where(x => x.UserId == userId).Select(x => x.CartId).FirstOrDefault();
            }

            // Selects the cart item from the database based on the cart and product id.
            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == cardId
                && c.ProductId == productId);

            if (cartItem == null && productId != 0)
            {
                // Create a new cart item if no cart item exists.
                cartItem = new CartItem()
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    CartId = cardId,
                    Quantity = 1,
                    ItemPrice = _db.Games.FirstOrDefault(p => p.ProductID == productId).Price,
                    DateCreated = DateTime.Now,
                };

                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,
                // then add one to the quantity
                if (productId != 0)
                {
                    cartItem.Quantity++;
                }
            }
            if (productId != 0)
            {
                _db.SaveChanges();
            }

            /*
            Game game = FindGame(productId);
            Cart.Add((game.ProductID);
            c.AddedOn = DateTime.Now;
            c.CartStatusId = 1;
            c.ProductId = productId;
            c.UpdatedOn = DateTime.Now;
            */

            return cardId;
        }

        /// <summary>
        /// Removes the selected product from the database and the view.
        /// </summary>
        /// <param name="productId">The id of the product to be removed.</param>
        /// <returns>Redirects the action to the store cart.</returns>
        public IActionResult Remove(int productId)
        {
            var userId = GetUserID();
            var cardId = _db.ShoppingCart.Where(x => x.UserId == userId).Select(x => x.CartId).FirstOrDefault();

            List<CartItem> cartItems = GetCartItems(cardId);

            // Loops through the cart items to find the correct product id.
            foreach(CartItem c in cartItems)
            {
                if("-" + c.ProductId.ToString() == productId.ToString())
                {
                    _db.ShoppingCartItems.Remove(c);
                    _db.SaveChanges();
                    break;
                }
            }

            return RedirectToAction("StoreCart");
        }

        /// <summary>
        /// Gets the user id for the shopping cart.
        /// </summary>
        /// <returns>An empty GUID for the id.</returns>
        public int? GetUserID()
        {
            int? userid = (int)TempData.Peek("UserId");
            if (userid != null)
            {
                return userid;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all the items in the shopping cart.
        /// </summary>
        /// <returns>A list that represents all the cart items in the shopping cart.</returns>
        public List<CartItem> GetCartItems(int cartId)
        {
            ShoppingCartId = cartId;

            return _db.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId).ToList();
        }

        /// <summary>
        /// Calculates the subtotal of the carts based on the quantity of items and their respective prices.
        /// </summary>
        /// <returns></returns>
        public decimal GetSubtotal()
        {
            return Subtotal;
        }

        public ActionResult CheckOut(FormCollection frc)
        {
            return View("StoreCheckout");
        }

        /// <summary>
        /// Processes the order that was placed by the customer.
        /// </summary>
        /// <param name="frc">The data from the form submitted for the method.</param>
        /// <returns>Returns the Order Success view.</returns>
        public IActionResult ProcessOrder(IFormCollection frc)
        {
            var keys = frc.Keys.ToArray();
            int orderid = 0;
            List<CartItem> items = GetCartItems(1);

            var paymentinfoid = _db.Payment_Information.AsNoTracking().ToList().LastOrDefault().Payment_Info_Id++;
            paymentinfoid++;
            // Create the payment information for the order.
            PaymentInformation payment = new PaymentInformation()
            {
                Card_Number = frc["cardnumber"],
                Card_Provider = frc["cardtype"],
                Security_Code = Convert.ToInt32(frc["cvc"]),
                Expiration_Date = Convert.ToDateTime(frc["expdate"]),
                Payment_Info_Id = paymentinfoid
            };

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 1 Order_ID FROM [dbo].[Order] ORDER BY Order_ID DESC;";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        orderid = Convert.ToInt32(dataReader["Order_ID"]);
                    };
                }
                orderid++;
                string orderDate = DateTime.Now.ToString("MM-dd-yyyy");
                sql = $"INSERT INTO [dbo].[Order] (Order_ID, Order_Date, Cust_ID, Payment_Info_ID) VALUES ({orderid}, {orderDate}, 1, {payment.Payment_Info_Id});";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                // Add order detail for each item in the shopping cart
                foreach (CartItem c in items)
                {
                    // Save to the order details table.
                    OrderDetails orderDetail = new OrderDetails()
                    {
                        Order_ID = orderid,
                        ItemPrice = c.ItemPrice,
                        Product_ID = c.ProductId,
                        Quantity = c.Quantity
                    };

                    sql = $"INSERT INTO [dbo].[Order_Details] (Order_ID, Quantity, ItemPrice, Product_ID) VALUES" +
                        $" ({orderid}, {orderDetail.Quantity}, {orderDetail.ItemPrice}, {orderDetail.Product_ID})";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }

                string expDate = payment.Expiration_Date.ToString("MM-dd-yyyy");
                sql = $"INSERT INTO [dbo].[Payment_Information] (Payment_Info_ID, Card_Number, Security_Code, Expiration_Date, Card_Provider) VALUES(" +
                    $"{payment.Payment_Info_Id}, {payment.Card_Number}, {payment.Security_Code}, '{expDate}', 'VISA')";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }

            // Remove Shopping Cart Session.

            _db.ShoppingCartItems.RemoveRange(_db.ShoppingCartItems);
            _db.SaveChanges();
            Message = "Thank you. Your order has been submitted.";
            // Remove Shopping Cart Session.

            return Redirect("StoreCart/0");
        }


        /// <summary>
        /// Used to cartch errors.
        /// </summary>
        /// <returns>The error that occured.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// The method to handle the placement of an order.
        /// </summary>
        /// <returns>Returns an IActionResult.</returns>
       [HttpPost]
        public IActionResult PlaceOrder()
        {
            int customerID = 5;
            DateTime today = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string sql = $"Insert Into [Order] (Order_Date, Cust_ID) Values ({today}, {customerID})";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return this.RedirectToAction("User", "Login", new { id = customerID });
        }
    }
}
