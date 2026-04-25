using SQLite;

namespace SocialFeed.Models;

public class Post
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
    
    public string? ImagePath { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsPublic { get; set; } = true;
}
