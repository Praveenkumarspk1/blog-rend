using Postgrest.Models;
using Postgrest.Attributes;
using System.Text.Json.Serialization;

namespace BlogSpace.Client.Models
{
    [Table("profiles")]
    public class UserProfile : BaseModel
    {
        [Column("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Id { get; set; }
        
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        
        [Column("username")]
        public string Username { get; set; } = string.Empty;
        
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;
        
        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }
        
        [Column("bio")]
        public string? Bio { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Properties not in database but used by UI
        [JsonIgnore]
        public string? Location { get; set; }
        
        [JsonIgnore]
        public string? Website { get; set; }
        
        [JsonIgnore]
        public int PostsCount { get; set; }
        
        [JsonIgnore]
        public int FollowersCount { get; set; }
        
        [JsonIgnore]
        public int FollowingCount { get; set; }
    }
} 