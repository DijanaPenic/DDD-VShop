﻿using Npgsql;
using System.Reflection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure.Database;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbContextBuilder : IDbContextBuilder
    {
        private DbConnection _connection;
        private readonly Assembly _migrationAssembly;
        private readonly string _connectionString;

        public PostgresDbContextBuilder(string connectionString, Assembly migrationAssembly)
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
                ob => ob.MigrationsAssembly(migrationAssemblyName)
            ).UseSnakeCaseNamingConvention();
        }
    }
}