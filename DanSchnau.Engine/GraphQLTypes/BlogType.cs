using DanSchnau.Entities;
using GraphQL.Types;

namespace DanSchnau.Engine.GraphQLTypes;

public class BlogType : ObjectGraphType<Blog>
{
    public BlogType() 
    {
        Name = "Blog";
        Field(b => b.BlogId).Description("The id of the blog.");
        Field(b => b.Published).Description("When the blog was first published.");
        Field(b => b.LastUpdated, nullable: true).Description("When the blog was last updated. Null if never.");
        Field(b => b.Title).Description("The Title of the Blog.");
        Field(b => b.Slug).Description("The slug for the blog, used in URL generation.");
        Field(b => b.Content).Description("The Content of the blog, serialized into HTML.");
        Field(name: "URL", expression: b => $"https://danschnau.com/blog/{b.Slug}").Description("The URL for the blog.");
    }
}
