using System.Text.Json.Serialization;

namespace RiftBot.Types
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        [JsonPropertyName("bot")]
        public bool Bot { get; set; }
    }
}