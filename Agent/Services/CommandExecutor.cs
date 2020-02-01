using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
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
                                await hubConnection.InvokeAsync("PSCoreResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.InvokeAsync("PSCoreResult", psCoreResult);
                            }
                            break;
                        }

                    case "winps":
                        if (OSUtils.IsWindows)
                        {
                            var result = WindowsPS.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("WinPS", result);
                                await hubConnection.InvokeAsync("WinPSResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.InvokeAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "cmd":
                        if (OSUtils.IsWindows)
                        {
                            var result = CMD.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("CMD", result);
                                await hubConnection.InvokeAsync("CMDResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.InvokeAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "bash":
                        if (OSUtils.IsLinux)
                        {
                            var result = Bash.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonSerializer.Serialize(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                await SendResultsViaAjax("Bash", result);
                                await hubConnection.InvokeAsync("BashResultViaAjax", commandID);
                            }
                            else
                            {
                                await hubConnection.InvokeAsync("CommandResult", result);
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
                await hubConnection.InvokeAsync("DisplayMessage", "There was an error executing the command.  It has been logged on the client device.", "Error executing command.", senderConnectionID);
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
            await webRequest.GetResponseAsync();
        }
    }
}
