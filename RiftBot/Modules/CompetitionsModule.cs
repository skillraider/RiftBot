using RunePixels.Sdk;

namespace RiftBot;

public class CompetitionsModule
{
    private readonly ILogger<CompetitionsModule> _logger;
    private readonly RunePixelsClient _runePixelsClient;

    public CompetitionsModule(ILogger<CompetitionsModule> logger, RunePixelsClient runePixelsClient)
    {
        _logger = logger;
        _runePixelsClient = runePixelsClient;
    }

    public List<SlashCommand> RegisterSlashCommands()
    {
        return new()
        {
            new()
            {
                CommandName = "get-competitions",
                Description = "Get all Rift Competitions",
                CommandHandler = (SocketSlashCommand command) => SearchCompetitions(command),
                HasModal = true,
                Options =
                {
                    new()
                    {
                        Type = ApplicationCommandOptionType.String,
                        Name = "search",
                        Description = "Phrase or term to search for",
                        Required = false
                    }
                }
            },
            new()
            {
                CommandName = "create-competition",
                Description = "Create a competition",
                CommandHandler = (SocketSlashCommand command) => CreateCompetitionModal(command),
                HasModal = true
            }
        };
    }

    public async Task SearchCompetitions(SocketSlashCommand command)
    {
        try
        {
            SocketSlashCommandDataOption option = command.Data.Options.FirstOrDefault(x => x.Name == "search");
            string search = "";
            if (option is not null)
            {
                search = option.Value.ToString();
            }

            List<Competition>? competitions = await _runePixelsClient.GetAllCompetitions(search: search).ConfigureAwait(false);
            if (competitions is null)
            {
                competitions = new();
            }
            competitions = competitions.Skip(0).Take(25).ToList();
            ComponentBuilder componentBuilder = new ComponentBuilder();
            SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder();
            selectMenuBuilder.CustomId = "competitionSelectMenu";
            foreach (var competition in competitions)
            {
                selectMenuBuilder.AddOption(competition.Name, competition.Id.ToString());
            }
            SelectMenuComponent sc = selectMenuBuilder.Build();
            componentBuilder.WithSelectMenu(selectMenuBuilder);
            await command.RespondAsync(components: componentBuilder.Build(), ephemeral: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task GetCompetitionStats(SocketMessageComponent messageComponent)
    {
        try
        {
            List<string> selectedCompetitionIds = messageComponent.Data.Values.ToList();
            List<Embed> embeds = new();
            foreach (string competitionId in selectedCompetitionIds)
            {
                Console.WriteLine(competitionId);
                EmbedBuilder embedbuilder = new EmbedBuilder();
                CompetitionInfo competitionInfo = await _runePixelsClient.GetCompetition(new()
                {
                    Id = int.Parse(competitionId)
                });
                if (competitionInfo is not null)
                {
                    embedbuilder.Title = competitionInfo.Competition.Name;
                    embedbuilder.AddField("Participants", competitionInfo.Players.Count);
                    embedbuilder.AddField("Total Experience", $"{competitionInfo.Players.Sum(x => x.Experience):N0}");
                    embeds.Add(embedbuilder.Build());
                }
            }

            await messageComponent.RespondAsync(embeds: embeds.ToArray(), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task CreateCompetitionModal(SocketSlashCommand command)
    {
        try
        {
            ModalBuilder mb = new();
            mb.Title = "Create Competition";
            mb.WithCustomId("createCompetition");
            mb.AddTextInput("Name", "name", TextInputStyle.Short, placeholder: "Rift - January 1970 Hiscores", required: true);
            mb.AddTextInput("Start Date", "startDate", TextInputStyle.Short, placeholder: "01/01/1970", required: true);
            mb.AddTextInput("End Date", "endDate", TextInputStyle.Short, placeholder: "01/01/1970", required: true);
            mb.AddTextInput("Participants", "participants", TextInputStyle.Paragraph, placeholder: "*rift\nskillraider", required: true);
            mb.AddTextInput("Secret Pin", "secretPin", TextInputStyle.Short, placeholder: "01/01/1970", minLength: 6, maxLength: 6, required: true);
            await command.RespondWithModalAsync(mb.Build());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task CreateCompetition(SocketModal modal)
    {
        try
        {
            string name = modal.Data.Components.First(x => x.CustomId == "name").Value.ToString();
            string startDateInput = modal.Data.Components.First(x => x.CustomId == "startDate").Value.ToString();
            if (!DateTimeOffset.TryParse(startDateInput, out DateTimeOffset startDate))
            {
                await modal.RespondAsync($"{startDateInput} is not a valid date");
                return;
            }
            string endDateInput = modal.Data.Components.First(x => x.CustomId == "endDate").Value.ToString();
            if (!DateTimeOffset.TryParse(endDateInput, out DateTimeOffset endDate))
            {
                await modal.RespondAsync($"{endDateInput} is not a valid date");
                return;
            }
            string participantsInput = modal.Data.Components.First(x => x.CustomId == "participants").Value.ToString();
            List<string> participants = participantsInput.Split("\n").ToList();
            participants.RemoveAll(x => string.IsNullOrEmpty(x));
            string secretPin = modal.Data.Components.First(x => x.CustomId == "secretPin").Value.ToString();
            CreateCompetition createCompetition = new CreateCompetition()
            {
                Name = name,
                Start = startDate,
                End = endDate,
                Pin = secretPin,
                Participants = participants
            };
            bool success = await _runePixelsClient.CreateCompetition(createCompetition);
            if (success)
            {
                List<Competition> competitionSearchResults = await _runePixelsClient.GetAllCompetitions(name);
                if (competitionSearchResults.Count > 0)
                {
                    string competitionUrl = $"https://runepixels.com/competition/{competitionSearchResults[0].Id}";
                    await modal.RespondAsync($"Competition Created. {competitionUrl}");
                }
                else
                {
                    await modal.RespondAsync($"Competition Created.");
                }
            }
            else
            {
                await modal.RespondAsync("Failed to create competition");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}