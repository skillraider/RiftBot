#nullable enable

namespace RiftBot;

public class SlashCommand
{
    public string CommandName { get; set; } = "";

    public string Description { get; set; } = "";

    public Func<SocketSlashCommand, Task>? CommandHandler { get; set; }

    public List<SlashCommandOption>? Options { get; set; } = new();
}