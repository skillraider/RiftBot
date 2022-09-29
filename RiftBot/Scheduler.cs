using Timer = System.Timers.Timer;

namespace RiftBot;

public class Scheduler
{
    private readonly ILogger<Scheduler> _logger;
    private readonly IConfiguration _config;
    private readonly ClanService _clanService;

    public Scheduler(ILogger<Scheduler> logger, IConfiguration config, ClanService clanService)
    {
        _logger = logger;
        _config = config;
        _clanService = clanService;
    }

    public async Task StartAsync()
    {
        Timer timer = new()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 1000 * 60
        };
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (_config.GetSection("RunScheduler").Value == "true")
        {
            await RunUpdate();
        }
    }

    private async Task RunUpdate()
    {
        try
        {
            if (DateTimeOffset.UtcNow.Hour == 0 && DateTimeOffset.UtcNow.Minute >= 5 && _config.GetSection("RunUpdate").Value == "true")
            {
                _logger.LogInformation($"{DateTime.Now:G} - Updating clan members");
                _config.GetSection("RunUpdate").Value = "false";
                await _clanService.UpdateClanMembers();
            }

            if (DateTimeOffset.UtcNow.Hour != 0 && _config.GetSection("RunUpdate").Value == "false")
            {
                _config.GetSection("RunUpdate").Value = "true";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now:G} - An error occurred while trying to update clan members");
            _logger.LogError($"{DateTime.Now:G} - {ex.Message}");
        }
    }
}