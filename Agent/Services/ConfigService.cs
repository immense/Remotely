using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Remotely.Agent.Services
{
    public class ConfigService
    {
        private static readonly object _fileLock = new();
        private ConnectionInfo _connectionInfo;
        private readonly string _debugGuid = "f2b0a595-5ea8-471b-975f-12e70e0f3497";

        private Dictionary<string, string> _commandLineArgs;
        private Dictionary<string, string> CommandLineArgs
        {
            get
            {
                if (_commandLineArgs is null)
                {
                    _commandLineArgs = new Dictionary<string, string>();
                    var args = Environment.GetCommandLineArgs();
                    for (var i = 1; i < args.Length; i += 2)
                    {
                        var key = args?[i];
                        if (key != null)
                        {
                            key = key.Trim().Replace("-", "").ToLower();
                            var value = args?[i + 1];
                            if (value != null)
                            {
                                _commandLineArgs[key] = args[i + 1].Trim();
                            }
                        }

                    }
                }
                return _commandLineArgs;
            }
        }

        public ConnectionInfo GetConnectionInfo()
        {
            // For debugging purposes (i.e. launch of a bunch of instances).
            if (CommandLineArgs.TryGetValue("organization", out var orgID) &&
                CommandLineArgs.TryGetValue("host", out var hostName) &&
                CommandLineArgs.TryGetValue("device", out var deviceID))
            {
                return new ConnectionInfo()
                {
                    DeviceID = deviceID,
                    Host = hostName,
                    OrganizationID = orgID
                };
            }

            if (Environment.UserInteractive && Debugger.IsAttached)
            {
                return new ConnectionInfo()
                {
                    DeviceID = _debugGuid,
                    Host = "https://localhost:5001",
                    OrganizationID = orgID
                };
            }


            if (_connectionInfo == null)
            {
                lock (_fileLock)
                {
                    if (!File.Exists("ConnectionInfo.json"))
                    {
                        Logger.Write(new Exception("No connection info available.  Please create ConnectionInfo.json file with appropriate values."));
                        return null;
                    }
                    _connectionInfo = JsonSerializer.Deserialize<ConnectionInfo>(File.ReadAllText("ConnectionInfo.json"));
                }
            }

            return _connectionInfo;
        }


        public void SaveConnectionInfo(ConnectionInfo connectionInfo)
        {
            lock (_fileLock)
            {
                this._connectionInfo = connectionInfo;
                File.WriteAllText("ConnectionInfo.json", JsonSerializer.Serialize(connectionInfo));
            }
        }
    }
}
