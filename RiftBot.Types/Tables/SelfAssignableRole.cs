namespace RiftBot.Types;

public class SelfAssignableRole
{
    [Key]
    public int Id { get; set; }

    [Required]
    public ulong MessageId { get; set; }

    [Required]
    public string Role { get; set; }

    [Required]
    public string Emote { get; set; }
}