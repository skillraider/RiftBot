namespace RiftBot;

public class SlashCommandOption
{
    private string name = "";
    public string Name
    {
        get => name;
        set => name = value.ToLower();
    }

    public string Description { get; set; } = "";

    public bool Required { get; set; } = false;

    public ApplicationCommandOptionType Type { get; set; } = ApplicationCommandOptionType.String;
}