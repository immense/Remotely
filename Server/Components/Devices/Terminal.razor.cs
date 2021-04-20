using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Remotely.Server.Components.ModalContents;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices
{
    public partial class Terminal : AuthComponentBase, IDisposable
    {
        private string _inputText;

        private string _lastCompletionInput;
        private int _lastCursorIndex;
        private ScriptingShell _shell;

        private string _terminalOpenClass;

        private ElementReference _terminalInput;
        private ElementReference _terminalWindow;

        [Inject]
        private IModalService ModalService { get; set; }

        [Inject]
        private IClientAppState AppState { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        [Inject]
        private IDataService DataService { get; set; }

        [Inject]
        private ILogger<Terminal> Logger { get; set; }

        private string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                if (TryMatchShellShortcuts())
                {
                    _inputText = string.Empty;
                }
            }
        }

        [Inject]
        private IJsInterop JsInterop { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        public void Dispose()
        {
            AppState.PropertyChanged -= AppState_PropertyChanged;
            CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
            AppState.PropertyChanged += AppState_PropertyChanged;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JsInterop.PreventTabOut(_terminalInput);
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        private void ApplyCompletion(PwshCommandCompletion completion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_lastCompletionInput))
                {
                    return;
                }

                var match = completion.CompletionMatches[completion.CurrentMatchIndex];

                var replacementText = string.Concat(
                    _lastCompletionInput.Substring(0, completion.ReplacementIndex),
                    match.CompletionText,
                    _lastCompletionInput[(completion.ReplacementIndex + completion.ReplacementLength)..]);

                InputText = replacementText;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error appying command completion.");
            }
        }

        private void AppState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.TerminalLines))
            {
                InvokeAsync(StateHasChanged);
                JsInterop.ScrollToEnd(_terminalWindow);
            }
        }

        private void CircuitConnection_MessageReceived(object sender, Models.CircuitEvent e)
        {
            if (e.EventName == Models.CircuitEventName.PowerShellCompletions)
            {
                var completion = (PwshCommandCompletion)e.Params[0];
                var intent = (CompletionIntent)e.Params[1];

                switch (intent)
                {
                    case CompletionIntent.ShowAll:
                        DisplayCompletions(completion.CompletionMatches);
                        break;
                    case CompletionIntent.NextResult:
                        ApplyCompletion(completion);
                        break;
                    default:
                        break;
                }
                AppState.InvokePropertyChanged(nameof(AppState.TerminalLines));
            }
        }
        private void DisplayCompletions(List<PwshCompletionResult> completionMatches)
        {
            var deviceId = AppState.DevicesFrameSelectedDevices.FirstOrDefault();
            var device = AgentHub.GetDevice(deviceId);

            AppState.AddTerminalLine($"Completions for {device?.DeviceName}", className: "font-weight-bold");

            foreach (var match in completionMatches)
            {
                AppState.AddTerminalLine(match.CompletionText, className: "", title: match.ToolTip);
            }
        }
        private void EvaluateInputKeypress(KeyboardEventArgs ev)
        {
            if (ev.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(InputText) || ev.ShiftKey)
                {
                    return;
                }

                var devices = AppState.DevicesFrameSelectedDevices.ToArray();
                if (!devices.Any())
                {
                    ToastService.ShowToast("You must select at least one device.", classString: "bg-warning");
                    return;
                }
                CircuitConnection.ExecuteCommandOnAgent(_shell, InputText, devices);
                AppState.AddTerminalHistory(InputText);
                InputText = string.Empty;
            }
        }

        private void ShowTerminalHelp()
        {
            ModalService.ShowModal("Terminal Help", new[]
            {
                "Enter terminal commands that will execute on all selected devices.",

                "Tab completion is available for PowerShell Core (PSCore) and Windows PowerShell (WinPS).  Tab and Shift + Tab " +
                "will cycle through potential completions.  Ctrl + Space will show all available completions.",

                "If more than one devices is selected, the first device's file system will be used when " +
                "auto-completing file and directory paths.",

                "PowerShell Core is cross-platform and is available on all client operating systems.  Bash is available " +
                "on Windows 10 if WSL (Windows Subsystem for Linux) is installed.",

                "Note: The first PS Core command or tab completion takes a few moments while the service is " +
                "starting on the remote device."
            });
        }
        private async Task EvaluateKeyDown(KeyboardEventArgs ev)
        {
            if (!ev.Key.Equals("Tab", StringComparison.OrdinalIgnoreCase) &&
                !ev.Key.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                _lastCompletionInput = null;
            }


            if (ev.Key.Equals("ArrowUp", StringComparison.OrdinalIgnoreCase))
            {
                InputText = AppState.GetTerminalHistory(false);
            }
            else if (ev.Key.Equals("ArrowDown", StringComparison.OrdinalIgnoreCase))
            {
                InputText = AppState.GetTerminalHistory(true);
            }
            else if (ev.Key.Equals("Tab", StringComparison.OrdinalIgnoreCase))
            {
                if (_shell != ScriptingShell.PSCore && _shell != ScriptingShell.WinPS)
                {
                    ToastService.ShowToast("PowerShell is required for tab completion.", classString: "bg-warning");
                    return;
                }

                if (!AppState.DevicesFrameSelectedDevices.Any())
                {
                    ToastService.ShowToast("No devices are selected.", classString: "bg-warning");
                    return;
                }

                if (string.IsNullOrWhiteSpace(InputText))
                {
                    return;
                }

                await GetNextCompletion(!ev.ShiftKey);
            }
            else if (ev.CtrlKey && ev.Key.Equals(" ", StringComparison.OrdinalIgnoreCase))
            {
                if (!AppState.DevicesFrameSelectedDevices.Any())
                {
                    return;
                }

                await ShowAllCompletions();
            }
        }

        private async Task GetNextCompletion(bool forward)
        {
            if (string.IsNullOrWhiteSpace(_lastCompletionInput))
            {
                _lastCompletionInput = InputText;
                _lastCursorIndex = await JsInterop.GetCursorIndex(_terminalInput);
            }

            await CircuitConnection.GetPowerShellCompletions(_lastCompletionInput, _lastCursorIndex, CompletionIntent.NextResult, forward);
        }

        private async Task ShowAllCompletions()
        {
            if (string.IsNullOrWhiteSpace(_lastCompletionInput))
            {
                _lastCompletionInput = InputText;
                _lastCursorIndex = await JsInterop.GetCursorIndex(_terminalInput);
            }

            await CircuitConnection.GetPowerShellCompletions(_lastCompletionInput, _lastCursorIndex, CompletionIntent.ShowAll, false);
        }
        private void ToggleTerminalOpen()
        {
            if (string.IsNullOrWhiteSpace(_terminalOpenClass))
            {
                _terminalOpenClass = "open";
            }
            else
            {
                _terminalOpenClass = string.Empty;
            }
        }

        private async Task ShowQuickScripts()
        {
            var quickScripts = await DataService.GetQuickScripts(User.Id);
            if (quickScripts?.Any() != true)
            {
                ToastService.ShowToast("No quick scripts saved.", classString: "bg-warning");
                return;
            }

            if (!AppState.DevicesFrameSelectedDevices.Any())
            {
                ToastService.ShowToast("You must select at least one device.", classString: "bg-warning");
                return;
            }

         
            void showModal(RenderTreeBuilder builder)
            {
                builder.OpenComponent<QuickScriptsSelector>(0);
                builder.AddAttribute(1, "QuickScripts", quickScripts);
                builder.AddAttribute(2, "OnRunClicked", RunQuickScript);
                builder.CloseComponent();
            }

            await ModalService.ShowModal("Quick Scripts", showModal);
        }

        private EventCallback<SavedScript> RunQuickScript => 
            EventCallback.Factory.Create<SavedScript>(this, async script =>
            {
                var scriptRun = new ScriptRun()
                {
                    OrganizationID = User.OrganizationID,
                    RunAt = Time.Now,
                    SavedScriptId = script.Id,
                    RunOnNextConnect = false,
                    Initiator = User.UserName,
                    InputType = ScriptInputType.OneTimeScript
                };

                scriptRun.Devices = DataService.GetDevices(AppState.DevicesFrameSelectedDevices);

                await DataService.AddScriptRun(scriptRun);

                await CircuitConnection.RunScript(AppState.DevicesFrameSelectedDevices, script.Id, scriptRun.Id, ScriptInputType.OneTimeScript, false);

                ToastService.ShowToast($"Running script on {scriptRun.Devices.Count} devices.");
            });

        private bool TryMatchShellShortcuts()
        {
            var currentText = InputText?.Trim()?.ToLower();

            if (string.IsNullOrWhiteSpace(currentText))
            {
                return false;
            }

            if (currentText.Equals(User.UserOptions.CommandModeShortcutPSCore.ToLower()))
            {
                _shell = ScriptingShell.PSCore;
                return true;
            }
            else if (currentText.Equals(User.UserOptions.CommandModeShortcutCMD.ToLower()))
            {
                _shell = ScriptingShell.CMD;
                return true;
            }
            else if (currentText.Equals(User.UserOptions.CommandModeShortcutWinPS.ToLower()))
            {
                _shell = ScriptingShell.WinPS;
                return true;
            }
            else if (currentText.Equals(User.UserOptions.CommandModeShortcutBash.ToLower()))
            {
                _shell = ScriptingShell.Bash;
                return true;
            }
            return false;
        }
    }
}
