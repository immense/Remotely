using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class ScriptRunner
    {
        public ScriptRunner(CommandExecutor commandExecutor, ConfigService configService)
        {
            CommandExecutor = commandExecutor;
            ConfigService = configService;
        }

        private CommandExecutor CommandExecutor { get; }
        private ConfigService ConfigService { get; }

        public async Task RunScript(string mode, string fileID, string commandContextID, string requesterID, HubConnection hubConnection)
        {
            var connectionInfo = ConfigService.GetConnectionInfo();
            var sharedFilePath = Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Remotely",
                    "SharedFiles"
                )).FullName;
            var webClient = new WebClient();

            var url = $"{connectionInfo.Host}/API/FileSharing/{fileID}";
            var wr = WebRequest.CreateHttp(url);
            var response = await wr.GetResponseAsync();
            var cd = response.Headers["Content-Disposition"];
            var filename = cd.Split(";").FirstOrDefault(x => x.Trim().StartsWith("filename")).Split("=")[1];
            using (var rs = response.GetResponseStream())
            {
                using (var sr = new StreamReader(rs))
                {
                    var result = await sr.ReadToEndAsync();
                    await CommandExecutor.ExecuteCommand(mode, result, commandContextID, requesterID, hubConnection);
                }
            }
        }
    }
}
