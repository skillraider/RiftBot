using System.Reflection;

namespace RiftBot;

public class CommandHandlingService
{
    private readonly IServiceProvider _services;

    private readonly RiftBotContext _context;
    private readonly CommandService _commandService;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly SelfAssignRoleService _selfAssignRoleService;

    public CommandHandlingService(IServiceProvider services, CommandService commandService,
        DiscordSocketClient discordSocketClient, SelfAssignRoleService selfAssignRoleService,
        RiftBotContext context)
    {
        _services = services;

        _context = context;
        _commandService = commandService;
        _discordSocketClient = discordSocketClient;
        _selfAssignRoleService = selfAssignRoleService;

        _discordSocketClient.Ready += Ready;
        _discordSocketClient.ReactionAdded += ReactionAdded;
        _discordSocketClient.ReactionRemoved += ReactionRemoved;
        _discordSocketClient.MessageReceived += MessageReceivedAsync;
        _discordSocketClient.UserJoined += UserJoined;
        _discordSocketClient.UserLeft += UserLeft;
    }

    private async Task UserJoined(SocketGuildUser arg)
    {
        try
        {
            int eventId = await _context.Event.Where(x => x.Name == Events.UserJoined).Select(x => x.Id).FirstOrDefaultAsync();

            _context.EventLog.Add(new()
            {
                Username = arg.Username,
                Discriminator = arg.Discriminator,
                EventId = eventId,
                Timestamp = DateTimeOffset.UtcNow
            });

            Console.WriteLine($"{DateTime.Now:G} - New member: {arg.Username} #{arg.Discriminator}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - You fucked something up!");
            Console.WriteLine($"{DateTime.Now:G} - {ex.Message}");
        }
    }

    private async Task UserLeft(SocketGuildUser arg)
    {
        try
        {
            int eventId = await _context.Event.Where(x => x.Name == Events.UserLeft).Select(x => x.Id).FirstOrDefaultAsync();

            _context.EventLog.Add(new()
            {
                Username = arg.Username,
                Discriminator = arg.Discriminator,
                EventId = eventId,
                Timestamp = DateTimeOffset.UtcNow
            });

            Console.WriteLine($"{DateTime.Now:G} - Member left: {arg.Username} #{arg.Discriminator}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - You fucked something up!");
            Console.WriteLine($"{DateTime.Now:G} - {ex.Message}");
        }
    }

    public async Task InitializeAsync()
    {
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task Ready()
    {
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(environment) || environment.ToLower() != "sandbox") return;

        SocketGuild guild = _discordSocketClient.Guilds.FirstOrDefault(x => x.Name == "RiftBot");
        SocketGuildChannel channel = guild?.Channels.FirstOrDefault(x => x.Name == "bot-sandbox");
        if (channel is null) return;
        await (channel as ISocketMessageChannel).SendMessageAsync("Ready");
    }

    private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
    {
        if (reaction.UserId == _discordSocketClient.CurrentUser.Id) return;

        await _selfAssignRoleService.AssignRole(channel, reaction);
    }

    private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot) return;

        await _selfAssignRoleService.RemoveRole(channel, reaction);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        // Ignore system messages, or messages from other bots
        if (!(rawMessage is SocketUserMessage message)) return;
        if (message.Source != MessageSource.User) return;

        // This value holds the offset where the prefix ends
        var argPos = 0;
        // Perform prefix check
        if (!message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos) &&
            !message.HasCharPrefix('!', ref argPos)) return;

        var context = new SocketCommandContext(_discordSocketClient, message);

        await _commandService.ExecuteAsync(context, argPos, _services);
    }

    //private Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    //{
    //    // command is unspecified when there was a search failure (command not found); we don't care about these errors
    //    if (!command.IsSpecified)
    //    {
    //        Console.WriteLine($"{DateTime.Now:G}: Unspecified command from {context.User.Username} - {context.Message}");
    //        return Task.CompletedTask;
    //    }

    //    // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
    //    if (result.IsSuccess)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    // the command failed, let's notify the user that something happened.
    //    //await context.Channel.SendMessageAsync($"error: {result}");
    //    return Task.CompletedTask;
    //}
}