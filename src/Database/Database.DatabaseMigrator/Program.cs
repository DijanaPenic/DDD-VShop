using VShop.SharedKernel.Infrastructure.Modules;

namespace Database.DatabaseMigrator;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureModules()
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}