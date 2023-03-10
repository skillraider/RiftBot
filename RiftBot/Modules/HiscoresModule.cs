namespace RiftBot;

public class HiscoresModule
{
    private readonly HiscoreService _hiscoresService;

    public HiscoresModule(HiscoreService hiscoresService)
    {
        _hiscoresService = hiscoresService;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "get-hiscore",
                Description = "Get a player's hiscore",
                CommandHandler = (SocketSlashCommand command) => CheckHiscores(command),
                Options = new()
                {
                    new()
                    {
                        Name = "playername",
                        Description = "In game player name",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    }
                }
            },
            new()
            {
                CommandName = "get-activity-hiscore",
                Description = "Get a player's activity hiscore",
                CommandHandler = (SocketSlashCommand command) => CheckHiscoresActivities(command),
                Options = new()
                {
                    new()
                    {
                        Name = "playername",
                        Description = "In game player name",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    }
                }
            },
            new()
            {
                CommandName = "get-runescore",
                Description = "Get a player's runescore",
                CommandHandler = (SocketSlashCommand command) => GetRunescore(command),
                Options = new()
                {
                    new()
                    {
                        Name = "playername",
                        Description = "In game player name",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    }
                }
            }
        };
    }

    public async Task CheckHiscores(SocketSlashCommand command)
    {
        string? playerName = command.Data.Options.FirstOrDefault(x => x.Name == "playername").Value.ToString();
        if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
            playerName = playerName.Trim('\"');

        PlayerExperience playerExperience = await _hiscoresService.GetPlayerStats(playerName).ConfigureAwait(false);

        List<SkillStats> skillStats = playerExperience?.SkillStats.ToList();

        if (skillStats is null)
        {
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{playerName} does not exist on the hiscores").ConfigureAwait(false);
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

        await command.ModifyOriginalResponseAsync(x => x.Content = sb.ToString()).ConfigureAwait(false);
    }

    public async Task CheckHiscoresActivities(SocketSlashCommand command)
    {
        string playerName = command.Data.Options.First(x => x.Name == "playername").Value.ToString();
        if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
            playerName = playerName.Trim('\"');

        PlayerActivities playerActivities = await _hiscoresService.GetPlayerActivities(playerName).ConfigureAwait(false);

        List<ActivityStats> activityStats = playerActivities?.ActivityStats.ToList();

        if (activityStats is null)
        {
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{playerName} does not exist on the hiscores").ConfigureAwait(false);
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

        await command.ModifyOriginalResponseAsync(x => x.Content = sb1.ToString()).ConfigureAwait(false);
        await command.Channel.SendMessageAsync(sb2.ToString()).ConfigureAwait(false);
        await command.Channel.SendMessageAsync(sb3.ToString()).ConfigureAwait(false);
    }

    public async Task GetRunescore(SocketSlashCommand command)
    {
        string name = command.Data.Options.First(x => x.Name == "playername").Value.ToString();
        PlayerActivities playerActivities = await _hiscoresService.GetPlayerActivities(name.ToLower()).ConfigureAwait(false);
        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor(command.User.Username, command.User.GetAvatarUrl());
        builder.WithThumbnailUrl("https://runescape.wiki/images/RuneScore.png?c17e3");
        builder.AddField("Runescore", $"{playerActivities.ActivityStats[24].Total:N0}");
        await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build()).ConfigureAwait(false);
    }
}