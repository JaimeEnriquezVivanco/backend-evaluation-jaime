using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Postable.Entities
{
    [Index(nameof(PostId), nameof(UserId), IsUnique = true)]
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [NotNull]
        public int PostId { get; set; }
        public Post? Post { get; set; }

        [NotNull]
        public int UserId { get; set; }
        public User? User { get; set; }

        [NotNull]
        public DateTime CreatedAt { get; set; }
    }
}