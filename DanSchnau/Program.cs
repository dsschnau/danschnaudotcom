using DanSchnau.Components;
using DanSchnau.Engine;
using DanSchnau.Engine.GraphQLTypes;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationInsightsTelemetry()
    .AddHttpClient()
    .Configure<OpenIdConnectOptions>("AzureADOpenID", options => { })
    .AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
builder.Services
     .AddAuthorization(options =>
     {
         options.AddPolicy("adminpolicy", o =>
         {
             /* redacted */
         });
     })
    .Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for 
        // non -essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    })
    .Configure<MvcOptions>(o => o.EnableEndpointRouting = false)
    .AddMvc(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });
builder.Services
    .AddMemoryCache()
    .AddSingleton<BlogRepo>()
    .AddGraphQL(b =>
    {
        b.AddSystemTextJson()
         .AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true)
         .AddSchema<BlogSchema>()
         .AddGraphTypes(typeof(BlogSchema).Assembly);
    })
    .AddLogging(builder => builder.AddConsole());

builder.Services.AddRazorComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseGraphQL<ISchema>();
app.UseGraphQLPlayground();

app.UseRouting();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();


app.MapRazorComponents<App>();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
