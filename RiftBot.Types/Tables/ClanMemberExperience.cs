namespace RiftBot.Types;

public class ClanMemberExperience
{
    [Key]
    public int Id { get; set; }

    [Required]
    public long ClanExperience { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; }

    [Required]
    public int ClanMemberId { get; set; }

    [ForeignKey(nameof(ClanMemberId))]
    public ClanMember ClanMember { get; set; }
}