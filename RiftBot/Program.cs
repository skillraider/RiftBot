using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RiftBot;

class Program
{
    public static async Task Main()
    {
        IHost host = CreateHostBuilder().Build();

        var ts = new ThreadStart(async () =>
        {
            await host.Services.GetRequiredService<Scheduler>().Start();
        });
        new Thread(ts).Start();
            
        await host.Services.GetRequiredService<RiftBot>().Run();
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", false);
                config.AddUserSecrets<Program>(true);
            })
            .ConfigureServices((hostingContext, services) =>
            {
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

                services.AddScoped<IClanMembersApi, ClanMembersApi>();
                services.AddScoped<IHiscoresApi, HiscoresApi>();
            });
}