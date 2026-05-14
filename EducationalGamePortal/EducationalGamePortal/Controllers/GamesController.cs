using EducationalGamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationalGamePortal.Controllers
{
    public class GamesController : Controller
    {
        private readonly EducationalGameContext _context;

        public GamesController(EducationalGameContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _context.Games
                .Include(g => g.Levels)
                .ToListAsync();
            return View(games);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.Levels)
                    .ThenInclude(l => l.GameSessions)
                        .ThenInclude(gs => gs.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            return View(game);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game game)
        {
            if (string.IsNullOrWhiteSpace(game.Title) || string.IsNullOrWhiteSpace(game.Subject))
            {
                ModelState.AddModelError("", "Назва та предмет є обов'язковими полями.");
                return View(game);
            }

            _context.Add(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game)
        {
            if (id != game.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(game.Title) || string.IsNullOrWhiteSpace(game.Subject))
            {
                ModelState.AddModelError("", "Назва та предмет є обов'язковими полями.");
                return View(game);
            }

            try
            {
                var existingGame = await _context.Games.FindAsync(id);
                if (existingGame == null) return NotFound();

                existingGame.Title = game.Title;
                existingGame.Subject = game.Subject;
                existingGame.Description = game.Description;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Games.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();

            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
