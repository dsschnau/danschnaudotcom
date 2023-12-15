using DanSchnau.Entities;

namespace DanSchnau.Models;

public class BlogIndexModel
{
    public IEnumerable<Blog> Blogs { get; set; }

    public IOrderedEnumerable<IGrouping<int, Blog>> GroupedByYearDescending
        => Blogs
            .GroupBy(b => b.Published.Year)
            .OrderByDescending(g => g.Key); 
}
