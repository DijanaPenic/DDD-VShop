using Npgsql;
using System.Reflection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using VShop.SharedKernel.Infrastructure.Database;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbContextBuilder : IDbContextBuilder
    {
        private DbConnection _connection;
        private readonly string _connectionString;

        public PostgresDbContextBuilder(IConfiguration configuration)
            => _connectionString = configuration.GetConnectionString("PostgresDb");
        
        public void ConfigureContext(DbContextOptionsBuilder optionsBuilder)
        {
            _connection ??= new NpgsqlConnection(_connectionString);
            Assembly migrationAssembly = Assembly.GetExecutingAssembly();

            optionsBuilder.UseNpgsql
            (
                _connection,
                ob => ob.MigrationsAssembly(migrationAssembly.GetName().Name)
            ).UseSnakeCaseNamingConvention();
        }
    }
}