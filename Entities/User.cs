using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Postable.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [NotNull]
        [MinLength(
            5,
            ErrorMessage = "Username must be 5 characters or more."
        )]
        [MaxLength(
            20,
            ErrorMessage = "Username must be 20 characters or less."
        )]
        public string? Username { get; set; }

        [NotNull]
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [NotNull]
        [AllowedValues(
            "user", "admin",
            ErrorMessage = "Role must be either 'user' or 'admin'"
        )]
        public string Role { get; set; } = "user";

        [NotNull]
        public DateTime CreatedAt { get; set; }

        public ICollection<Post>? Post { get; set; } = [];
        public ICollection<Like>? Like { get; set; } = [];
    }
}