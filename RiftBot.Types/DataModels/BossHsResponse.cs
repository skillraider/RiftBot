using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RiftBot.Types
{
    public class BossHsResponse
    {
        [JsonPropertyName("content")]
        public List<RecordRow> Content { get; set; }

        [JsonPropertyName("totalElements")]
        public int TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("first")]
        public bool First { get; set; }

        [JsonPropertyName("last")]
        public bool Last { get; set; }

        [JsonPropertyName("numberOfElements")]
        public int NumberOfElements { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("empty")]
        public bool Empty { get; set; }
    }

    public class RecordRow
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("bossId")]
        public int BossId { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("enrage")]
        public int Enrage { get; set; }

        [JsonPropertyName("killTimeSeconds")]
        public double KillTimeInSeconds { get; set; }

        [JsonPropertyName("timeOfKill")]
        public long TimeOfKill { get; set; }

        [JsonPropertyName("members")]
        public List<KillMembers> Members { get; set; }
    }

    public class KillMembers
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}