using DanSchnau.Engine;
using DanSchnau.Entities;
using Microsoft.AspNetCore.Components;

namespace DanSchnau.Components.Pages;

public partial class Home
{
    [Inject]
    private BlogRepo BlogRepo { get; set; }

    private IEnumerable<Blog> Blogs { get; set; }
    private IOrderedEnumerable<IGrouping<int, Blog>> GroupedByYearDescending
        => Blogs
            .GroupBy(b => b.Published.Year)
            .OrderByDescending(g => g.Key); 


    public Home()
    {
        Blogs = new List<Blog>();
    }

    protected async override Task OnInitializedAsync()
    {
        var blogs = await BlogRepo.GetBlogsAsync();
        Blogs = blogs.Where(b => b.Hidden == null || !b.Hidden.Value).ToList();
        await base.OnParametersSetAsync();
    }
}
