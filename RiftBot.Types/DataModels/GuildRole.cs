using System.Text.Json.Serialization;

namespace RiftBot.Types
{
    public class GuildRole
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}