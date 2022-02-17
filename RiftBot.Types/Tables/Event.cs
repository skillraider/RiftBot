namespace RiftBot.Types;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}