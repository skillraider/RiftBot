namespace RiftBot;

public class HiscoresModule : ModuleBase<SocketCommandContext>
{
    public HiscoreService HiscoresService { get; set; }

    [Command("hs", RunMode = RunMode.Async)]
    [Summary("!hs <player name> - Get a player's stats")]
    public async Task CheckHiscores([Remainder] string playerName)
    {
        if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
            playerName = playerName.Trim('\"');

        PlayerExperience playerExperience = await HiscoresService.GetPlayerStats(playerName);

        List<SkillStats> skillStats = playerExperience?.SkillStats.ToList();

        if (skillStats is null)
        {
            await ReplyAsync($"{playerName} does not exist on the hiscores");
            return;
        }

        playerName = playerName.PadRight(12).ToUpper();
        StringBuilder sb = new StringBuilder();
        sb.Append("```\n");
        sb.Append(".-----------------------------------------------.\n");
        sb.Append($"|           Hiscores for {playerName}           |\n");
        sb.Append("|-----------------------------------------------|\n");
        sb.Append("|    Skill    | Level |  Experience  |   Rank   |\n");
        sb.Append("|-------------|-------|--------------|----------|\n");

        foreach (SkillStats skillStat in skillStats)
        {
            string skill = skillStat.Name;
            skill = skill.PadRight(13);
            string level = $"{skillStat.Level:N0}";
            level = level.PadLeft(7);
            string experience = $"{skillStat.Experience:N0}";
            experience = experience.PadLeft(14);
            string rank = $"{skillStat.Rank:N0}";
            rank = rank.PadLeft(10);
            sb.Append($"|{skill}|{level}|{experience}|{rank}|\n");
        }

        sb.Append("'-----------------------------------------------'\n```");

        await ReplyAsync(sb.ToString());
    }

    [Command("hsa", RunMode = RunMode.Async)]
    [Summary("!hsa <player name> - Get a player's activities stats")]
    public async Task CheckHiscoresActivities([Remainder] string playerName)
    {
        if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
            playerName = playerName.Trim('\"');

        PlayerActivities playerActivities = await HiscoresService.GetPlayerActivities(playerName);

        List<ActivityStats> activityStats = playerActivities?.ActivityStats.ToList();

        if (activityStats is null)
        {
            await ReplyAsync($"{playerName} does not exist on the hiscores");
            return;
        }

        playerName = playerName.PadRight(12).ToUpper();
        StringBuilder sb1 = new StringBuilder();
        sb1.Append("```\n");
        sb1.Append(".-------------------------------------------------------------.\n");
        sb1.Append($"|              Activity Hiscores for {playerName}             |\n");
        sb1.Append("|-------------------------------------------------------------|\n");
        sb1.Append("|              Activity              |   Rank   |    Total    |\n");
        sb1.Append("'-------------------------------------------------------------'\n");
        sb1.Append("```");

        StringBuilder sb2 = new StringBuilder();
        sb2.Append("```\n");
        sb2.Append(".-------------------------------------------------------------.\n");
        for (int i = 0; i < activityStats.Count / 2; i++)
        {
            string activity = activityStats[i].Name;
            activity = activity.PadRight(36);
            string rank = $"{activityStats[i].Rank:N0}";
            rank = rank.PadLeft(10);
            string total = $"{activityStats[i].Total:N0}";
            total = total.PadLeft(13);
            sb2.Append($"|{activity}|{rank}|{total}|\n");
        }
        sb2.Append("'-------------------------------------------------------------'\n```");

        StringBuilder sb3 = new StringBuilder();
        sb3.Append("```\n");
        sb3.Append(".-------------------------------------------------------------.\n");
        for (int i = activityStats.Count / 2; i < activityStats.Count; i++)
        {
            string activity = activityStats[i].Name;
            activity = activity.PadRight(36);
            string rank = $"{activityStats[i].Rank:N0}";
            rank = rank.PadLeft(10);
            string total = $"{activityStats[i].Total:N0}";
            total = total.PadLeft(13);
            sb3.Append($"|{activity}|{rank}|{total}|\n");
        }
        sb3.Append("'-------------------------------------------------------------'\n```");

        await ReplyAsync(sb1.ToString());
        await ReplyAsync(sb2.ToString());
        await ReplyAsync(sb3.ToString());
    }
}