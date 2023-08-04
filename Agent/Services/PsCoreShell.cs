using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Dtos;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Remotely.Agent.Services;

public interface IPsCoreShell : IDisposable, IScriptingShell
{
    string? SenderConnectionId { get; set; }

    CommandCompletion GetCompletions(string inputText, int currentIndex, bool? forward);
    Task<ScriptResultDto> WriteInput(string input);
}

public class PsCoreShell : IPsCoreShell
{
    private readonly IConfigService _configService;
    private readonly ConnectionInfo _connectionInfo;
    private readonly ILogger<PsCoreShell> _logger;
    private readonly PowerShell _powershell;
    private bool _disposedValue;
    private CommandCompletion? _lastCompletion;
    private string? _lastInputText;

    public PsCoreShell(
        IConfigService configService,
        ILogger<PsCoreShell> logger)
    {
        _configService = configService;
        _connectionInfo = _configService.GetConnectionInfo();
        _logger = logger;

        _powershell = PowerShell.Create();

        _powershell.AddScript($@"$VerbosePreference = ""Continue"";
                            $DebugPreference = ""Continue"";
                            $InformationPreference = ""Continue"";
                            $WarningPreference = ""Continue"";
                            $env:DeviceId = ""{_connectionInfo.DeviceID}"";
                            $env:ServerUrl = ""{_connectionInfo.Host}""");

        _powershell.Invoke();
    }

    public bool IsDisposed => _disposedValue;
    public string? SenderConnectionId { get; set; }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public CommandCompletion GetCompletions(string inputText, int currentIndex, bool? forward)
    {
        if (_lastCompletion is null ||
            inputText != _lastInputText)
        {
            _lastInputText = inputText;
            _lastCompletion = CommandCompletion.CompleteInput(inputText, currentIndex, new(), _powershell);
        }

        if (forward.HasValue)
        {
            _lastCompletion.GetNextResult(forward.Value);
        }

        return _lastCompletion;
    }

    public async Task<ScriptResultDto> WriteInput(string input)
    {
        var deviceId = _configService.GetConnectionInfo().DeviceID;
        var sw = Stopwatch.StartNew();

        try
        {

            _powershell.Streams.ClearStreams();
            _powershell.Commands.Clear();

            _powershell.AddScript(input);
            var results = _powershell.Invoke();

            using var ps = PowerShell.Create();
            ps.AddScript("$args[0] | Out-String");
            ps.AddArgument(results);
            var result = await ps.InvokeAsync();
            
            var hostOutput = result.Count > 0 ? 
                $"{result[0].BaseObject}" : 
                string.Empty;

            var verboseOut = _powershell.Streams.Verbose.ReadAll().Select(x => x.Message);
            var debugOut = _powershell.Streams.Debug.ReadAll().Select(x => x.Message);
            var errorOut = _powershell.Streams.Error.ReadAll().Select(x => x.Exception.ToString() + Environment.NewLine + x.ScriptStackTrace);
            var infoOut = _powershell.Streams.Information.Select(x => x.MessageData.ToString());
            var warningOut = _powershell.Streams.Warning.Select(x => x.Message);

            var standardOut = hostOutput.Split(Environment.NewLine)
                .Concat(infoOut)
                .Concat(debugOut)
                .Concat(verboseOut)
                .Select(x => $"{x}");

            var errorAndWarningOut = errorOut.Concat(warningOut).ToArray();


            return new ScriptResultDto()
            {
                DeviceID = _configService.GetConnectionInfo().DeviceID,
                SenderConnectionID = SenderConnectionId,
                ScriptInput = input,
                Shell = Shared.Enums.ScriptingShell.PSCore,
                StandardOutput = standardOut.ToArray(),
                ErrorOutput = errorAndWarningOut,
                RunTime = sw.Elapsed,
                HadErrors = _powershell.HadErrors || errorAndWarningOut.Any()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while writing input to PSCore.");
            return new ScriptResultDto()
            {
                DeviceID = deviceId,
                SenderConnectionID = SenderConnectionId,
                ScriptInput = input,
                Shell = Shared.Enums.ScriptingShell.PSCore,
                StandardOutput = Array.Empty<string>(),
                ErrorOutput = new[] { "Error while writing input." },
                RunTime = sw.Elapsed,
                HadErrors = true
            };
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _powershell.Dispose();
            }
            _disposedValue = true;
        }
    }
}
