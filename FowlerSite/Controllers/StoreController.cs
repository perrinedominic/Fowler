using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using FowlerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FowlerSite.Controllers
{
    /// <summary>
    /// The class which is used to represent a Store controller.
    /// </summary>
    public class StoreController : Controller
    {
        /// <summary>
        /// Gets or sets the shopping cart id.
        /// </summary>
        public int ShoppingCartId { get; set; }

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
        /// Initializes a new instance of the StoreController class.
        /// </summary>
        /// <param name="context">The context used by the controller.</param>
        /// <param name="logger">The logger that is used by the controller.</param>
        public StoreController(OESContext context, ILogger<StoreController> logger)
        {
            _db = context;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Stores the cart.
        /// </summary>
        /// <param name="id">The cart id.</param>
        /// <returns>The view of the shopping cart items.</returns>
        public IActionResult StoreCart(int id)
        {
            int cardId = AddToCart(id);

            IEnumerable < CartItem > items = _db.ShoppingCartItems.Include(x => x.Game).Where(x => x.CartId == cardId).ToList();
            return View(items);
        }

        public IActionResult StoreCheckout()
        {
            return View();
        }

        public IActionResult StoreProduct()
        {
            return View();
        }

        public IActionResult StoreCatalog()
        {
            return View();
        }

        /// <summary>
        /// Adds a product to the shopping cart/
        /// </summary>
        /// <param name="productId">The id of the selected product.</param>
        /// <returns>The int id of the product.</returns>
        public int AddToCart(int productId)
        {
            // Retrieve the product from the database.
            var userId = GetUserID();

            var cardId = 0;

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
                    DateCreated = DateTime.Now
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

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public Guid GetUserID()
        {
            return Guid.Empty;
        }

        /// <summary>
        /// Gets all the items in the shopping cart.
        /// </summary>
        /// <returns>A list that represents all the cart items in the shopping cart.</returns>
        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = 1;

            return _db.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId).ToList();
        }

        public decimal GetSubtotal()
        {
            decimal totalCost = 0;
            List<CartItem> items = this.GetCartItems();

            foreach(CartItem i in items)
            {
                Game game;
                decimal cost;

                game = i.Game;
                cost = game.Price * i.Quantity;

                totalCost += cost;
            }

            return totalCost;
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
    }
}
