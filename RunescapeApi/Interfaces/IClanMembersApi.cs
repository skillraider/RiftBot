namespace RunescapeApi;

public interface IClanMembersApi
{
    Task<List<ClanMember>> GetClanMembers(string clanName);
}