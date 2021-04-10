using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class ScriptExecutor
    {
        public ScriptExecutor(ConfigService configService)
        {
            ConfigService = configService;
        }

        private ConfigService ConfigService { get; }

        public async Task RunCommandFromApi(ScriptingShell shell,
            string requestID,
            string command,
            string senderUsername,
            string authToken,
            HubConnection hubConnection)
        {
            try
            {

                var result = ExecuteScriptContent(shell, requestID, command, TimeSpan.FromMinutes(AppConstants.ScriptRunExpirationMinutes));

                result.InputType = ScriptInputType.Api;
                result.SenderUserName = senderUsername;

                await SendResultsToApi(result, authToken);
                await hubConnection.SendAsync("ScriptResultViaApi", requestID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public async Task RunCommandFromTerminal(ScriptingShell shell,
                    string command,
                    string authToken,
                    string senderUsername,
                    string senderConnectionID,
                    ScriptInputType scriptInputType,
                    TimeSpan timeout,
                    HubConnection hubConnection)
        {
            try
            {
                var result = ExecuteScriptContent(shell, senderConnectionID, command, timeout);

                result.InputType = scriptInputType;
                result.SenderUserName = senderUsername;

                var responseResult = await SendResultsToApi(result, authToken);
                if (responseResult is null)
                {
                    return;
                }
                await hubConnection.SendAsync("ScriptResult", responseResult.ID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage",
                    "There was an error executing the command. It has been logged on the client device.",
                    "Error executing command.",
                    "bg-danger",
                    senderConnectionID);
            }
        }

        public async Task RunScript(Guid savedScriptId,
                    int scriptRunId,
                    string initiator,
                    ScriptInputType scriptInputType,
                    string authToken)
        {
            try
            {
                Logger.Write($"Script run started.  Script ID: {savedScriptId}. Script Run: {scriptRunId}. Initiator: {initiator}.");

                var connectionInfo = ConfigService.GetConnectionInfo();
                var url = $"{connectionInfo.Host}/API/SavedScripts/{savedScriptId}";
                using var hc = new HttpClient();
                hc.DefaultRequestHeaders.Add("Authorization", authToken);
                var savedScript = await hc.GetFromJsonAsync<SavedScript>(url);

                var result = ExecuteScriptContent(savedScript.Shell,
                    Guid.NewGuid().ToString(),
                    savedScript.Content,
                    TimeSpan.FromMinutes(AppConstants.ScriptRunExpirationMinutes));

                result.SenderUserName = initiator;
                result.ScriptRunId = scriptRunId;
                result.InputType = scriptInputType;
                result.SavedScriptId = savedScriptId;

                var responseResult = await SendResultsToApi(result, authToken);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        // TODO: Async/await.
        private ScriptResult ExecuteScriptContent(ScriptingShell shell,
            string terminalSessionId,
            string command,
            TimeSpan timeout)
        {
            switch (shell)
            {
                case ScriptingShell.PSCore:
                    return PSCore.GetCurrent(terminalSessionId).WriteInput(command);

                case ScriptingShell.WinPS:
                    if (EnvironmentHelper.IsWindows)
                    {
                        return ExternalScriptingShell
                          .GetCurrent(ScriptingShell.WinPS, terminalSessionId)
                          .WriteInput(command, timeout);

                    }
                    break;

                case ScriptingShell.CMD:
                    if (EnvironmentHelper.IsWindows)
                    {
                        return ExternalScriptingShell
                             .GetCurrent(ScriptingShell.CMD, terminalSessionId)
                             .WriteInput(command, timeout);
                    }
                    break;

                case ScriptingShell.Bash:
                    return ExternalScriptingShell
                        .GetCurrent(ScriptingShell.Bash, terminalSessionId)
                        .WriteInput(command, timeout);
                default:
                    break;
            }
            return null;
        }
        private async Task<ScriptResult> SendResultsToApi(object result, string authToken)
        {
            var targetURL = ConfigService.GetConnectionInfo().Host + $"/API/ScriptResults";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            using var response = await httpClient.PostAsJsonAsync(targetURL, result);

            if (!response.IsSuccessStatusCode)
            {
                Logger.Write($"Failed to send script results.  Status Code: {response.StatusCode}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ScriptResult>(content, JsonSerializerHelper.CaseInsensitiveOptions);
        }
    }
}