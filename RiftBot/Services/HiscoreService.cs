namespace RiftBot;

public class HiscoreService
{
    private readonly RiftBotContext _context;
    private readonly IHiscoresApi _hiscoresApi;

    public HiscoreService(RiftBotContext context, IHiscoresApi hiscoresApi)
    {
        _context = context;
        _hiscoresApi = hiscoresApi;
    }

    public async Task<PlayerExperience> GetPlayerStats(string playerName)
    {
        PlayerExperience playerExperience;
        var a = await _context.Player.ToListAsync();
        Player player = await _context.Player.FirstOrDefaultAsync(x => x.Name.ToLower() == playerName.ToLower());
        if (player is null)
        {
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            player = new Player()
            {
                Name = playerName,
                StartedTracking = timestamp
            };
            await _context.Player.AddAsync(player);
            await _context.SaveChangesAsync();

            List<SkillStats> skillStats = await _hiscoresApi.GetPlayerStatsMain(playerName);
            playerExperience = new PlayerExperience()
            {
                PlayerId = player.Id,
                SkillStats = skillStats.ToArray(),
                Timestamp = timestamp
            };

            await _context.PlayerExperience.AddAsync(playerExperience);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (player.LastUpdated < DateTimeOffset.UtcNow.AddDays(-1))
            {
                List<SkillStats> skillStats = await _hiscoresApi.GetPlayerStatsMain(playerName);

                playerExperience = await _context.PlayerExperience.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                if (playerExperience is null)
                {
                    playerExperience = new PlayerExperience()
                    {
                        PlayerId = player.Id,
                        SkillStats = skillStats.ToArray(),
                        Timestamp = DateTimeOffset.UtcNow
                    };

                    await _context.PlayerExperience.AddAsync(playerExperience);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    playerExperience.SkillStats = skillStats.ToArray();

                    _context.PlayerExperience.Update(playerExperience);
                    await _context.SaveChangesAsync();
                }

                return playerExperience;
            }

            playerExperience =
                await _context.PlayerExperience.FirstOrDefaultAsync(x => x.PlayerId == player.Id);
        }

        return playerExperience;
    }

    public async Task<PlayerActivities> GetPlayerActivities(string playerName)
    {
        PlayerActivities playerActivities;

        Player player = await _context.Player.FirstOrDefaultAsync(x => x.Name.ToLower() == playerName.ToLower());
        if (player is null)
        {
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            player = new Player()
            {
                Name = playerName,
                StartedTracking = timestamp
            };
            await _context.Player.AddAsync(player);
            await _context.SaveChangesAsync();

            List<ActivityStats> activityStats = await _hiscoresApi.GetPlayerActivitiesMain(playerName);
            playerActivities = new PlayerActivities()
            {
                PlayerId = player.Id,
                ActivityStats = activityStats.ToArray(),
                Timestamp = timestamp
            };

            await _context.PlayerActivities.AddAsync(playerActivities);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (player.LastUpdated < DateTimeOffset.UtcNow.AddDays(-1))
            {
                List<ActivityStats> activityStats = await _hiscoresApi.GetPlayerActivitiesMain(playerName);
                playerActivities = await _context.PlayerActivities.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                if (playerActivities is null)
                {
                    playerActivities = new PlayerActivities()
                    {
                        PlayerId = player.Id,
                        ActivityStats = activityStats.ToArray(),
                        Timestamp = DateTimeOffset.UtcNow
                    };

                    await _context.PlayerActivities.AddAsync(playerActivities);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    playerActivities.ActivityStats = activityStats.ToArray();

                    _context.PlayerActivities.Update(playerActivities);
                    await _context.SaveChangesAsync();
                }

                return playerActivities;
            }

            playerActivities =
                    await _context.PlayerActivities.FirstOrDefaultAsync(x => x.PlayerId == player.Id);
        }

        return playerActivities;
    }
}