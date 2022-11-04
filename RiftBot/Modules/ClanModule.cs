namespace RiftBot;

public class ClanModule
{
    private readonly ILogger<ClanModule> _logger;
    private readonly ClanService _clanService;
    private readonly RiftBotContext _context;

    public ClanModule(ILogger<ClanModule> logger, RiftBotContext context, ClanService clanService)
    {
        _logger = logger;
        _clanService = clanService;
        _context = context;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "rankups",
                Description = "Check clan memebers clan experience to see if they qualify for a rank up",
                CommandHandler = (SocketSlashCommand command) => GetRankUps(command)
            },
            new()
            {
                CommandName = "clan-experience",
                Description = "Get a clan member's clan experience",
                CommandHandler = (SocketSlashCommand command) => GetClanMemberExperience(command),
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
                CommandName = "get-rank-preference",
                Description = "Get a clan member's rank type preference",
                CommandHandler = (SocketSlashCommand command) => GetMemberPreference(command),
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
                CommandName = "set-rank-preference",
                Description = "Set a clan member's rank type preference",
                CommandHandler = (SocketSlashCommand command) => SetMemberPreference(command),
                Options = new()
                {
                    new()
                    {
                        Name = "playername",
                        Description = "In game player name",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    },
                    new()
                    {
                        Name = "preference",
                        Description = "Rank type, pvm or skilling",
                        Type = ApplicationCommandOptionType.String,
                        Required = true
                    }
                }
            }
        };
    }

    public async Task GetClanMemberExperience(SocketSlashCommand command)
    {
        try
        {
            string memberName = command.Data.Options.First(x => x.Name == "playername").Value.ToString();
            if (string.IsNullOrEmpty(memberName))
            {
                await command.RespondAsync("Invalid player name").ConfigureAwait(false);
            }

            if (memberName.StartsWith('\"') || memberName.EndsWith('\"'))
                memberName = memberName.Trim('\"');

            string xp = await _clanService.GetClanMemberXp(memberName).ConfigureAwait(false);

            var embed = new EmbedBuilder()
                .WithDescription(xp)
                .Build();

            await command.ModifyOriginalResponseAsync(x => x.Embed = embed).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task GetMemberPreference(SocketSlashCommand command)
    {
        try
        {
            string playerName = command.Data.Options.First(x => x.Name == "playername").Value.ToString();
            if (playerName.StartsWith('\"') || playerName.EndsWith('\"'))
                playerName = playerName.Trim('\"');

            string preference = await _clanService.GetPreference(playerName).ConfigureAwait(false) ? "pvm" : "skilling";
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{playerName}'s preference is set to {preference}").ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            await command.ModifyOriginalResponseAsync(x => x.Content = ex.Message).ConfigureAwait(false);
            _logger.LogError(ex.Message);
        }
    }

    public async Task SetMemberPreference(SocketSlashCommand command)
    {
        try
        {
            string playerName = command.Data.Options.First(x => x.Name == "playername").Value.ToString();
            string preference = command.Data.Options.First(x => x.Name == "preference").Value.ToString();
            if (preference.ToLower() != "pvm" && preference.ToLower() != "skilling")
            {
                await command.ModifyOriginalResponseAsync(x => x.Content = "Invalid preference, please use \"pvm\" or \"skilling\"");
                return;
            }

            BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel").ConfigureAwait(false);
            BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild").ConfigureAwait(false);
            if (command.Channel.Name != restrictedCommandChannelSetting.Value && command.GuildId != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

            bool pvm = preference.ToLower() == "pvm";
            await _clanService.SetPreference(playerName, pvm).ConfigureAwait(false);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{playerName}'s preference has been set to {preference}").ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private class RandomMessage
    {
        public RandomMessage(string message, bool runAnyway, int timeout = 0)
        {
            Message = message;
            RunAnyway = runAnyway;
            Timeout = timeout;
        }

        public string Message { get; set; }
        public bool RunAnyway { get; set; }
        public int Timeout { get; set; }
    }

    public async Task GetRankUps(SocketSlashCommand command)
    {
        bool force = false;
        BotSetting restrictedCommandChannelSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandChannel").ConfigureAwait(false);
        BotSetting restrictedCommandGuildSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RestrictedCommandGuild").ConfigureAwait(false);
        if (command.Channel.Name != restrictedCommandChannelSetting.Value && command.GuildId != ulong.Parse(restrictedCommandGuildSetting.Value)) return;

        if (!force)
        {
            List<RandomMessage> randomMessages = new()
            {
                new($"No {Emote.Parse("<:Thefinger:725207592950956153>")}", false),
                new("Do it yourself!", false),
                new("Fuck off!", false),
                new("Fine... give me a few seconds", true, 3000),
                new("Zzzzzzzzz", false),
                new("Huh? Who? What?...... Oh, you again.... Go away", false),
                new("I need a raise", true, 3000)
            };

            BotSetting rankupsTrollChanceSetting = await _context.BotSettings.FirstOrDefaultAsync(x => x.Name == "RankupsTrollChance").ConfigureAwait(false);

            Random r = new();
            if (r.NextSingle() <= float.Parse(rankupsTrollChanceSetting.Value))
            {
                int index = r.Next(0, randomMessages.Count);
                RandomMessage randomMessage = randomMessages[index];

                await command.ModifyOriginalResponseAsync((message) =>
                {
                    message.Content = randomMessage.Message;
                }).ConfigureAwait(false);

                if (!randomMessage.RunAnyway) return;

                IDisposable typingContext = command.Channel.EnterTypingState();
                await Task.Delay(randomMessage.Timeout).ConfigureAwait(false);
                typingContext.Dispose();
            }
        }

        var embed = new EmbedBuilder();
        List<MemberRankUp> memberRankUps = await _clanService.GetRankUps().ConfigureAwait(false);
        if (memberRankUps.Count > 0)
        {
            memberRankUps = memberRankUps.OrderBy(x => x.MemberName).ToList();
            foreach (MemberRankUp memberRankUp in memberRankUps)
            {
                embed.AddField($"{memberRankUp.MemberName}",
                    $"From {memberRankUp.CurrentRank} to {memberRankUp.NewRank}\nExperience: {memberRankUp.CurrentXp:N0}/{memberRankUp.RequiredXp:N0}");
            }
        }
        else
        {
            embed.WithDescription("Ranks are all up to date!");
        }

        await command.ModifyOriginalResponseAsync((message) =>
        {
            message.Embed = embed.Build();
        }).ConfigureAwait(false);
    }
}