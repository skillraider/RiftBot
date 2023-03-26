using Discord;
using RiftBot.Modules;

namespace RiftBot;

public class RiftBot
{
    private readonly ILogger<RiftBot> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ClanModule _clanModule;
    private readonly HiscoresModule _hiscoresModule;
    private readonly ReactionRolesModule _reactionRolesModule;
    private readonly SelfAssignRoleService _selfAssignRoleService;
    private readonly GuildMetadataModule _guildMetadataModule;
    private readonly SettingModule _settingModule;
    private readonly CompetitionsModule _competitionsModule;

    private readonly List<SlashCommand> _commands = new();

    public RiftBot(ILogger<RiftBot> logger, IConfiguration config, DiscordSocketClient discordSocketClient,
        ClanModule clanModule, HiscoresModule hiscoresModule, ReactionRolesModule reactionRolesModule,
        SelfAssignRoleService selfAssignRoleService, GuildMetadataModule guildMetadataModule,
        SettingModule settingModule, CompetitionsModule competitionsModule)
    {
        _logger = logger;
        _config = config;
        _discordSocketClient = discordSocketClient;
        _clanModule = clanModule;
        _hiscoresModule = hiscoresModule;
        _reactionRolesModule = reactionRolesModule;
        _selfAssignRoleService = selfAssignRoleService;
        _guildMetadataModule = guildMetadataModule;
        _settingModule = settingModule;
        _competitionsModule = competitionsModule;

        _logger.LogInformation(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"));

        _commands.AddRange(_clanModule.RegisterSlashCommands());
        _commands.AddRange(_hiscoresModule.RegisterSlashCommands());
        _commands.AddRange(_reactionRolesModule.RegisterSlashCommands());
        _commands.AddRange(_guildMetadataModule.RegisterSlashCommands());
        _commands.AddRange(_settingModule.RegisterSlashCommands());
        _commands.AddRange(_competitionsModule.RegisterSlashCommands());
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            _discordSocketClient.Log += LogAsync;
            _discordSocketClient.Ready += ReadyAsync;
            _discordSocketClient.SlashCommandExecuted += SlashCommandExecutedAsync;
            _discordSocketClient.ReactionAdded += ReactionAdded;
            _discordSocketClient.ReactionRemoved += ReactionRemoved;
            _discordSocketClient.ModalSubmitted += ModalSubmitted;
            _discordSocketClient.SelectMenuExecuted += SelectMenuExecuted;

            await _discordSocketClient.LoginAsync(TokenType.Bot, _config.GetSection("Token").Value);
            await _discordSocketClient.StartAsync();

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{DateTime.Now:G} - RiftBot.Run: {ex}");
        }
    }

    private async Task SelectMenuExecuted(SocketMessageComponent messageComponent)
    {
        if (messageComponent.Data.CustomId == "competitionSelectMenu")
        {
            await _competitionsModule.GetCompetitionStats(messageComponent);
        }
    }

    private async Task ModalSubmitted(SocketModal modal)
    {
        if (modal.Data.CustomId == "createCompetition")
        {
            await _competitionsModule.CreateCompetition(modal);
        }
    }

    private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.UserId == _discordSocketClient.CurrentUser.Id) return;

        await _selfAssignRoleService.AssignRole(channel.Value, reaction);
    }

    private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot) return;

        await _selfAssignRoleService.RemoveRole(channel.Value, reaction);
    }

    private async Task ReadyAsync()
    {
        try
        {
            IReadOnlyCollection<SocketGuild> guilds = _discordSocketClient.Guilds;
            foreach (SocketGuild guild in guilds)
            {
                foreach (SlashCommand command in _commands)
                {
                    SlashCommandBuilder guildCommand = new SlashCommandBuilder();
                    guildCommand.WithName(command.CommandName);
                    guildCommand.WithDescription(command.Description);
                    if (command.Options.Count > 0)
                    {
                        foreach (SlashCommandOption option in command.Options)
                        {
                            SlashCommandOptionBuilder scob = new();
                            scob.IsRequired = option.Required;
                            scob.WithName(option.Name);
                            scob.WithDescription(option.Description);
                            scob.WithType(option.Type);
                            guildCommand.AddOption(scob);
                        }
                    }

                    await guild.CreateApplicationCommandAsync(guildCommand.Build());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        SlashCommand slashCommand = _commands.FirstOrDefault(x => x.CommandName == command.CommandName);
        if (!slashCommand.HasModal)
        {
            await command.DeferAsync();
        }

        if (slashCommand is not null)
        {
            await slashCommand.CommandHandler(command);
        }
    }

    private Task LogAsync(LogMessage log)
    {
        if (string.IsNullOrEmpty(log.Message)) return Task.CompletedTask;

        _logger.LogInformation($"{DateTime.Now:G} - {log.Message}");

        return Task.CompletedTask;
    }
}