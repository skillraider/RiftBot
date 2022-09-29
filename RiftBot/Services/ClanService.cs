namespace RiftBot;

public class ClanService
{
    private readonly ILogger<ClanService> _logger;
    private readonly RiftBotContext _context;
    private readonly IClanMembersApi _clanMembersApi;

    public ClanService(RiftBotContext context, IClanMembersApi clanMembersApi)
    {
        _context = context;
        _clanMembersApi = clanMembersApi;
    }

    public async Task<string> GetClanMemberXp(string memberName)
    {
        ClanMember clanMember = await _context.ClanMember.FirstOrDefaultAsync(x => x.Name.ToLower() == memberName.ToLower());

        if (clanMember == null)
        {
            return $"{memberName} is not in the clan";
        }

        List<ClanRankRequirement> clanRankRequirements =
            await _context.ClanRankRequirement
                .OrderBy(x => x.ClanRank)
                .Where(x => x.ClanId == clanMember.ClanId).ToListAsync();

        if (clanRankRequirements.Count <= 0)
        {
            return $"{clanMember.Name}'s Clan Experience: {clanMember.ClanExperience:N0}";
        }

        string xpTillMessage = $"{clanMember.Name}'s Clan Experience: {clanMember.ClanExperience:N0}\n";
        int rankValue = GetClanRankValue(clanMember.ClanRank);
        foreach (ClanRankRequirement clanRankRequirement in clanRankRequirements)
        {
            if (clanMember.ClanExperience <= clanRankRequirement.ClanExperience && rankValue < (int)clanRankRequirement.ClanRank)
            {
                double percent = clanMember.ClanExperience * 1.0 / clanRankRequirement.ClanExperience;
                xpTillMessage += $"{percent:P2} of {clanRankRequirement.ClanExperience:N0} needed for {clanRankRequirement.ClanRank}";
                break;
            }
        }

        return xpTillMessage;
    }

