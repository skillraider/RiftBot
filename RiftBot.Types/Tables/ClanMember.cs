namespace RiftBot.Types;

public class ClanMember
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string ClanRank { get; set; }

    [Required]
    public long ClanExperience { get; set; }

    [Required]
    public bool HasLeftClan { get; set; }

    [Required]
    [DefaultValue(false)]
    public bool PreferPvm { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; }

    [Required]
    public int ClanId { get; set; }

    [ForeignKey(nameof(ClanId))]
    public Clan Clan { get; set; }
}