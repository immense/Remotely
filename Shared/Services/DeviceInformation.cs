using Microsoft.Management.Infrastructure;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public class DeviceInformation
    {
        public static async Task<double> GetCpuUtilization()
        {
            double totalUtilization = 0;
            var utilizations = new Dictionary<int, Tuple<DateTime, TimeSpan>>();
            var processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    var startCpuUsage = proc.TotalProcessorTime;
                    utilizations.Add(proc.Id, new Tuple<DateTime, TimeSpan>(startTime, startCpuUsage));
                }
                catch
                {
                    continue;
                }
            }

            await Task.Delay(500);

            foreach (var kvp in utilizations)
            {
                var endTime = DateTime.UtcNow;
                try
                {
                    var proc = Process.GetProcessById(kvp.Key);
                    var startTime = kvp.Value.Item1;
                    var startCpuUsage = kvp.Value.Item2;
                    var endCpuUsage = proc.TotalProcessorTime;
                    var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                    var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                    var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
                    totalUtilization += cpuUsageTotal;
                }
                catch
                {
                    continue;
                }
            }

            return totalUtilization;
        }

        public static string GetCurrentUser()
        {
            try
            {
                if (OSUtils.IsWindows)
                {
                    var session = CimSession.Create(null);
                    var computerSystem = session.EnumerateInstances("root\\cimv2", "CIM_ComputerSystem");
                    var username = computerSystem.FirstOrDefault().CimInstanceProperties["UserName"].Value ?? "";
                    return username as string;
                }
                else if (OSUtils.IsLinux)
                {
                    var users = OSUtils.StartProcessWithResults("users", "");
                    var username = users?.Split()?.FirstOrDefault()?.Trim();
                    return $"{Environment.UserDomainName}\\{username}";
                }
                throw new Exception("Unsupported operating system.");
            }
            catch
            {
                return "Error Retrieving";
            }
        }

        public static (double, double) GetMemoryInGB()
        {
            if (OSUtils.IsWindows)
            {
                return GetWinMemoryInGB();
            }
            else if (OSUtils.IsLinux)
            {
                return GetLinxMemoryInGB();
            }
            else
            {
                return (0, 0);
            }
        }

        private static (double, double) GetLinxMemoryInGB()
        {
            try
            {
                var results = OSUtils.StartProcessWithResults("cat", "/proc/meminfo");
                var resultsArr = results.Split("\n".ToCharArray());
                var freeKB = resultsArr
                            .FirstOrDefault(x => x.Trim().StartsWith("MemAvailable"))
                            .Trim()
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 9168236 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 9168236

                var totalKB = resultsArr
                            .FirstOrDefault(x => x.Trim().StartsWith("MemTotal"))
                            .Trim()
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 16637468 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 16637468

                var freeGB = Math.Round((double.Parse(freeKB) / 1024 / 1024), 2);
                var totalGB = Math.Round((double.Parse(totalKB) / 1024 / 1024), 2);

                return (totalGB - freeGB, totalGB);
            }
            catch
            {
                return (0, 0);
            }
        }

        private static (double, double) GetWinMemoryInGB()
        {
            try
            {
                var session = CimSession.Create(null);
                var cimOS = session.EnumerateInstances("root\\cimv2", "CIM_OperatingSystem");
                var free = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["FreePhysicalMemory"]?.Value ?? 0);
                var freeGB = Math.Round(((double)free / 1024 / 1024), 2);
                var total = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["TotalVisibleMemorySize"]?.Value ?? 0);
                var totalGB = Math.Round(((double)total / 1024 / 1024), 2);

                return (totalGB - freeGB, totalGB);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
