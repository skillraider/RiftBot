namespace RiftBot;

public class GuildMetadataModule : ModuleBase<SocketCommandContext>
{
    private readonly RiftBotContext _context;

    public GuildService GuildService { get; set; }

    public EventService EventService { get; set; }

    public GuildMetadataModule(RiftBotContext context)
    {
        _context = context;
    }

    [Command("memberstats", RunMode = RunMode.Async)]
    [Summary("Admin: !memberstats - Returns the number of each type of discord member. (Clannie, Clan Friend, no rank)")]
    public async Task GetMemberCountByType()
    {
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel");
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild");
        if (Context.Channel.Name != restrictedCommandChannelSetting.Value && Context.Guild.Id != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        List<GuildMember> guildMembers = await GuildService.GetGuildMembersAsync();

        int clannies = 0;
        int clanFriends = 0;
        int unknown = 0;
        foreach (GuildMember member in guildMembers)
        {
            if (member.GuildRoles.Any(x => x.Name == "Clannie"))
            {
                clannies++;
            }
            else if (member.GuildRoles.Any(y => y.Name == "Clan Friend"))
            {
                clanFriends++;
            }
            else
            {
                unknown++;
            }
        }

        await ReplyAsync($"{guildMembers.Count} Members\n{clannies} Clannies\n{clanFriends} Clan Friends\n{unknown} Not Marked");
    }

    [Command("getunmarked", RunMode = RunMode.Async)]
    [Summary("Admin: !getunmarked - Returns the discord members that do not have a \"Clannie\" or \"Clan Friend\" rank")]
    public async Task GetUnmarkedMembers()
    {
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel");
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild");
        if (Context.Channel.Name != restrictedCommandChannelSetting.Value && Context.Guild.Id != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        List<GuildMember> guildMembers = await GuildService.GetGuildMembersAsync();

        StringBuilder sb = new StringBuilder();
        foreach (GuildMember member in guildMembers.OrderBy(x => x.Nickname).ThenBy(x => x.User.Username))
        {
            if (member.GuildRoles.All(x => x.Name != "Clannie") && member.GuildRoles.All(x => x.Name != "Clan Friend"))
            {
                sb.Append($"{member.User.Username} #{member.User.Discriminator}");

                if (!string.IsNullOrEmpty(member.Nickname))
                {
                    sb.AppendLine($" - {member.Nickname}");
                }
                else
                {
                    sb.AppendLine();
                }
            }
        }

        await ReplyAsync(sb.ToString());
    }

    [Command("newmembers", RunMode = RunMode.Async)]
    [Summary("Admin: !newmembers - Gets a list of new members")]
    public async Task GetNewMembers()
    {
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel");
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild");
        if (Context.Channel.Name != restrictedCommandChannelSetting.Value && Context.Guild.Id != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        List<EventLog> eventLogs = await EventService.GetEventLogs(x => x.Event.Name == Events.UserJoined);
        eventLogs = eventLogs.OrderByDescending(x => x.Timestamp).ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("```");

        foreach (EventLog eventLog in eventLogs)
        {
            string line = $"{eventLog.Timestamp}: User Joined - {eventLog.Username} #{eventLog.Discriminator}";
            // Discord has a 2000 character message limit.
            if (sb.Length + line.Length < 1995)
            {
                sb.AppendLine(line);
            }
            else
            {
                break;
            }
        }

        sb.AppendLine("```");
        await ReplyAsync(sb.ToString());
    }

    [Command("lostmembers", RunMode = RunMode.Async)]
    [Summary("Admin: !lostmembers - Gets a list of members who left")]
    public async Task GetLostMembers()
    {
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel");
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild");
        if (Context.Channel.Name != restrictedCommandChannelSetting.Value && Context.Guild.Id != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        List<EventLog> eventLogs = await EventService.GetEventLogs(x => x.Event.Name == Events.UserLeft);
        eventLogs = eventLogs.OrderByDescending(x => x.Timestamp).ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("```");

        foreach (EventLog eventLog in eventLogs)
        {
            string line = $"{eventLog.Timestamp}: User Left - {eventLog.Username} #{eventLog.Discriminator}";
            // Discord has a 2000 character message limit.
            if (sb.Length + line.Length < 1995)
            {
                sb.AppendLine(line);
            }
            else
            {
                break;
            }
        }

        sb.AppendLine("```");
        await ReplyAsync(sb.ToString());
    }
}