namespace RiftBot.Types;

public class EventLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Discriminator { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; }

    [Required]
    public int EventId { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; }
}