using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Tests.LoadTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(ConnectAgents).Wait();

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }

        private static async Task ConnectAgents()
        {
            try
            {
                if (!CommandLineParser.CommandLineArgs.ContainsKey("serverurl") ||
                       !CommandLineParser.CommandLineArgs.ContainsKey("organizationid") ||
                       !CommandLineParser.CommandLineArgs.ContainsKey("agentcount"))
                {
                    Console.WriteLine("Command line arguments must include all of the following: ");
                    Console.WriteLine();
                    Console.WriteLine("-serverurl [full URL of the Remotely server]");
                    Console.WriteLine();
                    Console.WriteLine("-organizationid [organization ID that the device will belong to]");
                    Console.WriteLine();
                    Console.WriteLine("-agentcount [the number of agent connections to simulate]");
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to exit...");
                    Environment.Exit(0);
                }

                var agentCount = int.Parse(CommandLineParser.CommandLineArgs["agentcount"]);
                var organizationId = CommandLineParser.CommandLineArgs["organizationid"];
                var serverurl = CommandLineParser.CommandLineArgs["serverurl"];
                var connections = new Dictionary<string, HubConnection>();

                for (var i = 0; i < agentCount; i++)
                {
                    try
                    {
                        var deviceId = Guid.NewGuid().ToString();

                        var hubConnection = new HubConnectionBuilder()
                            .WithUrl(serverurl + "/AgentHub")
                            .Build();

                        Console.WriteLine("Connecting device number " + i.ToString());
                        await hubConnection.StartAsync();

                        var device = await DeviceInformation.Create(deviceId, organizationId);
                        device.DeviceName = "TestDevice-" + Guid.NewGuid();

                        var result = await hubConnection.InvokeAsync<bool>("DeviceCameOnline", device);

                        if (!result)
                        {
                            Console.WriteLine($"Device {i} failed to come online.");
                            return;
                        }

                        var heartbeatTimer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
                        heartbeatTimer.Elapsed += async (sender, args) =>
                        {
                            var currentInfo = await DeviceInformation.Create(device.ID, organizationId);
                            currentInfo.DeviceName = device.DeviceName;
                            await hubConnection.SendAsync("DeviceHeartbeat", currentInfo);
                        };
                        heartbeatTimer.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Device {i} failed to connect.");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred.  Check your syntex.  Error: ");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
            }
        }
    }
}