﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System.IO;

namespace FowlerSite.Controllers
{
    public class GamesController : Controller
    {
        private readonly OESContext _context;

        public GamesController(OESContext context)
        {
            _context = context;
        }

        public void SetGameInfo(string[] info)
        {
            Game game = new Game();
            game.Name = info[0];
            game.Description = info[1];
            game.Genre = info[2];
            game.Price = Convert.ToDecimal(info[3]);
            this.AddGame(game);
        }

        public void AddGame([Bind("ProductID,Name,Description,Price,Genre")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                _context.SaveChangesAsync();
            }
        }

        // GET: Games also does a database check to see if all of your games exist.
        public async Task<IActionResult> Index()
        {
            List<Game> games = await _context.Games.ToListAsync();

            // Gets the games from the txt file.
            string[] lines = System.IO.File.ReadAllLines(@"Games.txt");

            if (games.Count != lines.Length - 1)
            {
                // Loop through the text file.
                for (int i = games.Count; i < lines.Length - 1; i++)
                {
                    string[] gameInfo = lines[i + 1].Split('`');
                    this.SetGameInfo(gameInfo);
                }
            }
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

        public IActionResult Find(Game game)
        {
            return View(game);
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ProductID == id);
        }
    }
}
