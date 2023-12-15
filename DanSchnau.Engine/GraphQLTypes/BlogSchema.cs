using GraphQL.Instrumentation;
using GraphQL.Types;

namespace DanSchnau.Engine.GraphQLTypes;

public class BlogSchema : Schema
{
    public BlogSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        // Query = ....
        Query = (BlogQuery)serviceProvider.GetService(typeof(BlogQuery)) ??
            throw new InvalidOperationException();

        // no mutations for this enterprise today.
        // Mutation = ....

        FieldMiddleware.Use(new InstrumentFieldsMiddleware());
    }
}
