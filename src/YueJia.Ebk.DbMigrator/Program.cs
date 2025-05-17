using Microsoft.Extensions.Hosting;

namespace YueJia.Ebk.DbMigrator;
internal class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => services.AddInDbMigratorServices(hostContext.Configuration))
        .RunConsoleAsync();
    }
}
