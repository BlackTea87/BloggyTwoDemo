using BloggyTwo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggyTwo.Data;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class BlogPostController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<AppUserData> _userManager;

        public BlogPostController(BlogDbContext context, UserManager<AppUserData> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show only the posts by the logged-in user
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var posts = _context.BlogPosts
                .Where(p => p.AuthorId == user.Id)
                .OrderByDescending(p => p.Created);

            return View(await posts.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // User must be logged in
            }

            model.AuthorId = user.Id;
            model.Created = DateTime.UtcNow;

            _context.BlogPosts.Add(model);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error here if you want
                ModelState.AddModelError("", "Unable to save changes. Try again.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Get the post only if author is current user
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == userId);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: BlogPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            // Verify that post belongs to current user
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == userId);

            if (post == null)
            {
                return NotFound();
            }

            post.Title = model.Title;
            post.Content = model.Content;

            try
            {
                _context.BlogPosts.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
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

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == userId);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == userId);

            if (post == null)
                return NotFound();

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
