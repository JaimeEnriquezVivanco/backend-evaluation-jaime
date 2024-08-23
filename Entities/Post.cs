using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Postable.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }
        public required User User { get; set; }

        [NotNull]
        [MinLength(10)]
        [MaxLength(1000)]
        public string? Content { get; set; }

        [NotNull]
        public DateTime CreatedAt { get; set; }

        public ICollection<Like> Like { get; set; } = [];
    }
}