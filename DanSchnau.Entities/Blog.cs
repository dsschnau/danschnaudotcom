namespace DanSchnau.Entities;

public class Blog
{
    public string BlogId { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Markdown { get; set; }
    public DateTime Published { get; set; }
    public DateTime? LastUpdated { get; set; }
    public bool? Hidden { get; set; }
}
