namespace RiftBot.Modules;

public class SettingModule
{
    private readonly RiftBotContext _context;

    public SettingModule(RiftBotContext context)
    {
        _context = context;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "get-settings",
                Description = "Get RiftBot settings",
                CommandHandler = (SocketSlashCommand command) => GetSettings(command),
                Options = new()
                {
                    new()
                    {
                        Name = "settingname",
                        Description = "Setting name",
                        Type = ApplicationCommandOptionType.String,
                        Required = false
                    }
                }
            },
            new()
            {
                CommandName = "set-setting",
                Description = "Set RiftBot setting",
                CommandHandler = (SocketSlashCommand command) => ChangeSetting(command),
                Options = new()
                {
                    new()
                    {
                        Name = "settingname",
                        Description = "Setting name",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    },
                    new()
                    {
                        Name = "settingvalue",
                        Description = "Setting value",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    }
                }
            }
        };
    }

    public async Task GetSettings(SocketSlashCommand command)
    {
        string settingName = command.Data.Options.FirstOrDefault(x => x.Name == "settingname")?.Value?.ToString();
        if (!string.IsNullOrEmpty(settingName))
        {
            BotSetting botSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name.ToLower() == settingName.ToLower()).ConfigureAwait(false);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{botSetting.Name}: {botSetting.Value}").ConfigureAwait(false);
        }
        else
        {
            List<BotSetting> botSettings = await _context.BotSettings.ToListAsync().ConfigureAwait(false);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("```");

            foreach (BotSetting botSetting in botSettings)
            {
                sb.AppendLine($"{botSetting.Name}: {botSetting.Value}");
            }

            sb.AppendLine("```");
            await command.ModifyOriginalResponseAsync(x => x.Content = sb.ToString()).ConfigureAwait(false);
        }
    }

    public async Task ChangeSetting(SocketSlashCommand command)
    {
        string settingName = command.Data.Options.FirstOrDefault(x => x.Name == "settingname")?.Value?.ToString();
        string settingValue = command.Data.Options.FirstOrDefault(x => x.Name == "settingvalue")?.Value?.ToString();
        if (string.IsNullOrEmpty(settingName))
        {
            await command.ModifyOriginalResponseAsync(x => x.Content = "Invalid setting name").ConfigureAwait(false);
        }

        BotSetting botSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name.ToLower() == settingName.ToLower()).ConfigureAwait(false);
        if (botSetting is null)
        {
            await command.ModifyOriginalResponseAsync(x => x.Content = $"Setting name ({settingName}) does not match any existing settings").ConfigureAwait(false);
            return;
        }

        botSetting.Value = settingValue;
        _context.BotSettings.Update(botSetting);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        await command.ModifyOriginalResponseAsync(x => x.Content = $"{botSetting.Name} changed to {botSetting.Value}").ConfigureAwait(false);
    }
}