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

        PlayerExperience playerExperience = await HiscoresService.GetPlayerStats(playerName).ConfigureAwait(false);

        List<SkillStats> skillStats = playerExperience?.SkillStats.ToList();

        if (skillStats is null)
        {
            await ReplyAsync($"{playerName} does not exist on the hiscores").ConfigureAwait(false);
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

        await ReplyAsync(sb.ToString()).ConfigureAwait(false);
    }

    [Command("hsa", RunMode = RunMode.Async)]
    [Summary("!hsa <player name> - Get a player's activities stats")]
    public async Task CheckHiscoresActivities([Remainder] string playerName)
    {
        if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
            playerName = playerName.Trim('\"');

        PlayerActivities playerActivities = await HiscoresService.GetPlayerActivities(playerName).ConfigureAwait(false);

        List<ActivityStats> activityStats = playerActivities?.ActivityStats.ToList();

        if (activityStats is null)
        {
            await ReplyAsync($"{playerName} does not exist on the hiscores").ConfigureAwait(false);
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

        await ReplyAsync(sb1.ToString()).ConfigureAwait(false);
        await ReplyAsync(sb2.ToString()).ConfigureAwait(false);
        await ReplyAsync(sb3.ToString()).ConfigureAwait(false);
    }

    [Command("runescore", RunMode = RunMode.Async)]
    [Summary("!runescore <name> - Get a player's runescore")]
    public async Task GetRunescore([Remainder] string name)
    {
        PlayerActivities playerActivities = await HiscoresService.GetPlayerActivities(name.ToLower()).ConfigureAwait(false);
        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
        builder.WithThumbnailUrl("https://runescape.wiki/images/RuneScore.png?c17e3");
        builder.AddField("Runescore", $"{playerActivities.ActivityStats[24].Total:N0}");
        await ReplyAsync(embed: builder.Build()).ConfigureAwait(false);
    }

    [Command("bosshs", RunMode = RunMode.Async)]
    [Summary("!bosshs <team size> - Get the top 10 kill times per team size")]
    public async Task CheckBoss([Remainder] string teamSize)
    {
        string url = $"https://secure.runescape.com";
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        var response = await client.GetAsync($"/m=group_hiscores/v1/groups?groupSize={teamSize}&size=10&bossId=1&page=0");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            BossHsResponse data = JsonSerializer.Deserialize<BossHsResponse>(content);
            StringBuilder sb = new();
            sb.AppendLine("```");
            foreach (RecordRow row in data.Content)
            {
                for (int i = 0; i < row.Members.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append($"{row.Rank} - {row.Members[i].Name}, ");
                    }
                    else if (i < row.Members.Count - 1)
                    {
                        sb.Append($"{row.Members[i].Name}, ");
                    }
                    else
                    {
                        sb.Append($"{row.Members[i].Name} - ");
                    }
                }

                TimeSpan killTime = new(0, 0, 0, 0, (int)(row.KillTimeInSeconds * 1000));
                sb.AppendLine($"Enrage: {row.Enrage}% - Kill Time: {killTime.Minutes:D2}:{killTime.Seconds:D2}.{killTime.Milliseconds}");
                sb.AppendLine("");
            }
            sb.AppendLine("```");
            await ReplyAsync(sb.ToString());
        }
    }
}