using System.Diagnostics;
using BloggyTwo.Data;
using BloggyTwo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggyTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogDbContext _context;
        public HomeController(BlogDbContext context)
        {
            _context = context;
        }

        // Show all blog posts from all users
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Author)
                .OrderByDescending(p => p.Created)
                .ToListAsync();

            return View(posts);
        }
    }    
}
