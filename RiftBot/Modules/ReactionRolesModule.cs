namespace RiftBot;

public class ReactionRolesModule : ModuleBase<SocketCommandContext>
{
    private readonly RiftBotContext _context;

    public SelfAssignRoleService SelfAssignRoleService { get; set; }

    public ReactionRolesModule(RiftBotContext context)
    {
        _context = context;
    }

    [Command("createreactionrole", RunMode = RunMode.Async)]
    [Summary("Admin: !createreactionrole <message_id> <role> <emote>")]
    public async Task CreateReactionRole(ulong messageId, string role, string emote)
    {
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel");
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild");
        if (Context.Channel.Name != restrictedCommandChannelSetting.Value && Context.Guild.Id != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        await SelfAssignRoleService.CreateReactionRole(messageId, role, emote);
    }
}