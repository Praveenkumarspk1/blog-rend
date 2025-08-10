using Postgrest.Models;
using Postgrest.Attributes;
using System.Text.Json.Serialization;

namespace BlogSpace.Client.Models
{
    [Table("notifications")]
    public class Notification : BaseModel
    {
        [Column("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Id { get; set; }
        
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
        
        [Column("type")]
        public string Type { get; set; } = string.Empty;
        
        [Column("message")]
        public string Message { get; set; } = string.Empty;
        
        [Column("related_id")]
        public string? RelatedId { get; set; }
        
        [Column("read")]
        public bool Read { get; set; }

        // Property not in database but used by UI
        [JsonIgnore]
        public string? Summary { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public UserProfile? Actor { get; set; }
    }
} 