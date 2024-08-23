using System.ComponentModel.DataAnnotations;

namespace Postable.Entities.Dtos
{
    public class UserPatchDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}