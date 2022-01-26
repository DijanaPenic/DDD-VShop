using Npgsql;
using System.Reflection;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.PostgresDb
{
    internal class DbContextBuilder : IDbContextBuilder
    {
        private DbConnection _connection;
        private readonly Assembly _migrationAssembly;
        private readonly string _connectionString;

        public DbContextBuilder(string connectionString, Assembly migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }
        
        public void ConfigureContext(DbContextOptionsBuilder optionsBuilder)
        {
            _connection ??= new NpgsqlConnection(_connectionString);
            string migrationAssemblyName = _migrationAssembly.GetName().Name;

            optionsBuilder.UseNpgsql
            (
                _connection,
                ob => ob
                    .MigrationsAssembly(migrationAssemblyName)
                    .UseNodaTime()
            ).UseSnakeCaseNamingConvention();
        }
    }
}