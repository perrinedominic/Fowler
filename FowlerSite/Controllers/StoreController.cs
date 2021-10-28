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

        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart id.
        /// </summary>
        public int ShoppingCartId { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal
        {
            get
            {
                return GetSubtotal();
            }
            set { }
        }


        private OESContext _db;

        /// <summary>
        /// The Key for the cart session.
        /// </summary>
        public const string CartSessionKey = "CartId";

        /// <summary>
        /// The logger for the controller.
        /// </summary>
        private readonly ILogger<StoreController> _logger;

        /// <summary>
        /// 
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
            int cardId = 1;

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
                };

                _db.ShoppingCart.Add(cart);

                _db.SaveChanges();

                cardId = cart.CartId;
            }
            else
            {
                cardId = _db.ShoppingCart.Where(x => x.UserId == userId).Select(x => x.CartId).FirstOrDefault();
            }

            // Selects the cart item from the database based on the cart and product id.
            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == cardId
                && c.ProductId == productId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.
                cartItem = new CartItem()
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    CartId = cardId,
                    Game = _db.Games.SingleOrDefault(
                        p => p.ProductID == productId),
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                };

                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,
                // then add one to the quantity
                cartItem.Quantity++;
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

            List<CartItem> cartItems = GetCartItems(1);

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
        public Guid GetUserID()
        {
            return Guid.Empty;
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
            ShoppingCartId = 1;
            decimal totalCost = 0;
            List<CartItem> items = this.GetCartItems(1);

            foreach(CartItem i in items)
            {
               totalCost += i.Game.Price;
            }

            return totalCost;
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
        public ActionResult ProcessOrder(IFormCollection frc)
        {
            var keys = frc.Keys.ToArray();
            decimal subtotal = 0;
            List<CartItem> items = GetCartItems(1);
            int product = 0;
            
            foreach(CartItem c in items)
            {
                c.Game = _db.Games.SingleOrDefault(
                        p => p.ProductID == c.ProductId);
                var price = c.Game.Price;
                subtotal += price * c.Quantity;
                product = c.Game.ProductID;
            }

            var orderid = _db.Order.ToList().LastOrDefault().Order_ID;
            orderid++;

            var total = subtotal / 0.945m;

            // Save to the order table.
            Order order = new Order()
            {
                Order_ID = orderid,
                Order_Date = DateTime.Now,
            };

            // Save to the order details table.
            OrderDetails orderDetail = new OrderDetails()
            {
                PaymentType = "Card",
                Sub_Total = subtotal,
                Total = total,
                Order_ID = order.Order_ID,
                Product_ID = product,
            };

            _db.Order.Add(order);
            _db.Order_Details.Add(orderDetail);
            _db.SaveChanges();

            // Remove Shopping Cart Session.

            return View("OrderSuccess");
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
