using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BloggyTwo.Models;

namespace BloggyTwo.Data
{
    public class BlogDbContext : IdentityDbContext<AppUserData>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
