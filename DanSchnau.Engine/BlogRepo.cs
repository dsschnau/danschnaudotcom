using DanSchnau.Entities;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace DanSchnau.Engine;

public class BlogRepo : BaseRepo
{
    public BlogRepo(IMemoryCache memoryCache) : base(memoryCache)
    {
    }

    public async Task<Blog> GetBlogBySlugAsync(string slug)
    {
        return await Db.QueryFirstAsync<Blog>(@"
					SELECT TOP 1 * FROM [dbo].[Blog]
					WHERE slug = @slug",
                new { slug = slug });
    }
    public async Task<Blog> GetBlogByIdAsync(string blogId)
    {
        if (string.IsNullOrWhiteSpace(blogId))
            throw new Exception("null blogId");


        var blog = await Db.QueryFirstAsync<Blog>(@"
						SELECT TOP 1 * FROM [dbo].[Blog]
						WHERE blogId = @blogId",
                new { blogId = blogId });

        if (blog == null)
            throw new ArgumentOutOfRangeException($"{nameof(blogId)} not found");

        return blog;
    }

    public async Task<IList<Blog>> GetBlogsAsync()
    {
        var result = await Db.QueryAsync<Blog>(@"
					SELECT * FROM [dbo].[Blog] ORDER BY [Published] DESC");
        return result.ToList();
    }

    public async Task<DateTime> GetLastUpdatedAsync()
    {
        var lastPublished = await Db.QuerySingleAsync<DateTime>(@"SELECT MAX(Published) FROM [dbo].[Blog]");
        var lastUpdated = await Db.QuerySingleAsync<DateTime>(@"SELECT MAX(LastUpdated) FROM [dbo].[Blog]");

        return lastUpdated > lastPublished ? lastUpdated : lastPublished;
    }
    public async Task UnHidePostAsync(string blogId)
    {
        await Db.ExecuteAsync(@"
					UPDATE [dbo].[Blog]
					SET Hidden = 0
					WHERE BlogId = @blogId", new { blogId = blogId });
    }

    public async Task HidePostAsync(string blogId)
    {
        await Db.ExecuteAsync(@"
					UPDATE [dbo].[Blog]
					SET Hidden = 1
					WHERE BlogId = @blogId", new { blogId = blogId });
    }
    public async Task DeletePostAsync(string blogId)
    {
        await Db.ExecuteAsync(@"
					DELETE FROM [dbo].[Blog]
					WHERE BlogId = @blogId", new { blogId = blogId });
    }

    public async Task CreateBlogAsync(Blog blog)
    {
        await Db.ExecuteAsync(@"
					INSERT INTO [dbo].[Blog]
					(Title, Slug, Content, Published, LastUpdated, Hidden) VALUES
					(@title, @slug, @content, @published, @lastupdated, @hidden)",
                new
                {
                    title = blog.Title,
                    slug = blog.Slug,
                    content = blog.Content,
                    published = blog.Published,
                    lastupdated = blog.LastUpdated,
                    hidden = blog.Hidden
                });
    }

    public async Task CreateBlogWithMarkdownAsync(Blog blog)
    {
        await Db.ExecuteAsync(@"
					INSERT INTO [dbo].[Blog]
					(Title, Slug, Content, Markdown, Published, LastUpdated, Hidden) VALUES
					(@title, @slug, @content, @markdown, @published, @lastupdated, @hidden)",
                new
                {
                    title = blog.Title,
                    slug = blog.Slug,
                    content = blog.Content,
                    markdown = blog.Markdown,
                    published = blog.Published,
                    lastupdated = blog.LastUpdated,
                    hidden = blog.Hidden
                });
    }

    public async Task UpdateBlogByIdWithMarkdownAsync(Blog blog)
    {
        await Db.ExecuteAsync(@"
					UPDATE [dbo].[Blog]
					SET
					Title = @title,
					Slug = @slug,
                    Content = @content,
                    Markdown = @markdown,
					LastUpdated = @lastupdated
					WHERE BlogId = @blogId",
               new
               {
                   blogId = blog.BlogId,
                   title = blog.Title,
                   slug = blog.Slug,
                   content = blog.Content,
                   markdown = blog.Markdown,
                   published = blog.Published,
                   lastupdated = DateTime.UtcNow,
                   hidden = blog.Hidden
               });
    }
}
