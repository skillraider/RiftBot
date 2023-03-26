using System.Text.Json;

namespace RunePixels.Sdk;

public class RunePixelsClient
{
    private readonly HttpClient _httpClient;

    public RunePixelsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CompetitionInfo?> GetCompetition(CompetitionSearch competitionSearch)
    {
        string contentString = JsonSerializer.Serialize(competitionSearch);
        StringContent requestContent = new(contentString);
        HttpResponseMessage response = await _httpClient.PostAsync("competition", requestContent);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            CompetitionInfo? competitionInfo = JsonSerializer.Deserialize<CompetitionInfo>(content);
            if (competitionInfo is not null)
            {
                return competitionInfo;
            }
        }

        return null;
    }

    public async Task<List<Competition>?> GetAllCompetitions(string search = "", CompetitionState state = CompetitionState.All, int skip = 0)
    {
        string searchQuery;
        if (string.IsNullOrEmpty(search))
        {
            searchQuery = $"competitions/all?clanID=0&state={(int)state}&skip=0";
        }
        else
        {
            searchQuery = $"competitions/all/search?clanID=0&state={(int)state}&search={search}";
        }
        
        HttpResponseMessage response = await _httpClient.GetAsync(searchQuery);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            List<Competition>? competitions = JsonSerializer.Deserialize<List<Competition>>(content);
            if (competitions is not null)
            {
                return competitions;
            }
        }

        return null;
    }

    public async Task<bool> CreateCompetition(CreateCompetition createCompetition)
    {
        string contentString = JsonSerializer.Serialize(createCompetition);
        StringContent requestContent = new(contentString);
        HttpResponseMessage response = await _httpClient.PostAsync("competitions", requestContent);
        return response.IsSuccessStatusCode;
    }
}