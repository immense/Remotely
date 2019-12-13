using Microsoft.Management.Infrastructure;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotely.Shared.Services
{
    public class DeviceInformation
    {
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

        public static Tuple<double, double> GetLinxMemoryInGB()
        {
            try
            {
                var results = OSUtils.StartProcessWithResults("cat", "/proc/meminfo");
                var resultsArr = results.Split("\n".ToCharArray());
                var freeKB = resultsArr
                            .FirstOrDefault(x => x.Trim().StartsWith("MemFree"))
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

                return new Tuple<double, double>(freeGB, totalGB);
            }
            catch
            {
                return new Tuple<double, double>(0, 0);
            }
        }

        public static Tuple<double, double> GetWinMemoryInGB()
        {
            try
            {
                var session = CimSession.Create(null);
                var cimOS = session.EnumerateInstances("root\\cimv2", "CIM_OperatingSystem");
                var free = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["FreePhysicalMemory"]?.Value ?? 0);
                var freeGB = Math.Round(((double)free / 1024 / 1024), 2);
                var total = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["TotalVisibleMemorySize"]?.Value ?? 0);
                var totalGB = Math.Round(((double)total / 1024 / 1024), 2);

                return new Tuple<double, double>(freeGB, totalGB);
            }
            catch
            {
                return new Tuple<double, double>(0, 0);
            }
        }
    }
}
