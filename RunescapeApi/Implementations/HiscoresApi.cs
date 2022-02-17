namespace RunescapeApi;

public class HiscoresApi : IHiscoresApi
{
    private readonly IConfiguration _config;

    public HiscoresApi(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<SkillStats>> GetPlayerStatsMain(string playerName) =>
        await GetPlayerStats(_config.GetApiEndpoints("MainHiscores"), playerName);

    public async Task<List<ActivityStats>> GetPlayerActivitiesMain(string playerName) =>
        await GetPlayerActivities(_config.GetApiEndpoints("MainHiscores"), playerName);

    public async Task<List<SkillStats>> GetPlayerStatsIronman(string playerName) =>
        await GetPlayerStats(_config.GetApiEndpoints("IronmanHiscores"), playerName);

    public async Task<List<SkillStats>> GetPlayerStatsHardcore(string playerName) => 
        await GetPlayerStats(_config.GetApiEndpoints("HardcoreHiscores"), playerName);

    private async Task<List<SkillStats>> GetPlayerStats(string url, string playerName)
    {
        url = string.Format(url, playerName);
        string response = await GetAsync(url);
        return ParseStats(response);
    }

    private async Task<List<ActivityStats>> GetPlayerActivities(string url, string playerName)
    {
        url = string.Format(url, playerName);
        string response = await GetAsync(url);
        return ParseActivities(response);
    }

    private async Task<string> GetAsync(string url)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw new HttpListenerException((int) response.StatusCode);
    }

    private List<SkillStats> ParseStats(string data)
    {
        string[] skillRows = data.Split('\n');
        var skills = Enum.GetNames(typeof(Skills));
        List<SkillStats> skillStats = new List<SkillStats>();
        for (int i = 0; i < skills.Length; i++)
        {
            string[] cells = skillRows[i].Split(',');
            skillStats.Add(new SkillStats
            {
                Name = skills[i],
                Rank = int.Parse(cells[0]),
                Level = int.Parse(cells[1]),
                Experience = long.Parse(cells[2])
            });
        }

        return skillStats;
    }

    private List<ActivityStats> ParseActivities(string data)
    {
        try
        {
            string[] skillRows = data.Split('\n');
            var skills = Enum.GetNames(typeof(Skills));
            var activities = Enum.GetNames(typeof(Activities));
            List<ActivityStats> activityStats = new List<ActivityStats>();
            for (int i = skills.Length; i < activities.Length + skills.Length; i++)
            {
                string[] cells = skillRows[i].Split(',');
                activityStats.Add(new ActivityStats()
                {
                    Name = activities[i - skills.Length],
                    Rank = int.Parse(cells[0]),
                    Total = long.Parse(cells[1])
                });
            }

            return activityStats;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:G} - {ex.Message}");
            return new List<ActivityStats>();
        }
    }
}