using DanSchnau.Engine;
using DanSchnau.Entities;
using DanSchnau.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Markdig;

namespace DanSchnau.Controllers;

[Authorize()]
public class AdminController : Controller
{

    private BlogRepo BlogRepo { get; }
    private SocialService SocialService { get; }

    public AdminController(BlogRepo blogRepo, IHttpClientFactory httpClientFactory)
    {
        BlogRepo = blogRepo;
        SocialService = new SocialService(httpClientFactory);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var blogs = await BlogRepo.GetBlogsAsync();
        ViewData["Title"] = "Admin";
        return View(blogs);
    }

    [HttpGet]
    public IActionResult CreateMarkdownPost()
    {
        ViewData["Title"] = "Admin";
        return View();
    }

    /// <summary>
    /// Saves the post as hidden and returns to "CreateMarkdownPost" View
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PreviewMarkdownPost(CreatePostModel createModel)
    {
        ViewData["Title"] = "Admin";
        var content = Markdown.ToHtml(createModel.Markdown);
        var blog = new Blog
        {
            Hidden = true,
            Title = createModel.Title,
            Slug = createModel.Slug,
            Content = content,
            Published = DateTime.UtcNow,
            Markdown = createModel.Markdown,
            LastUpdated = null
        };

        await BlogRepo.CreateBlogAsync(blog);
        blog.BlogId = (await BlogRepo.GetBlogBySlugAsync(blog.Slug)).BlogId;

        return View("CreateMarkdownPost", blog);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMarkdownPost(CreatePostModel createModel)
    {
        ViewData["Title"] = "Admin";
        var content = Markdown.ToHtml(createModel.Markdown);
        var blog = new Blog
        {
            Hidden = true,
            Title = createModel.Title,
            Slug = createModel.Slug,
            Content = content,
            Published = DateTime.UtcNow,
            Markdown = createModel.Markdown,
            LastUpdated = null
        };

        await BlogRepo.CreateBlogWithMarkdownAsync(blog);
        await SocialService.PostToMastodonAsync(blog);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Obsolete("Use CreateMarkdownPost Instead.")]
    public async Task<IActionResult> CreatePost(CreatePostModel createModel)
    {
        ViewData["Title"] = "Admin";
        var blog = new Blog
        {
            Title = createModel.Title,
            Slug = createModel.Slug,
            Content = createModel.Content,
            Published = DateTime.UtcNow,
            LastUpdated = null
        };

        await SocialService.PostToMastodonAsync(blog);

        await BlogRepo.CreateBlogAsync(blog);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> EditPost(string blogId)
    {
        ViewData["Title"] = "Admin";
        var blog = await BlogRepo.GetBlogByIdAsync(blogId);
        return View(blog);
    }

    [HttpPost]
    public async Task<IActionResult> EditPost([FromForm] Blog blog)
    {
        ViewData["Title"] = "Admin";
        blog.Content = Markdown.ToHtml(blog.Markdown);
        await BlogRepo.UpdateBlogByIdWithMarkdownAsync(blog);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> HidePost([FromForm] string blogId)
    {
        ViewData["Title"] = "Admin";
        await BlogRepo.HidePostAsync(blogId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> PublishPreview(string blogId)
    {
        ViewData["Title"] = "Admin";
        var blog = await BlogRepo.GetBlogByIdAsync(blogId);
        await BlogRepo.UnHidePostAsync(blogId);
        await SocialService.PostToMastodonAsync(blog);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UnHidePost([FromForm] string blogId)
    {
        ViewData["Title"] = "Admin";
        await BlogRepo.UnHidePostAsync(blogId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost([FromForm] string blogId)
    {
        ViewData["Title"] = "Admin";
        await BlogRepo.DeletePostAsync(blogId);
        return RedirectToAction("Index");
    }
}
