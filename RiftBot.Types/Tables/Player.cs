namespace RiftBot.Types;

public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public bool IsIronman { get; set; }

    [Required]
    public bool IsHardcore { get; set; }

    [Required]
    public DateTimeOffset StartedTracking { get; set; }

    [Required]
    public DateTimeOffset LastUpdated { get; set; }
}