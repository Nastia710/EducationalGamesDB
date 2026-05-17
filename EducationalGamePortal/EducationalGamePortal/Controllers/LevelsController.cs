using EducationalGamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EducationalGamePortal.Controllers
{
    public class LevelsController : Controller
    {
        private readonly EducationalGameContext _context;

        public LevelsController(EducationalGameContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var levels = await _context.Levels
                .Include(l => l.Game)
                .OrderBy(l => l.Game.Title)
                .ThenBy(l => l.LevelNumber)
                .ToListAsync();
            return View(levels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Levels
                .Include(l => l.Game)
                .Include(l => l.GameSessions)
                    .ThenInclude(gs => gs.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (level == null) return NotFound();

            return View(level);
        }

        public async Task<IActionResult> Create(int? gameId)
        {
            ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", gameId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Level level)
        {
            if (string.IsNullOrWhiteSpace(level.Title) || string.IsNullOrWhiteSpace(level.Difficulty))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Поля «Назва» та «Складність» є обов'язковими для заповнення.");
                ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
                return View(level);
            }

            _context.Add(level);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Games", new { id = level.GameId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Levels.FindAsync(id);
            if (level == null) return NotFound();

            ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
            return View(level);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Level level)
        {
            if (id != level.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(level.Title) || string.IsNullOrWhiteSpace(level.Difficulty))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Поля «Назва» та «Складність» є обов'язковими для заповнення.");
                ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
                return View(level);
            }

            try
            {
                var existingLevel = await _context.Levels.FindAsync(id);
                if (existingLevel == null) return NotFound();

                existingLevel.GameId = level.GameId;
                existingLevel.LevelNumber = level.LevelNumber;
                existingLevel.Title = level.Title;
                existingLevel.Difficulty = level.Difficulty;
                existingLevel.MaxScore = level.MaxScore;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Levels.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Levels
                .Include(l => l.Game)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (level == null) return NotFound();

            return View(level);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level != null)
            {
                int gameId = level.GameId;
                _context.Levels.Remove(level);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Games", new { id = gameId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
