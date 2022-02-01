using Xunit;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace VShop.SharedKernel.Tests.IntegrationTests;

public abstract class DatabaseLifetime : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        // Postgres database
        await ClearRelationalDatabaseAsync();

        // EventStore database
        await RestartEventStoreDatabaseAsync();
    }

    protected abstract Task ClearRelationalDatabaseAsync();

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
                .ListContainersAsync(new ContainersListParameters {All = true});
            ContainerListResponse testContainer = eventStoreContainers
                .SingleOrDefault(c => c.Names.Any(n => n.Contains("eventstore.db.tests")));

            await dockerClient.Containers.RestartContainerAsync(testContainer?.ID, new ContainerRestartParameters());

            // NOTE: slower than Thread.Sleep.
            bool isContainerReady = false;
            while (!isContainerReady)
            {
                ContainerInspectResponse containerStatus = await dockerClient.Containers
                    .InspectContainerAsync(testContainer?.ID, CancellationToken.None);

                isContainerReady = containerStatus.State.Health.Status == "healthy";
            }

            //Thread.Sleep(3000);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);

            throw;
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

