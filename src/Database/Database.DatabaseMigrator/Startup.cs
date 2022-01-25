using System.Reflection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace Database.DatabaseMigrator;

public class Startup
{
    private const string Module = "Sales";
    
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        Assembly migrationAssembly = Module switch
        {
            "Sales" => typeof(SalesDbContext).Assembly,
            _ => throw new Exception("Missing migration assembly.")
        };

        services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
        (
            _configuration[$"{Module}:Postgres:ConnectionString"],
            migrationAssembly
        ));
        services.AddDbContext<SalesDbContext>();
        services.AddDbContext<SchedulerDbContext>();
        services.AddDbContext<SubscriptionDbContext>();
        services.AddScoped<IClockService, ClockService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {

    }
}