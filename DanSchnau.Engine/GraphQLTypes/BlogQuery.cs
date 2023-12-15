using GraphQL.Types;

namespace DanSchnau.Engine.GraphQLTypes;

public class BlogQuery : ObjectGraphType<object>
{
    private readonly BlogRepo BlogRepo;

    public BlogQuery(BlogRepo blogRepo)
    {
        BlogRepo = blogRepo;
        Name = "Query";

        FieldAsync<ListGraphType<BlogType>>("blogs", "get all blogs", resolve:
            async context => await blogRepo.GetBlogsAsync());

    }
}
