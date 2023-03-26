using System.Text.Json.Serialization;

namespace RunePixels.Sdk;

public class Player
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("startXP")]
    public long StartXP { get; set; }

    [JsonPropertyName("xp")]
    public long Experience { get; set; }

    [JsonPropertyName("playerType")]
    public int PlayerType { get; set; }

    //[JsonPropertyName("nameStyle")]
    //public string NameStyle { get; set; } = "";
}