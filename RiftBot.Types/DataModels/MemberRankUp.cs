namespace RiftBot.Types;

public class MemberRankUp
{
    public string MemberName { get; set; }

    public string CurrentRank { get; set; }

    public string NewRank { get; set; }

    public long CurrentXp { get; set; }

    public long RequiredXp { get; set; }
}