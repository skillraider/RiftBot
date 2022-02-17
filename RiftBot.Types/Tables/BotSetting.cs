namespace RiftBot.Types;

public class BotSetting
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Value { get; set; }
}