using System.Xml;
using System.Xml.Linq;
using DanSchnau.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DanSchnau.Controllers;

[AllowAnonymous]
public class FeedController : Controller
{
    private BlogRepo BlogRepo { get; }
    public FeedController(BlogRepo blogRepo)
    {
        this.BlogRepo = blogRepo;
    }

    [HttpGet("/blog/atom.xml")]
    [HttpGet("/blog/feed")]
    [HttpGet("/feed")]
    public async Task<IActionResult> Index()
    {
        var blogs = await this.BlogRepo.GetBlogsAsync();
        blogs = blogs.Where(b => b.Hidden == null || !b.Hidden.Value).ToList();

        var blogLastUpdated = await BlogRepo.GetLastUpdatedAsync();

        XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement(atomNamespace+ "feed",
                new XElement(atomNamespace+ "title", "Dan Schnau's Blog"),
                new XElement(atomNamespace+ "subtitle", "A blog from Dan Schnau."),
                new XElement(atomNamespace+ "link",
                    new XAttribute("href", "https://danschnau.com/feed"),
                    new XAttribute("rel", "self")
                    ),
                new XElement(atomNamespace+ "link",
                    new XAttribute("href", "https://www.danschnau.com")
                ),
                new XElement(atomNamespace+ "id", "urn:uuid:945c0606-0f88-48e8-b9d0-1472e0e37264"),
                new XElement(atomNamespace+ "updated", XmlConvert.ToString(blogLastUpdated, XmlDateTimeSerializationMode.Utc)),
                from blog in blogs
                select
                new XElement(atomNamespace+ "entry",
                    new XElement(atomNamespace+ "title", blog.Title),
                    new XElement(atomNamespace+ "link", 
                        new XAttribute("href", $"https://danschnau.com/blog/{blog.Slug}")),
                    new XElement(atomNamespace+ "id", $"urn:uuid:{blog.BlogId.ToString().ToLowerInvariant()}"),
                    new XElement(atomNamespace+ "updated", XmlConvert.ToString((blog.LastUpdated ?? blog.Published), XmlDateTimeSerializationMode.Utc)),
                    new XElement(atomNamespace+ "content", new XAttribute("type", "html"),
                        blog.Content
                    ),
                    new XElement(atomNamespace+ "author",
                        new XElement(atomNamespace+ "name", "Dan Schnau"),
                        new XElement(atomNamespace+ "uri", "https://danschnau.com")
                        )
                    )
                )
            );
        return Content(ToStringWithDecl(doc), "application/xml");
    }

    private string ToStringWithDecl(XDocument d) => $"{d.Declaration}{Environment.NewLine}{d}";
}