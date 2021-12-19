using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Dapper;
using Npgsql;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public class ResetDatabaseLifetime : IAsyncLifetime
    {
        public Task InitializeAsync() => StartDatabaseResetAsync();

        public static async Task StartDatabaseResetAsync()
        {   
            // Postgres database
            await ClearRelationalDatabaseAsync();
            
            // EventStore database
            await RestartEventStoreDatabaseAsync();
        }

        private static async Task ClearRelationalDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(IntegrationTestsFixture.RelationalDbConnectionString);
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""shopping_cart"".""shopping_cart_info_product_item""; " +
                               @"DELETE FROM ""shopping_cart"".""shopping_cart_info""; " +
                               @"DELETE FROM ""subscription"".""checkpoint""; " +
                               @"DELETE FROM ""scheduler"".""message_log""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_cron_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_simple_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_job_details""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_simprop_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_blob_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_calendars""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_paused_trigger_grps""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_fired_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_scheduler_state""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_locks""; ";

            await connection.ExecuteScalarAsync(sql);
            await connection.CloseAsync();
        }
        
        // Source: https://github.com/EventStore/EventStore/issues/1328
        // Restart will also clear the EventStore database (in-mem mode is used).
        private static async Task RestartEventStoreDatabaseAsync()
        {
            try
            {
                Uri address = Environment.OSVersion.Platform == PlatformID.Unix 
                 ? new Uri("unix:///var/run/docker.sock")
                 : new Uri("npipe://./pipe/docker_engine");
             
                DockerClientConfiguration dockerConfig = new(address);
                DockerClient dockerClient = dockerConfig.CreateClient();
                
                IList<ContainerListResponse> eventStoreContainers = await dockerClient.Containers
                    .ListContainersAsync(new ContainersListParameters { All = true });
                ContainerListResponse testContainer = eventStoreContainers
                    .SingleOrDefault(c => c.Names.Any(n => n.Contains("eventstore.db.tests")));
                
                await dockerClient.Containers.RestartContainerAsync(testContainer?.ID, new ContainerRestartParameters());

                // NOTE: 30% slower than Thread.Sleep.
                // bool isContainerReady = false;
                // while (!isContainerReady)
                // {
                //     ContainerInspectResponse containerStatus = await dockerClient.Containers
                //         .InspectContainerAsync(testContainer?.ID, CancellationToken.None);
                //
                //     isContainerReady = containerStatus.State.Health.Status == "healthy";
                // }

                Thread.Sleep(1500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                
                throw;
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}