namespace RiftBot;

public class ReactionRolesModule : ModuleBase<SocketCommandContext>
{
    private readonly RiftBotContext _context;

    private readonly SelfAssignRoleService _selfAssignRoleService;

    public ReactionRolesModule(RiftBotContext context, SelfAssignRoleService selfAssignRoleService)
    {
        _context = context;
        _selfAssignRoleService = selfAssignRoleService;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "create-reaction-role",
                Description = "Create an entry so a user can react to a specific message with a specific emote to gain a role",
                CommandHandler = (SocketSlashCommand command) => CreateReactionRole(command),
                Options = new()
                {
                    new()
                    {
                        Name = "message-id",
                        Description = "The id of the message users must react to",
                        Type = ApplicationCommandOptionType.String,
                        Required = true,
                    },
                    new()
                    {
                        Name = "role",
                        Description = "The role users will gain",
                        Type = ApplicationCommandOptionType.Role,
                        Required = true,
                    },
                    new()
                    {
                        Name = "emote",
                        Description = "The emote users must react with",
                        Type = ApplicationCommandOptionType.String,
                        Required = true,
                    }
                }
            }
        };
    }

    public async Task CreateReactionRole(SocketSlashCommand command)
    {
        ulong messageId = ulong.Parse(command.Data.Options.First(x => x.Name == "message-id").Value.ToString());
        IRole role = command.Data.Options.First(x => x.Name == "role").Value as IRole;
        string emote = command.Data.Options.First(x => x.Name == "emote").Value.ToString();
        await _selfAssignRoleService.CreateReactionRole(messageId, role.Mention, emote).ConfigureAwait(false);
        await command.ModifyOriginalResponseAsync(x => x.Content = "Reaction role created!");
    }
}