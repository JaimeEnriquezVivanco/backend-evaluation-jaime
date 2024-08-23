using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Postable.Entities.Dtos
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class UserPostDto
    {

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

        [EmailAddress]
        public string? Email { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [NotNull]
        [AllowedValues("user", "admin")]
        public string Role { get; set; } = "user";
    }
}