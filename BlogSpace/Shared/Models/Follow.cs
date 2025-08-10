using System;

namespace BlogSpace.Shared.Models
{
    public class Follow
    {
        public string Id { get; set; } = string.Empty;
        public string FollowerId { get; set; } = string.Empty;
        public string FollowingId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "accepted"; // pending, accepted, rejected

        // Navigation properties
        public virtual User? Follower { get; set; }
        public virtual User? Following { get; set; }
    }
} 