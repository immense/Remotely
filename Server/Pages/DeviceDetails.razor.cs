using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Components;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public partial class DeviceDetails : AuthComponentBase
    {
        private readonly ConcurrentQueue<string> _logLines = new();
        private readonly ConcurrentQueue<ScriptResult> _scriptResults = new();

        private string _alertMessage;
        private string _inputDeviceId;

        [Parameter]
        public string DeviceId { get; set; }

        [Parameter]
        public string ActiveTab { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        [Inject]
        private IDataService DataService { get; set; }

        private Device Device { get; set; }

        [Inject]
        private IModalService ModalService { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (!string.IsNullOrWhiteSpace(DeviceId))
            {
                Device = DataService.GetDevice(DeviceId);
            }

            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
        }

        private void CircuitConnection_MessageReceived(object sender, Models.CircuitEvent e)
        {
            if (e.EventName == Models.CircuitEventName.RemoteLogsReceived)
            {
                var logChunk = (string)e.Params[0];
                _logLines.Enqueue(logChunk);
                InvokeAsync(StateHasChanged);
            }
        }

        private void EditFormKeyDown()
        {
            _alertMessage = string.Empty;
        }

        private void EvaluateDeviceIdInputKeyDown(KeyboardEventArgs args)
        {
            if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
            {
                NavManager.NavigateTo($"/device-details/{_inputDeviceId}");
            }
        }

        private void GetRemoteLogs()
        {
            _logLines.Clear();

            if (Device.IsOnline)
            {
                CircuitConnection.GetRemoteLogs(Device.ID);
            }
        }

        private void GetScriptHistory()
        {
            _scriptResults.Clear();

            if (User.IsAdministrator)
            {
                var results = DataService
                    .GetAllScriptResults(User.OrganizationID, Device.ID)
                    .OrderByDescending(x => x.TimeStamp);

                foreach (var result in results)
                {
                    _scriptResults.Enqueue(result);
                }
            }
            else
            {
                var results = DataService
                    .GetAllCommandResultsForUser(User.OrganizationID, User.UserName, Device.ID)
                    .OrderByDescending(x => x.TimeStamp);

                foreach (var result in results)
                {
                    _scriptResults.Enqueue(result);
                }
            }
        }

        private string GetTrimmedText(string source, int stringLength)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "(none)";
            }

            if (source.Length <= stringLength)
            {
                return source;
            }

            return source[0..25] + "...";
        }

        private string GetTrimmedText(string[] source, int stringLength)
        {
            return GetTrimmedText(string.Join("", source), stringLength);
        }

        private Task HandleValidSubmit()
        {
            DataService.UpdateDevice(Device.ID,
                  Device.Tags,
                  Device.Alias,
                  Device.DeviceGroupID,
                  Device.Notes,
                  Device.WebRtcSetting);

            _alertMessage = "Device details saved.";
            ToastService.ShowToast("Device details saved.");

            return Task.CompletedTask;
        }

        private void NavigateToDeviceId()
        {
            NavManager.NavigateTo($"/device-details/{_inputDeviceId}");
        }

        private void ShowAllDisks()
        {
            var disksString = JsonSerializer.Serialize(Device.Drives, JsonSerializerHelper.IndentedOptions);
            void modalBody(RenderTreeBuilder builder)
            {
                builder.AddMarkupContent(0, $"<div style='white-space: pre'>{disksString}</div>");
            }
            ModalService.ShowModal($"All Disks for {Device.DeviceName}", modalBody);
        }

        private void ShowFullScriptOutput(ScriptResult result)
        {
            void outputModal(RenderTreeBuilder builder)
            {
                var output = string.Join("\r\n", result.StandardOutput);
                var error = string.Join("\r\n", result.ErrorOutput);
                var textareaStyle = "width: 100%; height: 200px; white-space: pre;";

                builder.AddMarkupContent(0, "<h5>Input</h5>");
                builder.AddMarkupContent(1, $"<textarea readonly style=\"{textareaStyle}\">{result.ScriptInput}</textarea>");
                builder.AddMarkupContent(2, "<h5 class=\"mt-3\">Standard Output</h5>");
                builder.AddMarkupContent(3, $"<textarea readonly style=\"{textareaStyle}\">{output}</textarea>");
                builder.AddMarkupContent(4, "<h5 class=\"mt-3\">Error Output</h5>");
                builder.AddMarkupContent(3, $"<textarea readonly style=\"{textareaStyle}\">{error}</textarea>");
            }

            ModalService.ShowModal("Script Input/Output", outputModal);
        }
    }
}