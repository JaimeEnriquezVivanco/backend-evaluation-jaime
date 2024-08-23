using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Postable.Entities
{
    public class PostPostDto
    {
        [NotNull]
        [MinLength(10)]
        [MaxLength(1000)]
        public string? Content { get; set; }
    }
}