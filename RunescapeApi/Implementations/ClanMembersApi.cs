namespace RunescapeApi;

public class ClanMembersApi : IClanMembersApi
{
    private readonly IConfiguration _config;

    public ClanMembersApi(IConfiguration config)
    {
        _config = config;
    }

    public Task<List<ClanMember>> GetClanMembers(string clanName)
    {
        return GetClanMemberData(clanName);
    }

    private async Task<List<ClanMember>> GetClanMemberData(string clanName)
    {
        string body = await GetClanData(clanName);
        List<ClanMember> members = ExtractMemberData(body);
        return members;
    }

    private async Task<string> GetClanData(string clanName)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(string.Format(_config.GetApiEndpoints("ClanMembers"), clanName));
        return await response.Content.ReadAsStringAsync();
    }

    private List<ClanMember> ExtractMemberData(string body)
    {
        List<ClanMember> members = new List<ClanMember>();

        string[] lines = body.Split('\n');
        lines = lines[1..];

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;

            ClanMember member = new ClanMember();
            string[] data = line.Split(',');

            member.Name = data[0].Replace('\uFFFD', ' ');
            member.ClanRank = data[1];
            member.ClanExperience = long.Parse(data[2]);

            members.Add(member);
        }

        return members;
    }
}