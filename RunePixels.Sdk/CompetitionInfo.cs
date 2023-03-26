using System.Text.Json.Serialization;

namespace RunePixels.Sdk;

public class CompetitionInfo
{
    [JsonPropertyName("competition")]
    public Competition Competition { get; set; } = new();

    [JsonPropertyName("players")]
    public List<Player> Players { get; set; } = new();
}