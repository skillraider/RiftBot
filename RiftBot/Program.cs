using AsyncAwaitBestPractices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RiftBot.Modules;

namespace RiftBot;

class Program
{
    public static async Task Main()
    {
        IHost host = CreateHostBuilder().Build();

        host.Services.GetRequiredService<Scheduler>().StartAsync().SafeFireAndForget();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await host.Services.GetRequiredService<RiftBot>().RunAsync(cancellationTokenSource.Token);
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddUserSecrets<Program>(true);
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.AddSingleton(new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.All
                });
                services.AddScoped<DiscordSocketClient>();

                services.AddTransient<RiftBot>();
                services.AddTransient<Scheduler>();

                services.AddDbContext<RiftBotContext>(options =>
                {
                    options.UseNpgsql(hostingContext.Configuration.GetConnectionString("Default"));
                });

                services.AddScoped<ClanModule>();
                services.AddScoped<ClanService>();

                services.AddScoped<EventService>();

                services.AddScoped<HiscoresModule>();
                services.AddScoped<HiscoreService>();

                services.AddScoped<ReactionRolesModule>();
                services.AddScoped<SelfAssignRoleService>();

                services.AddScoped<GuildMetadataModule>();
                services.AddScoped<GuildService>();

                services.AddScoped<SettingModule>();

                services.AddScoped<IClanMembersApi, ClanMembersApi>();
                services.AddScoped<IHiscoresApi, HiscoresApi>();
            });
}