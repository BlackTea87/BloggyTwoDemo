using BloggyTwo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggyTwo.Data;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<AppUserData> _userManager;

        public AdminController(BlogDbContext context, UserManager<AppUserData> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List all users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Delete User and their blog posts
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var posts = _context.BlogPosts.Where(p => p.AuthorId == id);
            _context.BlogPosts.RemoveRange(posts);
            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }
            else
            {
                ModelState.AddModelError("", "Error deleting user");
                return View(nameof(Users), await _userManager.Users.ToListAsync());
            }
        }

        // List all posts
        public async Task<IActionResult> BlogPosts()
        {
            var posts = await _context.BlogPosts.Include(p => p.Author).ToListAsync();
            return View(posts);
        }

        // Admin create blog post
        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(BlogPost model)
        {
            if (!ModelState.IsValid) return View(model);

            // Admin creating post - assign Admin as Author
            var adminId = _userManager.GetUserId(User);
            model.AuthorId = adminId;
            model.Created = DateTime.UtcNow;

            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BlogPosts));
        }

        // Admin Edit post
        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(int id, BlogPost model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            post.Title = model.Title;
            post.Content = model.Content;

            _context.BlogPosts.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BlogPosts));
        }

        // Admin Delete post
        [HttpGet]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("DeletePost")]
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BlogPosts));
        }
    }
}
