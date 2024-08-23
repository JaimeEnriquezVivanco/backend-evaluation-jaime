namespace Postable.Entities.Dtos
{
    public class PostDisplayDto
    {
        public required int Id { get; set; }
        public required string? Content { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string? Username { get; set; }
        public required int LikesCount { get; set; }
    }
}