using Immense.RemoteControl.Shared;
using Microsoft.Extensions.Logging;
using Remotely.Shared;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Remotely.Agent.Services;

public interface IConfigService
{
    ConnectionInfo GetConnectionInfo();
    void SaveConnectionInfo(ConnectionInfo connectionInfo);
}

public class ConfigService : IConfigService
{
    private static readonly object _fileLock = new();
    private readonly string _debugGuid = "f2b0a595-5ea8-471b-975f-12e70e0f3497";
    private readonly ILogger<ConfigService> _logger;
    private ConnectionInfo? _connectionInfo;

    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;
    }

    private Dictionary<string, string>? _commandLineArgs;

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
                    var key = args[i];
                    if (key != null)
                    {
                        key = key.Trim().Replace("-", "").ToLower();
                        var value = args[i + 1];
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
        try
        { // For debugging purposes (i.e. launch of a bunch of instances).
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
                    Host = "http://localhost:5000",
                    OrganizationID = AppConstants.DebugOrgId
                };
            }


            if (_connectionInfo == null)
            {
                lock (_fileLock)
                {
                    if (!File.Exists("ConnectionInfo.json"))
                    {
                        _logger.LogError("No connection info available.  Please create ConnectionInfo.json file with appropriate values.");
                        throw new InvalidOperationException("Config file does not exist.");
                    }
                    _connectionInfo = JsonSerializer.Deserialize<ConnectionInfo>(File.ReadAllText("ConnectionInfo.json"));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve connection info.");
        }

        if (_connectionInfo is null)
        {
            throw new InvalidOperationException("Unable to load config data.");
        }

        return _connectionInfo;
    }


    public void SaveConnectionInfo(ConnectionInfo connectionInfo)
    {
        lock (_fileLock)
        {
            _connectionInfo = connectionInfo;
            File.WriteAllText("ConnectionInfo.json", JsonSerializer.Serialize(connectionInfo));
        }
    }
}
