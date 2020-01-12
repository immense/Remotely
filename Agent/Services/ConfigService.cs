using Remotely.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Remotely.Agent.Services
{
    public static class ConfigService
    {
        private static ConnectionInfo connectionInfo;
        public static ConnectionInfo GetConnectionInfo()
        {
            if (connectionInfo == null)
            {
                if (!File.Exists("ConnectionInfo.json"))
                {
                    Logger.Write(new Exception("No connection info available.  Please create ConnectionInfo.json file with appropriate values."));
                    return null;
                }
                connectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(File.ReadAllText("ConnectionInfo.json"));
            }

            return connectionInfo;
        }


        public static void SaveConnectionInfo(ConnectionInfo connectionInfo)
        {
            File.WriteAllText("ConnectionInfo.json", JsonConvert.SerializeObject(connectionInfo));
        }
    }
}
