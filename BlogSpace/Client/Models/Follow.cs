using Postgrest.Models;
using Postgrest.Attributes;

namespace BlogSpace.Client.Models
{
    [Table("follows")]
    public class Follow : BaseModel
    {
        [Column("follower_id")]
        public string FollowerId { get; set; } = string.Empty;
        
        [Column("following_id")]
        public string FollowingId { get; set; } = string.Empty;
        
        [Column("status")]
        public string Status { get; set; } = "accepted";
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
} 