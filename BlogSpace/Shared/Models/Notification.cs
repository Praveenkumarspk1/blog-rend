using System;

namespace BlogSpace.Shared.Models
{
    public class Notification
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // follow, like, comment, mention
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RelatedUserId { get; set; }
        public string? RelatedPostId { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual User? RelatedUser { get; set; }
        public virtual Post? RelatedPost { get; set; }
    }
} 