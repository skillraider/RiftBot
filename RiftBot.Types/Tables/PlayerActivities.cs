namespace RiftBot.Types;

public class PlayerActivities
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PlayerId { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public Player Player { get; set; }

    [Required]
    [Column(TypeName = "jsonb")]
    public ActivityStats[] ActivityStats { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; }
}