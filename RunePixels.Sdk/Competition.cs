using System.Text.Json.Serialization;

namespace RunePixels.Sdk;

public class Competition
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("pin")]
    public string Pin { get; set; } = "";

    [JsonPropertyName("activity")]
    public int Activity { get; set; }

    [JsonPropertyName("isTeam")]
    public bool IsTeam { get; set; }

    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("clanID")]
    public int ClanID { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; } = "";
}