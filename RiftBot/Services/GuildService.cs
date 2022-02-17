namespace RiftBot;

public class GuildService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public GuildService(IConfiguration config)
    {
        _config = config;

        string token = _config.GetSection("Token").Value;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://discord.com/api/v9/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {token}");
    }

    public async Task<List<GuildMember>> GetGuildMembersAsync()
    {
        List<GuildMember> guildMembers = new();

        HttpResponseMessage membersResponse = await _httpClient.GetAsync("guilds/332626371648028672/members?limit=1000");

        if (membersResponse.IsSuccessStatusCode)
        {
            string body = await membersResponse.Content.ReadAsStringAsync();
            guildMembers = JsonSerializer.Deserialize<List<GuildMember>>(body) ?? new();
        }

        if (guildMembers.Count > 0)
        {
            List<GuildRole> guildRoles = await GetGuildRolesAsync();
            MatchUsersToRoles(guildMembers, guildRoles);
        }

        return guildMembers;
    }

    public async Task<List<GuildRole>> GetGuildRolesAsync()
    {
        List<GuildRole> guildRoles = new();

        HttpResponseMessage rolesResponse = await _httpClient.GetAsync("guilds/332626371648028672/roles");

        if (rolesResponse.IsSuccessStatusCode)
        {
            string body = await rolesResponse.Content.ReadAsStringAsync();
            guildRoles = JsonSerializer.Deserialize<List<GuildRole>>(body) ?? new();
        }

        return guildRoles;
    }

    private void MatchUsersToRoles(List<GuildMember> guildMembers, List<GuildRole> guildRoles)
    {
        foreach (GuildMember guildMember in guildMembers)
        {
            for (int i = 0; i < guildMember.Roles.Length; i++)
            {
                guildMember.GuildRoles.Add(guildRoles.FirstOrDefault(x => x.Id == guildMember.Roles[i]));
            }
        }
    }
}