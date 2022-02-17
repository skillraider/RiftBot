using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RiftBot.Types
{
    public class GuildMember
    {
        [JsonPropertyName("nick")]
        public string Nickname { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("roles")]
        public string[] Roles { get; set; }

        [JsonPropertyName("guildroles")]
        public List<GuildRole> GuildRoles { get; set; } = new();
    }
}