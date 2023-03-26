using System.Text.Json.Serialization;

namespace RunePixels.Sdk;

public class CreateCompetition
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("pin")]
    public string Pin { get; set; } = "";

    [JsonPropertyName("isTeam")]
    public bool IsTeam { get; set; }

    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    [JsonPropertyName("clanID")]
    public int ClanID { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; } = "";

    [JsonPropertyName("participants")]
    public List<string> Participants { get; set; } = new();
}