using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kinksweeper.Models;

public class PostContainer
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string file_url { get; set; } = null!;
    public long id { get; set; } = 0;
}

public static class Rule34ImageProvider
{
    private static readonly HttpClient client = new();
    private const string baseUrl = "https://api.rule34.xxx/index.php?page=dapi&s=post&q=index&limit=1000&json=1&tags=";

    public static async Task<List<PostContainer>> GetKinkPictures(int count, List<string> tags)
    {
        var builder = new StringBuilder().Append(baseUrl);
        tags.ForEach(tag => builder.Append(tag).Append("%20"));
        builder.Append("-animated");

        var responseStream = await client.GetStreamAsync(builder.ToString());
        var responseData = await JsonSerializer.DeserializeAsync<List<PostContainer>>(responseStream);
        return responseData!
            .OrderBy(_ => Guid.NewGuid())
            .Take(count)
            .ToList();
    }

    public static async Task<Stream?> DownloadImage(string url)
    {
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStreamAsync();
    }
}