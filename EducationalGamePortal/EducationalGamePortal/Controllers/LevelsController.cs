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
            if (level.GameId == 0 || level.LevelNumber <= 0 || string.IsNullOrWhiteSpace(level.Title) || string.IsNullOrWhiteSpace(level.Difficulty) || level.MaxScore <= 0)
            {
                ModelState.Clear();
                if (level.GameId == 0) ModelState.AddModelError("GameId", "Будь ласка, оберіть гру.");
                if (level.LevelNumber <= 0) ModelState.AddModelError("LevelNumber", "Номер рівня повинен бути більшим за 0.");
                if (string.IsNullOrWhiteSpace(level.Title)) ModelState.AddModelError("Title", "Назва рівня є обов'язковою.");
                if (string.IsNullOrWhiteSpace(level.Difficulty)) ModelState.AddModelError("Difficulty", "Складність рівня є обов'язковою.");
                if (level.MaxScore <= 0) ModelState.AddModelError("MaxScore", "Максимальний бал повинен бути більшим за 0.");
                
                ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
                return View(level);
            }

            bool levelNumberExists = await _context.Levels
                .AnyAsync(l => l.GameId == level.GameId && l.LevelNumber == level.LevelNumber);

            bool levelTitleExists = await _context.Levels
                .AnyAsync(l => l.GameId == level.GameId && l.Title == level.Title);

            if (levelNumberExists || levelTitleExists)
            {
                ModelState.Clear();
                if (levelNumberExists) ModelState.AddModelError("LevelNumber", "Рівень з таким номером вже існує у вибраній грі.");
                if (levelTitleExists) ModelState.AddModelError("Title", "Рівень з такою назвою вже існує у вибраній грі.");
                
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
            ViewBag.ReturnUrl = Request.Headers["Referer"].ToString();
            return View(level);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Level level, string returnUrl)
        {
            if (id != level.Id) return NotFound();

            if (level.GameId == 0 || level.LevelNumber <= 0 || string.IsNullOrWhiteSpace(level.Title) || string.IsNullOrWhiteSpace(level.Difficulty) || level.MaxScore <= 0)
            {
                ModelState.Clear();
                if (level.GameId == 0) ModelState.AddModelError("GameId", "Будь ласка, оберіть гру.");
                if (level.LevelNumber <= 0) ModelState.AddModelError("LevelNumber", "Номер рівня повинен бути більшим за 0.");
                if (string.IsNullOrWhiteSpace(level.Title)) ModelState.AddModelError("Title", "Назва рівня є обов'язковою.");
                if (string.IsNullOrWhiteSpace(level.Difficulty)) ModelState.AddModelError("Difficulty", "Складність рівня є обов'язковою.");
                if (level.MaxScore <= 0) ModelState.AddModelError("MaxScore", "Максимальний бал повинен бути більшим за 0.");

                ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
                ViewBag.ReturnUrl = returnUrl;
                return View(level);
            }

            bool levelNumberExists = await _context.Levels
                .AnyAsync(l => l.GameId == level.GameId && l.LevelNumber == level.LevelNumber && l.Id != id);

            bool levelTitleExists = await _context.Levels
                .AnyAsync(l => l.GameId == level.GameId && l.Title == level.Title && l.Id != id);

            if (levelNumberExists || levelTitleExists)
            {
                ModelState.Clear();
                if (levelNumberExists) ModelState.AddModelError("LevelNumber", "Рівень з таким номером вже існує у вибраній грі.");
                if (levelTitleExists) ModelState.AddModelError("Title", "Рівень з такою назвою вже існує у вибраній грі.");
                
                ViewBag.Games = new SelectList(await _context.Games.OrderBy(g => g.Title).ToListAsync(), "Id", "Title", level.GameId);
                ViewBag.ReturnUrl = returnUrl;
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

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
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
