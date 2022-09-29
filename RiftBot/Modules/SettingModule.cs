namespace RiftBot.Modules;

public class SettingModule : ModuleBase<SocketCommandContext>
{
    private readonly RiftBotContext _context;

    public SettingModule(RiftBotContext context)
    {
        _context = context;
    }

    [RequireOwner]
    [Command("settings", RunMode = RunMode.Async)]
    [Summary("!settings [\"settingName\"] - Gets a list of all bot settings")]
    public async Task GetSettings(string settingName = "")
    {
        if (!string.IsNullOrEmpty(settingName))
        {
            BotSetting botSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name.ToLower() == settingName.ToLower()).ConfigureAwait(false);
            await ReplyAsync($"{botSetting.Name}: {botSetting.Value}").ConfigureAwait(false);
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
            await ReplyAsync(sb.ToString()).ConfigureAwait(false);
        }
    }

    [RequireOwner]
    [Command("setting", RunMode = RunMode.Async)]
    [Summary("!setting \"settingName\" \"settingValue\" - Change a setting value")]
    public async Task ChangeSetting(string settingName, string settingValue)
    {
        if (string.IsNullOrEmpty(settingName))
        {
            await ReplyAsync("Invalid setting name").ConfigureAwait(false);
        }

        BotSetting botSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name.ToLower() == settingName.ToLower()).ConfigureAwait(false);
        if (botSetting == null)
        {
            await ReplyAsync($"Setting name ({settingName}) does not match any existing settings").ConfigureAwait(false);
        }

        botSetting.Value = settingValue;
        _context.BotSettings.Update(botSetting);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        await ReplyAsync($"{botSetting.Name} changed to {botSetting.Value}").ConfigureAwait(false);
    }
}