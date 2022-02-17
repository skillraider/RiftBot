namespace RiftBot;

public class MemeModule : ModuleBase<SocketCommandContext>
{
    public HiscoreService HiscoreService { get; set; }

    [Command("runescore", RunMode = RunMode.Async)]
    [Summary("!runescore <name> - Do you have a better RuneScore than Denkir?")]
    public async Task GetGood([Remainder] string name)
    {
        PlayerActivities denkirActivities = await HiscoreService.GetPlayerActivities("denkir");
        PlayerActivities playerActivities = await HiscoreService.GetPlayerActivities(name.ToLower());
        var denkirScore = denkirActivities.ActivityStats[24].Total;
        var otherScore = playerActivities.ActivityStats[24].Total;

        if (denkirScore > otherScore)
        {
            await ReplyAsync($"Denkir: {denkirScore:N0} > {name}: {otherScore:N0}");
            await ReplyAsync("YOU SUCK!");
        }
        else if (denkirScore < otherScore)
        {
            await ReplyAsync($"Denkir: {denkirScore:N0} < {name}: {otherScore:N0}");
            await ReplyAsync("*Sad wolf noises*");
        }
        else
        {
            await ReplyAsync($"Denkir: {denkirScore:N0} = {name}: {otherScore:N0}");
            await ReplyAsync("*Confused wolf noises*");
        }
    }
}