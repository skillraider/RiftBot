namespace RiftBot;

public class RiftBot
{
    private readonly CommandService _commandService;
    private readonly CommandHandlingService _commandHandlingService;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IConfiguration _config;

    public RiftBot(CommandService commandService, CommandHandlingService commandHandlingService, DiscordSocketClient discordSocketClient, IConfiguration config)
    {
        _config = config;
        _commandService = commandService;
        _commandHandlingService = commandHandlingService;
        _discordSocketClient = discordSocketClient;

        Console.WriteLine(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"));
    }

    public async Task Run()
    {
        try
        {
            _discordSocketClient.Log += LogAsync;
            _commandService.Log += LogAsync;

            await _discordSocketClient.LoginAsync(TokenType.Bot, _config.GetSection("Token").Value);
            await _discordSocketClient.StartAsync();

            await _commandHandlingService.InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - RiftBot.Run: {ex}");
        }
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine($"{DateTime.Now:G} - {log.Message}\n\t{log.Exception}");

        return Task.CompletedTask;
    }
}