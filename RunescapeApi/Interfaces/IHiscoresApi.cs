namespace RunescapeApi;

public interface IHiscoresApi
{
    Task<List<SkillStats>> GetPlayerStatsMain(string playerName);

    Task<List<SkillStats>> GetPlayerStatsIronman(string playerName);

    Task<List<SkillStats>> GetPlayerStatsHardcore(string playerName);

    Task<List<ActivityStats>> GetPlayerActivitiesMain(string playerName);
}