using Moq;
using System.Reflection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.Modules.Billing.Infrastructure.DAL;
using VShop.Modules.Catalog.Infrastructure.DAL;
using VShop.Modules.ProcessManager.Infrastructure.DAL;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;

namespace Database.DatabaseMigrator;

public class Startup
{
    private const string Module = "Sales";
    
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        Assembly migrationAssembly = Module.ToLowerInvariant() switch
        {
            "sales" => typeof(SalesDbContext).Assembly,
            "billing" => typeof(BillingDbContext).Assembly,
            "catalog" => typeof(CatalogDbContext).Assembly,
            "processmanager" => typeof(ProcessManagerDbContext).Assembly,
            _ => throw new Exception("Missing migration assembly.")
        };
        
        services.AddDbContextBuilder(_configuration[$"{Module}:Postgres:ConnectionString"], migrationAssembly);
        services.AddDbContext<SalesDbContext>();
        services.AddDbContext<BillingDbContext>();
        services.AddDbContext<CatalogDbContext>();
        services.AddDbContext<ProcessManagerDbContext>();
        services.AddDbContext<SchedulerDbContext>();
        services.AddDbContext<SubscriptionDbContext>();
        services.AddDbContext<IntegrationDbContext>();
        services.AddScoped(_ => new Mock<IClockService>().Object);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {

    }
}