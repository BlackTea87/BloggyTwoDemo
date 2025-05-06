using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloggyTwo.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        // Foreign key to ApplicationUser
        [Required]
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public AppUserData Author { get; set; }
    }
}
