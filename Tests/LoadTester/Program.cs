using Castle.Core.Logging;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Moq;
using Remotely.Agent.Services;
using Remotely.Agent.Services.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Tests.LoadTester;

internal class Program
{
    private static readonly double _heartbeatMs = TimeSpan.FromMinutes(1).TotalMilliseconds;
    private static int _agentCount;
    private static string? _organizationId;
    private static string? _serverurl;
    private static Mock<ICpuUtilizationSampler>? _cpuSampler;
    private static Mock<ILogger<DeviceInfoGeneratorWin>>? _logger;
    private static DeviceInfoGeneratorWin? _deviceInfo;
    private static Stopwatch? _stopwatch;
    private static int _connectedCount;

    private static void Main(string[] args)
    {
        _cpuSampler = new Mock<ICpuUtilizationSampler>();
        _cpuSampler.Setup(x => x.CurrentUtilization).Returns(0);
        _logger = new Mock<ILogger<DeviceInfoGeneratorWin>>();

        _deviceInfo = new DeviceInfoGeneratorWin(_cpuSampler.Object, _logger.Object);

        _stopwatch = Stopwatch.StartNew();
        ConnectAgents();

        Console.Write("Press Enter to exit...");
        Console.ReadLine();
    }

    private static void ConnectAgents()
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

            _agentCount = int.Parse(CommandLineParser.CommandLineArgs["agentcount"]);
            _organizationId = CommandLineParser.CommandLineArgs["organizationid"];
            _serverurl = CommandLineParser.CommandLineArgs["serverurl"];

            for (var i = 0; i < _agentCount; i++)
            {
                _ = StartAgent(i);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred.  Check your syntex.  Error: ");
            Console.WriteLine();
            Console.WriteLine(ex.Message);
        }
    }

    private static async Task StartAgent(int i)
    {
        while (true)
        {
            try
            {
                var waitSeconds = Math.Max(3, Random.Shared.NextDouble() * 10);
                await Task.Delay(TimeSpan.FromSeconds(waitSeconds));

                var deviceId = Guid.NewGuid().ToString();

                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(_serverurl + "/hubs/service")
                    .WithAutomaticReconnect(new RetryPolicy())
                    .Build();

                Console.WriteLine($"Connecting device number {i}");
                await hubConnection.StartAsync();

                var device = await _deviceInfo!.CreateDevice(deviceId, _organizationId!);
                device.DeviceName = "TestDevice-" + Guid.NewGuid();

                var result = await hubConnection.InvokeAsync<bool>("DeviceCameOnline", device);

                if (!result)
                {
                    Console.WriteLine($"Device {i} failed to come online.");
                    return;
                }


                _ = Task.Run(async () =>
                {
                    await Task.Delay(new Random().Next(1, (int)_heartbeatMs));
                    var heartbeatTimer = new System.Timers.Timer(_heartbeatMs);
                    heartbeatTimer.Elapsed += async (sender, args) =>
                    {
                        try
                        {
                            var currentInfo = await _deviceInfo.CreateDevice(device.ID, _organizationId!);
                            currentInfo.DeviceName = device.DeviceName;
                            await hubConnection.SendAsync("DeviceHeartbeat", currentInfo);
                        }
                        catch { }
                    };
                    heartbeatTimer.Start();
                });

                Console.WriteLine($"Connected device number {i}");

                Interlocked.Increment(ref _connectedCount);
                if (_connectedCount == _agentCount)
                {
                    Console.WriteLine($"Finished connecting all devices.  Elapsed: {_stopwatch!.Elapsed}");
                }

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Device {i} failed to connect.");
                Console.WriteLine(ex.Message);
            }
        }
    }

    private class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(3);
        }
    }
}