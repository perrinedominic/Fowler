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
    public class StoreController : Controller
    {
        public int ShoppingCartId { get; set; }

        private OESContext _db;

        public const string CartSessionKey = "CartId";

        private readonly ILogger<StoreController> _logger;

        public StoreController(OESContext context, ILogger<StoreController> logger)
        {
            _db = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

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
            _db.SaveChanges();

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

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = 1;

            return _db.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId).ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
