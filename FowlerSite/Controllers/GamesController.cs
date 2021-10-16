using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System.IO;
using Newtonsoft.Json;

namespace FowlerSite.Controllers
{
    public class GamesController : Controller
    {
        private readonly OESContext _context;

        public IEnumerable<Game> Games { get; set; }

        public GamesController(OESContext context)
        {
            _context = context;
            Games = _context.Games.ToList();
        }

        /// <summary>
        /// The method that adds a game to the database.
        /// </summary>
        /// <param name="game">The game being added.</param>
        public void AddGame([Bind("ProductID,Name,Description,Price,Genre")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                _context.SaveChangesAsync();
            }
        }

        /////// <summary>
        /////// The method to compare the json in the games.txt file to your existing database.
        /////// </summary>
        public async void JsonCompare() { 
        
            var games = _context.Games.ToList();

            // Gets the games from the txt file.
            string[] lines = await System.IO.File.ReadAllLinesAsync(@"Games.txt");
            string serializedGames = System.IO.File.ReadAllText(@"Games.txt");
            List<Game> deserializedGames = JsonConvert.DeserializeObject<List<Game>>(serializedGames);

            foreach (Game g in deserializedGames)
            {
                if (!games.Any(game => game.ProductID == g.ProductID))
                {
                    AddGame(g);
                }
            }
        }

        /// <summary>
        /// The method to add a new game to the json text.
        /// </summary>
        public void UpdateTextJson(Game game)
        {
            var gameJson = JsonConvert.SerializeObject(game);

            System.IO.File.WriteAllText(@"Games.txt", gameJson);
        }

        // GET: Games also does a database check to see if all of your games exist.
        public async Task<IActionResult> Index()
        {
            List<Game> games = await _context.Games.ToListAsync();

            // JsonCompare();

            return View(games);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Name,Description,Price,Genre")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            UpdateTextJson(game);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Description,Price,Genre")] Game game)
        {
            if (id != game.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// The method to find a game and go to that page.
        /// </summary>
        /// <param name="game">The game that needs to be found.</param>
        /// <returns>Returns a view.</returns>
        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ProductID == id);
        }

        public IActionResult Filter(string name, string genre, decimal lowPrice, decimal highPrice)
        {
            if (name != null)
                Games = from x in _context.Games where x.Name.Contains(name) select x;

            if (genre != null)
                Games = from x in _context.Games where (x.Genre == genre) select x;

            if (lowPrice >= 0 && lowPrice !> highPrice)
                Games = from x in _context.Games where ((x.Price >= lowPrice) && (x.Price <= highPrice)) select x;

            return RedirectToAction("Index");
        }
    }
}
