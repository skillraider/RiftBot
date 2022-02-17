namespace RiftBot.Database;

public class RiftBotContext : DbContext
{
    public RiftBotContext(DbContextOptions<RiftBotContext> options) : base(options) { }

    public DbSet<BotSetting> BotSettings { get; set; }

    public DbSet<Clan> Clan { get; set; }

    public DbSet<ClanMember> ClanMember { get; set; }

    public DbSet<ClanMemberExperience> ClanMemberExperience { get; set; }

    public DbSet<ClanRankRequirement> ClanRankRequirement { get; set; }

    public DbSet<Event> Event { get; set; }

    public DbSet<EventLog> EventLog { get; set; }

    public DbSet<Player> Player { get; set; }

    public DbSet<PlayerActivities> PlayerActivities { get; set; }

    public DbSet<PlayerExperience> PlayerExperience { get; set; }

    public DbSet<SelfAssignableRole> SelfAssignableRole { get; set; }
}