    public async Task<List<MemberRankUp>> GetRankUps()
    {
        try
        {
            List<MemberRankUp> memberRankUps = new List<MemberRankUp>();
            List<ClanMember> members = await _context.ClanMember
                .Where(x => !x.PreferPvm && !x.HasLeftClan).ToListAsync();
            List<ClanRankRequirement> clanRankRequirements = await _context.ClanRankRequirement.OrderBy(x => x.ClanExperience).ToListAsync().ConfigureAwait(false);

            foreach (ClanMember member in members)
            {
                int rankValue = GetClanRankValue(member.ClanRank);
                foreach (ClanRankRequirement requirement in clanRankRequirements)
                {
                    if (member.ClanExperience >= requirement.ClanExperience && rankValue < (int)requirement.ClanRank)
                    {
                        MemberRankUp existingRankUp = memberRankUps.FirstOrDefault(x => x.MemberName == member.Name);
                        if (existingRankUp is not null)
                        {
                            memberRankUps.Remove(existingRankUp);
                        }

                        memberRankUps.Add(new MemberRankUp()
                        {
                            MemberName = member.Name,
                            CurrentRank = member.ClanRank,
                            NewRank = requirement.ClanRank.ToString(),
                            CurrentXp = member.ClanExperience,
                            RequiredXp = requirement.ClanExperience
                        });
                    }
                }
            }

            return memberRankUps;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now:G} - {ex.Message}");
            return new List<MemberRankUp>();
        }
    }

    public async Task<bool> GetPreference(string playerName)
    {
        ClanMember clanMember = await _context.ClanMember.FirstOrDefaultAsync(x => x.Name.ToLower() == playerName.ToLower());
        if (clanMember is null)
        {
            throw new ArgumentException($"{playerName} is not a member of the clan");
        }

        return clanMember.PreferPvm;
    }

    public async Task SetPreference(string playerName, bool preference)
    {
        ClanMember clanMember = await _context.ClanMember.FirstOrDefaultAsync(x => x.Name.ToLower() == playerName.ToLower());
        if (clanMember is null)
        {
            throw new ArgumentException($"{playerName} is not a member of the clan");
        }

        clanMember.PreferPvm = preference;
        _context.ClanMember.Update(clanMember);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClanMembers()
    {
        List<ClanMember> clanMembers = await _clanMembersApi.GetClanMembers("rift");
        await UpdateDatabase("rift", clanMembers);
    }

    private async Task UpdateDatabase(string clanName, List<ClanMember> updatedClanMembers)
    {
        Clan clan = await _context.Clan
            .FirstOrDefaultAsync(x => x.Name == clanName);

        if (clan is null)
        {
            clan = new Clan()
            {
                Name = clanName,
                NumberOfMembers = updatedClanMembers.Count,
                StartedTracking = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow
            };

            await _context.Clan.AddAsync(clan);

            await _context.SaveChangesAsync();
        }
        else
        {
            clan.NumberOfMembers = updatedClanMembers.Count;
            clan.LastUpdated = DateTimeOffset.UtcNow;

            _context.Clan.Update(clan);
            await _context.SaveChangesAsync();
        }

        List<ClanMember> existingClanMembers = await _context.ClanMember
            .Where(x => x.HasLeftClan == false).ToListAsync();

        foreach (ClanMember clanMember in updatedClanMembers)
        {
            ClanMember existingClanMember =
                existingClanMembers.Find(existingMember => existingMember.Name.ToLower() == clanMember.Name.ToLower());
            if (existingClanMember is null)
            {
                clanMember.HasLeftClan = false;
                clanMember.LastUpdated = DateTime.UtcNow;
                clanMember.ClanId = clan.Id;

                await _context.ClanMember.AddAsync(clanMember);
                await _context.SaveChangesAsync();

                await _context.ClanMemberExperience.AddAsync(new ClanMemberExperience()
                {
                    ClanExperience = clanMember.ClanExperience,
                    ClanMemberId = clanMember.Id,
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
            else
            {
                existingClanMember.ClanExperience = clanMember.ClanExperience;
                existingClanMember.ClanRank = clanMember.ClanRank;
                existingClanMember.LastUpdated = DateTime.UtcNow;

                var clanMemberExperience = await _context.ClanMemberExperience
                    .FirstOrDefaultAsync(x => x.ClanMemberId == existingClanMember.Id);
                if (clanMemberExperience is null)
                {
                    await _context.ClanMemberExperience.AddAsync(new ClanMemberExperience()
                    {
                        ClanExperience = clanMember.ClanExperience,
                        ClanMemberId = existingClanMember.Id,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                }
                else
                {
                    clanMemberExperience.ClanExperience = clanMember.ClanExperience;
                    clanMemberExperience.Timestamp = DateTimeOffset.UtcNow;

                    _context.ClanMemberExperience.Update(clanMemberExperience);
                }

                _context.ClanMember.Update(existingClanMember);
                await _context.SaveChangesAsync();
            }
        }

        existingClanMembers = await _context.ClanMember.ToListAsync();
        foreach (ClanMember existingClanMember in existingClanMembers)
        {
            ClanMember newMember = updatedClanMembers.Find(x => x.Name.ToLower() == existingClanMember.Name.ToLower());
            if (newMember is null)
            {
                var clanMemberExperience =
                    await _context.ClanMemberExperience
                        .FirstOrDefaultAsync(x => x.ClanMemberId == existingClanMember.Id);

                _context.ClanMember.Remove(existingClanMember);
                _context.ClanMemberExperience.Remove(clanMemberExperience);
                await _context.SaveChangesAsync();
            }
        }
    }

    private static int GetClanRankValue(string clanRank) => clanRank switch
    {
        "Recruit" => 0,
        "Corporal" => 1,
        "Sergeant" => 2,
        "Lieutenant" => 3,
        "Captain" => 4,
        "General" => 5,
        "Admin" => 6,
        "Organiser" => 7,
        "Coordinator" => 8,
        "Overseer" => 9,
        "Deputy Owner" => 10,
        "Owner" => 11,
        _ => 0,
    };
}