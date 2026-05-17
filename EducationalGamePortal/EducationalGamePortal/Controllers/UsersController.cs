using EducationalGamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationalGamePortal.Controllers
{
    public class UsersController : Controller
    {
        private readonly EducationalGameContext _context;

        public UsersController(EducationalGameContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.Nickname.ToLower().Contains(searchString.ToLower()));
            }

            var users = await usersQuery.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UsersTable", users);
            }

            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.GameSessions)
                    .ThenInclude(gs => gs.Level)
                        .ThenInclude(l => l.Game)
                .Include(u => u.UserAchievements)
                    .ThenInclude(ua => ua.Achievement)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Nickname) || string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("", "Нікнейм та email є обов'язковими полями.");
                return View(user);
            }

            user.PasswordHash = "default_hash";
            user.RegistrationDate = DateTime.Now;

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(user.Nickname) || string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("", "Нікнейм та email є обов'язковими полями.");
                return View(user);
            }

            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null) return NotFound();

                existingUser.Nickname = user.Nickname;
                existingUser.Email = user.Email;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
