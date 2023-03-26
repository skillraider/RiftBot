using System.Text.Json.Serialization;

namespace RunePixels.Sdk;

public class CompetitionSearch
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("playerSubType")]
    public int PlayerSubType { get; set; }

    [JsonPropertyName("playerType")]
    public int PlayerType { get; set; }

    [JsonPropertyName("skills")]
    public List<string> Skills { get; set; } = new();
}