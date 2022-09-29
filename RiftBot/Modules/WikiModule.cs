namespace RiftBot;

public class WikiModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<WikiModule> _logger;

    public WikiModule(ILogger<WikiModule> logger)
    {
        _logger = logger;
    }

    [Command("wiki-search", RunMode = RunMode.Async)]
    [Summary("!wiki-search <search>")]
    public async Task SearchWiki([Remainder] string search)
    {
        try
        {
            string response = await CallWikiApi(search).ConfigureAwait(false);
            Results results = JsonSerializer.Deserialize<Results>(response);
            await ReplyAsync(embed: BuildEmbed(results)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now:G} - {ex.Message}");
            EmbedBuilder builder = new EmbedBuilder();
            builder.AddField("I ran into an issue, so here's the link.",
                $"[Runescape Wiki - {search}](https://runescape.wiki/?search={search})");
            await ReplyAsync(embed: builder.Build()).ConfigureAwait(false);
        }
    }

    private static async Task<string> CallWikiApi(string search)
    {
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        httpClientHandler.AllowAutoRedirect = false;
        httpClientHandler.CheckCertificateRevocationList = true;
        HttpClient client = new HttpClient(httpClientHandler);
        client.BaseAddress = new Uri("https://runescape.wiki/");
        var path = new Uri(client.BaseAddress, $"api.php?action=opensearch&search={search}");
        var response = await client.GetAsync(path).ConfigureAwait(false);

        string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        client.Dispose();
        httpClientHandler.Dispose();
        body = FixShittyJson(body);

        return body;
    }

    private static string FixShittyJson(string body)
    {
        body = body.Trim('[').Trim(']');
        body = "{ \"search\" : " + body + "]}";
        body = body.Insert(body.IndexOf('['), "\"topics\": ");
        body = body.Insert(body.IndexOf('[', body.IndexOf('[') + 1), "\"junk\": ");
        body = body.Insert(body.IndexOf('[', body.IndexOf('[', body.IndexOf('[') + 1) + 1), "\"links\": ");
        return body;
    }

    private static Embed BuildEmbed(Results results)
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