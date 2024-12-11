using System.ComponentModel.DataAnnotations;

namespace BlogApplication.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Description { get; set; }

        public string Status { get; set; } = "Draft";

        public string? UserId { get; set; }

        public string? AuthorName { get; set; }

        public DateTime? CreatedAt { get; set; }


    }
}
