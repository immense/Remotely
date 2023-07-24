using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Remotely.Shared;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Agent.Services;

public interface IScriptExecutor
{
    Task RunCommandFromApi(ScriptingShell shell, string requestID, string command, string senderUsername, string authToken, HubConnection hubConnection);
    Task RunCommandFromTerminal(ScriptingShell shell, string command, string authToken, string senderUsername, string senderConnectionID, ScriptInputType scriptInputType, TimeSpan timeout, HubConnection hubConnection);
    Task RunScript(Guid savedScriptId, int scriptRunId, string initiator, ScriptInputType scriptInputType, string authToken);
}

public class ScriptExecutor : IScriptExecutor
{
    private readonly IConfigService _configService;
    private readonly ILogger<ScriptExecutor> _logger;

    public ScriptExecutor(IConfigService configService, ILogger<ScriptExecutor> logger)
    {
        _configService = configService;
        _logger = logger;
    }


    public async Task RunCommandFromApi(ScriptingShell shell,
        string requestID,
        string command,
        string senderUsername,
        string authToken,
        HubConnection hubConnection)
    {
        try
        {

            var result = await ExecuteScriptContent(shell, requestID, command, TimeSpan.FromMinutes(AppConstants.ScriptRunExpirationMinutes));

            result.InputType = ScriptInputType.Api;
            result.SenderUserName = senderUsername;

            await SendResultsToApi(result, authToken);
            await hubConnection.SendAsync("ScriptResultViaApi", requestID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while running command from API.");
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
            var result = await ExecuteScriptContent(shell, senderConnectionID, command, timeout);

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
            _logger.LogError(ex, "Error while running command from terminal.");
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
            _logger.LogInformation(
                "Script run started.  Script ID: {savedScriptId}. Script Run: {scriptRunId}. Initiator: {initiator}.",
                savedScriptId,
                scriptRunId,
                initiator);

            var connectionInfo = _configService.GetConnectionInfo();
            var url = $"{connectionInfo.Host}/API/SavedScripts/{savedScriptId}";
            using var hc = new HttpClient();
            hc.DefaultRequestHeaders.Add("Authorization", authToken);
            var response = await hc.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return;
                }
                _logger.LogWarning("Failed to get saved script.  Status Code: {responseStatusCode}", response.StatusCode);
                return;
            }

            var savedScript = await response.Content.ReadFromJsonAsync<SavedScript>();

            if (savedScript is null)
            {
                _logger.LogWarning("Failed to deserialize saved script.");
                return;
            }

            if (string.IsNullOrWhiteSpace(savedScript.Content))
            {
                _logger.LogWarning("Script content is empty.  Aborting script run.");
                return;
            }

            var result = await ExecuteScriptContent(savedScript.Shell,
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
            _logger.LogError(ex, "Error while running script.");
        }
    }

    private async Task<ScriptResult> ExecuteScriptContent(
        ScriptingShell shell,
        string terminalSessionId,
        string command,
        TimeSpan timeout)
    {
        switch (shell)
        {
            case ScriptingShell.PSCore:
                return await PsCoreShell
                    .GetCurrent(terminalSessionId)
                    .WriteInput(command);

            case ScriptingShell.WinPS:
                if (EnvironmentHelper.IsWindows)
                {
                    var instance = await ExternalScriptingShell
                      .GetCurrent(ScriptingShell.WinPS, terminalSessionId);

                    return await instance.WriteInput(command, timeout);
                }
                break;

            case ScriptingShell.CMD:
                if (EnvironmentHelper.IsWindows)
                {
                    var instance = await ExternalScriptingShell
                         .GetCurrent(ScriptingShell.CMD, terminalSessionId);

                    return await instance.WriteInput(command, timeout);
                }
                break;

            case ScriptingShell.Bash:
                {
                    var instance = await ExternalScriptingShell
                        .GetCurrent(ScriptingShell.Bash, terminalSessionId);

                    return await instance.WriteInput(command, timeout);
                }
            default:
                break;
        }
        return null;
    }
    private async Task<ScriptResult> SendResultsToApi(object result, string authToken)
    {
        var targetURL = _configService.GetConnectionInfo().Host + $"/API/ScriptResults";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

        using var response = await httpClient.PostAsJsonAsync(targetURL, result);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send script results.  Status Code: {responseStatusCode}", response.StatusCode);
            return default;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ScriptResult>(content, JsonSerializerHelper.CaseInsensitiveOptions);
    }
}