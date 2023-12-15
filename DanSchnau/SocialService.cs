using DanSchnau.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
namespace DanSchnau;

public class SocialService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SocialService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task PostToMastodonAsync(Blog blog)
    {
        using HttpClient client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("Authorization", "Bearer oaQCRkoZsjSA_HKS0iPt3Gzy4nYu2h1t-xZ2__ViCrc");

        try
        {
            // Make HTTP Post Request
            var url = "https://toot.cafe/api/v1/statuses";
            var content = new
            {
                status = $"New Post: {blog.Title} - https://danschnau.com/blog/{blog.Slug}"
            };


            var result = await client.PostAsync(url, JsonContent.Create(content));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
