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
        try
        {
            Console.WriteLine($"{DateTime.Now:G} - {Context.User.Username}: {Context.Message}");
            var warning = await ReplyAsync("Can only delete messages from the last 14 days");
            await Task.Delay(2000);
            var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before).FlattenAsync();
            messages = messages.Where(x => (DateTime.UtcNow - x.Timestamp).TotalDays <= 14);
            messages = messages.Take(number);
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.DeleteMessageAsync(warning);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - Error:\n\t{Context.User.Username}: {Context.Message}\n\t{ex.Message}");
        }
    }

    [Command("startscheduler", RunMode = RunMode.Async)]
    [RequireOwner]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    public async Task StartScheduler()
    {
        _config.GetSection("RunScheduler").Value = "true";
        await ReplyAsync("Scheduler has been started");
    }

    [Command("stopscheduler", RunMode = RunMode.Async)]
    [RequireOwner]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    public async Task StopScheduler()
    {
        _config.GetSection("RunScheduler").Value = "false";
        await ReplyAsync("Scheduler has been stopped");
    }
}