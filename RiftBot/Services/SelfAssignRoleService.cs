namespace RiftBot;

public class SelfAssignRoleService
{
    private readonly RiftBotContext _context;

    public SelfAssignRoleService(RiftBotContext context)
    {
        _context = context;
    }

    public async Task AssignRole(IMessageChannel channel, SocketReaction reaction)
    {
        try
        {
            List<SelfAssignableRole> reactionRoles = await _context.SelfAssignableRole.AsNoTracking()
                .Where(x => x.MessageId == reaction.MessageId).ToListAsync();

            if (reactionRoles.Count <= 0) return;

            SocketGuildChannel schannel = channel as SocketGuildChannel;
            List<SocketRole> roles = schannel?.Guild.Roles.ToList();
            if (roles is null || roles.Count <= 0) return;

            SocketRole roleToAssign = null;

            foreach (SelfAssignableRole selfAssignableRole in reactionRoles)
            {
                Emote emote = Emote.Parse(selfAssignableRole.Emote);
                if (emote is null) continue;

                if (emote.Name == reaction.Emote.Name)
                {
                    ulong roleId = MentionUtils.ParseRole(selfAssignableRole.Role);
                    roleToAssign = roles.FirstOrDefault(x => x.Id == roleId);
                    break;
                }
            }

            if (roleToAssign is not null)
            {
                await (reaction.User.Value as SocketGuildUser).AddRoleAsync(roleToAssign);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task RemoveRole(IMessageChannel channel, SocketReaction reaction)
    {
        try
        {
            List<SelfAssignableRole> reactionRoles = await _context.SelfAssignableRole.AsNoTracking()
            .Where(x => x.MessageId == reaction.MessageId).ToListAsync();

            SocketGuildChannel schannel = channel as SocketGuildChannel;
            List<SocketRole> roles = schannel.Guild.Roles.ToList();

            SocketRole roleToAssign = null;

            foreach (SelfAssignableRole selfAssignableRole in reactionRoles)
            {
                Emote emote = Emote.Parse(selfAssignableRole.Emote);
                if (emote.Name == reaction.Emote.Name)
                {
                    ulong roleId = MentionUtils.ParseRole(selfAssignableRole.Role);
                    roleToAssign = roles.FirstOrDefault(x => x.Id == roleId);
                    break;
                }
            }

            if (roleToAssign != null)
            {
                await (reaction.User.Value as SocketGuildUser).RemoveRoleAsync(roleToAssign);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task CreateReactionRole(ulong messageId, string role, string emote)
    {
        await _context.SelfAssignableRole.AddAsync(new SelfAssignableRole()
        {
            MessageId = messageId,
            Role = role,
            Emote = emote
        });
        await _context.SaveChangesAsync();
    }
}