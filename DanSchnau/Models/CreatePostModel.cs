namespace DanSchnau.Models;

public class CreatePostModel
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Markdown { get; set; }
}
