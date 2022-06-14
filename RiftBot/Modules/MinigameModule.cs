namespace RiftBot.Modules;

public class MinigameModule : ModuleBase<SocketCommandContext>
{
    [Command("runesphere", RunMode = RunMode.Async)]
    [Summary("!runesphere - Finds the next spawn time on world 59")]
    public async Task GetNextRunesphere()
    {
        TimeSpan runesphereTimer = new TimeSpan(0, 0, 9053);
        DateTime start = new DateTime(2022, 06, 13, 12, 33, 09, DateTimeKind.Utc);
        DateTime next = start.Add(runesphereTimer);
        while (next < DateTime.UtcNow)
        {
            next = next.Add(runesphereTimer);
        }

        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
        builder.WithThumbnailUrl("https://runescape.wiki/images/thumb/Runesphere_(air).png/31px-Runesphere_(air).png");
        builder.AddField(build =>
        {
            build.WithName("Next World 59 Runesphere");
            build.WithValue($"{next:yyyy/MM/dd HH:mm:ss} UTC");
        });
        builder.Footer = new EmbedFooterBuilder();
        builder.Footer.Text = "There may be some drift in the timing.\n" +
            "If so, notify leadership.";
        await ReplyAsync(embed: builder.Build());
    }
}