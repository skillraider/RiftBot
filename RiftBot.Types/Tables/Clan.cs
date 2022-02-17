namespace RiftBot.Types;

public class Clan
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int NumberOfMembers { get; set; }

    [Required]
    public DateTimeOffset StartedTracking { get; set; }

    [Required]
    public DateTimeOffset LastUpdated { get; set; }
}