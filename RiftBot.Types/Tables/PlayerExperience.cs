namespace RiftBot.Types;

public class PlayerExperience
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PlayerId { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public Player Player { get; set; }

    [Required]
    [Column(TypeName = "jsonb")]
    public SkillStats[] SkillStats { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; }
}