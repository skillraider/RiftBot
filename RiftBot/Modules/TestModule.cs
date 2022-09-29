namespace RiftBot;

public class TestModule : ModuleBase<SocketCommandContext>
{
    public async Task PingBossRole(string roleName, string time)
    {
        var aodRole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == roleName.ToLower());
        if (aodRole is null)
        {
            await ReplyAsync($"Cannot find role called {roleName}").ConfigureAwait(false);
        }

        var message = await ReplyAsync($"{aodRole.Mention} event starting at {time} game time, react if you plan on joining or not").ConfigureAwait(false);
        await message.AddReactionsAsync(new[] { new Emoji("\uD83D\uDC4D"), new Emoji("\uD83D\uDC4E") }).ConfigureAwait(false);
    }
}