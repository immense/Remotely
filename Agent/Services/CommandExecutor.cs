using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class CommandExecutor
    {
        public CommandExecutor(ConfigService configService)
        {
            ConfigService = configService;
        }

        private ConfigService ConfigService { get; }

        public async Task ExecuteCommand(string mode, string command, string commandID, string senderConnectionID, HubConnection hubConnection)
        {
            try
            {
                switch (mode.ToLower())
                {
                    case "pscore":
                        {
                            var psCoreResult = PSCore.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(psCoreResult);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("PSCore", psCoreResult);
                                await hubConnection.SendAsync("PSCoreResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.SendAsync("PSCoreResult", psCoreResult);
                            }
                            break;
                        }
                    case "winps":
                        if (EnvironmentHelper.IsWindows)
                        {
                            var result = WindowsPS.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("WinPS", result);
                                await hubConnection.SendAsync("WinPSResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.SendAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "cmd":
                        if (EnvironmentHelper.IsWindows)
                        {
                            var result = CMD.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("CMD", result);
                                await hubConnection.SendAsync("CMDResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.SendAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "bash":
                        if (EnvironmentHelper.IsLinux)
                        {
                            var result = Bash.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("Bash", result);
                                await hubConnection.SendAsync("BashResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.SendAsync("CommandResult", result);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", "There was an error executing the command.  It has been logged on the client device.", "Error executing command.", senderConnectionID);
            }
        }

        public async Task ExecuteCommandFromApi(string mode, string requestID, string command, string commandID, string senderUserName, HubConnection hubConnection)
        {
            try
            {
                switch (mode.ToLower())
                {
                    case "pscore":
                        var psCoreResult = PSCore.GetCurrent(senderUserName).WriteInput(command, commandID);
                        await SendResultsViaAjax("PSCore", psCoreResult);
                        break;

                    case "winps":
                        if (EnvironmentHelper.IsWindows)
                        {
                            var result = WindowsPS.GetCurrent(senderUserName).WriteInput(command, commandID);
                            await SendResultsViaAjax("WinPS", result);
                        }
                        break;
                    case "cmd":
                        if (EnvironmentHelper.IsWindows)
                        {
                            var result = CMD.GetCurrent(senderUserName).WriteInput(command, commandID);
                            await SendResultsViaAjax("CMD", result);
                        }
                        break;
                    case "bash":
                        if (EnvironmentHelper.IsLinux)
                        {
                            var result = Bash.GetCurrent(senderUserName).WriteInput(command, commandID);
                            await SendResultsViaAjax("Bash", result);
                        }
                        break;
                    default:
                        break;
                }

                await hubConnection.SendAsync("CommandResultViaApi", commandID, requestID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        private async Task SendResultsViaAjax(string resultType, object result)
        {
            var targetURL = ConfigService.GetConnectionInfo().Host + $"/API/Commands/{resultType}";
            var webRequest = WebRequest.CreateHttp(targetURL);
            webRequest.Method = "POST";

            using (var sw = new StreamWriter(webRequest.GetRequestStream()))
            {
                await sw.WriteAsync(JsonSerializer.Serialize(result));
            }
            (await webRequest.GetResponseAsync())?.Dispose();
        }
    }
}
