namespace RiftBot;

public class GuildMetadataModule
{
    private readonly RiftBotContext _context;

    private readonly GuildService _guildService;

    private readonly EventService _eventService;

    public GuildMetadataModule(RiftBotContext context, GuildService guildService, EventService eventService)
    {
        _context = context;
        _guildService = guildService;
        _eventService = eventService;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "guild-member-stats",
                Description = "Check the number of each type of discord member. (Clannie, Clan Friend, no rank)",
                CommandHandler = (SocketSlashCommand command) => GetMemberCountByType(command)
            },
            new()
            {
                CommandName = "guild-members-without-roles",
                Description = "Get the discord members that do not have a \"Clannie\" or \"Clan Friend\" role",
                CommandHandler = (SocketSlashCommand command) => GetUnmarkedMembers(command)
            },
            new()
            {
                CommandName = "get-new-discord-members",
                Description = "Get a list of Discord members that have recently joined",
                CommandHandler = (SocketSlashCommand command) => GetNewMembers(command)
            },
            new()
            {
                CommandName = "get-lost-discord-members",
                Description = "Get a list of Discord members that have recently left",
                CommandHandler = (SocketSlashCommand command) => GetLostMembers(command)
            }
        };
    }

    public async Task GetMemberCountByType(SocketSlashCommand command)
    {
        List<GuildMember> guildMembers = await _guildService.GetGuildMembersAsync().ConfigureAwait(false);

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

        await command.ModifyOriginalResponseAsync(x => x.Content = $"{guildMembers.Count} Members\n{clannies} Clannies\n{clanFriends} Clan Friends\n{unknown} Not Marked").ConfigureAwait(false);
    }

    public async Task GetUnmarkedMembers(SocketSlashCommand command)
    {
        List<GuildMember> guildMembers = await _guildService.GetGuildMembersAsync().ConfigureAwait(false);

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

        await command.ModifyOriginalResponseAsync(x => x.Content = sb.ToString()).ConfigureAwait(false);
    }

    public async Task GetNewMembers(SocketSlashCommand command)
    {
        List<EventLog> eventLogs = await _eventService.GetEventLogs(x => x.Event.Name == Events.UserJoined).ConfigureAwait(false);
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
        await command.ModifyOriginalResponseAsync(x => x.Content = sb.ToString()).ConfigureAwait(false);
    }

    public async Task GetLostMembers(SocketSlashCommand command)
    {
        List<EventLog> eventLogs = await _eventService.GetEventLogs(x => x.Event.Name == Events.UserLeft).ConfigureAwait(false);
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
        await command.ModifyOriginalResponseAsync(x => x.Content = sb.ToString()).ConfigureAwait(false);
    }
}