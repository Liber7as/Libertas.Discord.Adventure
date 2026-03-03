using Libertas.Discord.Adventure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.Sources.Clear();
        cfg.AddJsonFile("appsettings.json", false, true);
        cfg.AddUserSecrets(typeof(Program).Assembly, true, true);
    })
    .ConfigureServices((ctx, services) => { ctx.AddAdventure(services); })
    .ConfigureLogging((ctx, logging) =>
    {
        logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();