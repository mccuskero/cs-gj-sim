using Orleans;
using Orleans.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HEUSS.Services.SimulationEngine;

/// <summary>
/// Orleans Silo host for the HEUSS simulation engine.
/// Configures and runs the distributed actor system.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = await StartSiloAsync();

            Console.WriteLine("✓ Orleans Silo started successfully");
            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();

            await host.StopAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to start silo: {ex.Message}");
            return 1;
        }
    }

    private static async Task<IHost> StartSiloAsync()
    {
        var builder = Host.CreateDefaultBuilder()
            .UseOrleans((context, siloBuilder) =>
            {
                siloBuilder
                    // Configure cluster
                    .UseLocalhostClustering()

                    // Configure grain storage
                    .AddMemoryGrainStorage("person-storage")
                    .AddMemoryGrainStorage("house-storage")
                    .AddMemoryGrainStorage("vehicle-storage")
                    .AddMemoryGrainStorage("business-storage")
                    .AddMemoryGrainStorage("datacenter-storage")
                    .AddMemoryGrainStorage("farm-storage")
                    .AddMemoryGrainStorage("region-storage")
                    .AddMemoryGrainStorage("nation-storage")

                    // Configure streaming
                    .AddMemoryStreams("EnergyStreamProvider")
                    .AddMemoryGrainStorage("PubSubStore") // Required for streaming pub/sub

                    // Configure reminders for temporal coordinator
                    .UseInMemoryReminderService()

                    // Configure dashboard (optional, for debugging)
                    .UseDashboard(options =>
                    {
                        options.Port = 8080;
                        options.HostSelf = true;
                    });
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

        var host = builder.Build();
        await host.StartAsync();

        return host;
    }
}
