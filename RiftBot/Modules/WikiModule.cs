namespace RiftBot;

public class WikiModule : ModuleBase<SocketCommandContext>
{
    [Command("wiki-search", RunMode = RunMode.Async)]
    [Summary("!wiki-search <search>")]
    public async Task SearchWiki([Remainder] string search)
    {
        try
        {
            string response = await CallWikiApi(search);
            Results results = JsonSerializer.Deserialize<Results>(response);
            await ReplyAsync(embed: BuildEmbed(results));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - {ex.Message}");
            EmbedBuilder builder = new EmbedBuilder();
            builder.AddField("I ran into an issue, so here's the link.",
                $"[Runescape Wiki - {search}](https://runescape.wiki/?search={search})");
            await ReplyAsync(embed: builder.Build());
        }
    }

    private async Task<string> CallWikiApi(string search)
    {
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        httpClientHandler.AllowAutoRedirect = false;
        HttpClient client = new HttpClient(httpClientHandler);
        client.BaseAddress = new Uri("https://runescape.wiki/");
        var path = new Uri(client.BaseAddress, $"api.php?action=opensearch&search={search}");
        var response = await client.GetAsync(path);

        string body = await response.Content.ReadAsStringAsync();
        body = FixShittyJson(body);

        return body;
    }

    private string FixShittyJson(string body)
    {
        body = body.Trim('[').Trim(']');
        body = "{ \"search\" : " + body + "]}";
        body = body.Insert(body.IndexOf('['), "\"topics\": ");
        body = body.Insert(body.IndexOf('[', body.IndexOf('[') + 1), "\"junk\": ");
        body = body.Insert(body.IndexOf('[', body.IndexOf('[', body.IndexOf('[') + 1) + 1), "\"links\": ");
        return body;
    }

    private Embed BuildEmbed(Results results)
    {
        EmbedBuilder builder = new EmbedBuilder();
        string message = "";
        if (results.Topics.Count > 0)
        {
            for (int i = 0; i < results.Topics.Count; i++)
            {
                message += $"[{results.Topics[i]}]({results.Links[i]})\n";
            }
        }
        else
        {
            message = "No results";
        }

        builder.AddField($"Search results for: {results.Search}", message);

        return builder.Build();
    }
}

public class Results
{
    [JsonPropertyName("search")]
    public string Search { get; set; }

    [JsonPropertyName("topics")]
    public List<string> Topics { get; set; }

    [JsonPropertyName("junk")]
    public List<string> Junk { get; set; }

    [JsonPropertyName("links")]
    public List<string> Links { get; set; }
}