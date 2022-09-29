namespace RiftBot;

public class AdminModule : ModuleBase<SocketCommandContext>
{
    private readonly IConfiguration _config;

    public AdminModule(IConfiguration config)
    {
        _config = config;
    }

    [Command("purge", RunMode = RunMode.Async)]
    [RequireOwner]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    [Summary("!purge [numberOfMessages] - Purges messages in the channel (only up to 14 days old)")]
    public async Task PurgeChat(int number = int.MaxValue)
    {
        var warning = await ReplyAsync("Can only delete messages from the last 14 days").ConfigureAwait(false);
        await Task.Delay(2000).ConfigureAwait(false);
        var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before).FlattenAsync().ConfigureAwait(false);
        messages = messages.Where(x => (DateTime.UtcNow - x.Timestamp).TotalDays <= 14);
        messages = messages.Take(number);
        await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages).ConfigureAwait(false);

        await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
        await Context.Channel.DeleteMessageAsync(warning).ConfigureAwait(false);
    }

    [Command("startscheduler", RunMode = RunMode.Async)]
    [RequireOwner]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    public async Task StartScheduler()
    {
        _config.GetSection("RunScheduler").Value = "true";
        await ReplyAsync("Scheduler has been started").ConfigureAwait(false);
    }

    [Command("stopscheduler", RunMode = RunMode.Async)]
    [RequireOwner]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    public async Task StopScheduler()
    {
        _config.GetSection("RunScheduler").Value = "false";
        await ReplyAsync("Scheduler has been stopped").ConfigureAwait(false);
    }
}