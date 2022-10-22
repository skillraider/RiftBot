using AsyncAwaitBestPractices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RiftBot;

class Program
{
    public static async Task Main()
    {
        IHost host = CreateHostBuilder().Build();

        host.Services.GetRequiredService<Scheduler>().StartAsync().SafeFireAndForget();
        await host.Services.GetRequiredService<RiftBot>().Run();
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
                services.AddScoped<CommandService>();
                services.AddScoped<CommandHandlingService>();

                services.AddTransient<RiftBot>();
                services.AddTransient<Scheduler>();

                services.AddDbContext<RiftBotContext>(options =>
                {
                    options.UseNpgsql(hostingContext.Configuration.GetConnectionString("Default"));
                });
                services.AddScoped<ClanService>();
                services.AddScoped<EventService>();
                services.AddScoped<HiscoreService>();
                services.AddScoped<SelfAssignRoleService>();
                services.AddScoped<GuildService>();
                services.AddScoped<Scheduler>();

                services.AddScoped<IClanMembersApi, ClanMembersApi>();
                services.AddScoped<IHiscoresApi, HiscoresApi>();
            });
}