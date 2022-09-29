namespace RiftBot;

public class RiftBot
{
    private readonly ILogger<RiftBot> _logger;
    private readonly CommandService _commandService;
    private readonly CommandHandlingService _commandHandlingService;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IConfiguration _config;

    public RiftBot(ILogger<RiftBot> logger, IConfiguration config, CommandHandlingService commandHandlingService, CommandService commandService, DiscordSocketClient discordSocketClient)
    {
        _logger = logger;
        _config = config;
        _commandService = commandService;
        _commandHandlingService = commandHandlingService;
        _discordSocketClient = discordSocketClient;

        _logger.LogInformation(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"));
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
            _logger.LogInformation($"{DateTime.Now:G} - RiftBot.Run: {ex}");
        }
    }

    private Task LogAsync(LogMessage log)
    {
        if (string.IsNullOrEmpty(log.Message)) return Task.CompletedTask;

        _logger.LogInformation($"{DateTime.Now:G} - {log.Message}");

        return Task.CompletedTask;
    }
}