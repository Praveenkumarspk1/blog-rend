using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BlogSpace.Shared.Models
{
    public class Post
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Visibility { get; set; } = "public"; // public, private, followers
        public string AuthorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Published { get; set; }
        
        private string _tags = string.Empty;
        
        [NotMapped]
        public List<string> Tags
        {
            get => string.IsNullOrEmpty(_tags) ? new List<string>() : _tags.Split(',').ToList();
            set => _tags = value != null ? string.Join(',', value) : string.Empty;
        }

        public string TagsString
        {
            get => _tags;
            set => _tags = value;
        }

        public string Slug { get; set; } = string.Empty;

        // Navigation properties
        public virtual User? Author { get; set; }
    }
} 