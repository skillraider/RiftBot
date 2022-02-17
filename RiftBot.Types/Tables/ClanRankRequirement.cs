namespace RiftBot.Types;

public class ClanRankRequirement
{
    [Key]
    public int Id { get; set; }

    [Required]
    public ClanRanks ClanRank { get; set; }

    [Required]
    public long ClanExperience { get; set; }

    [Required]
    public int ClanId { get; set; }

    [ForeignKey(nameof(ClanId))]
    public Clan Clan { get; set; }
}