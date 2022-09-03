using CommunityToolkit.Mvvm.Messaging;
using Immense.RemoteControl.Desktop.Shared.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Win.Services
{
    public interface IAppStateEx : IAppState
    {
        string OrganizationId { get; set; }
    }
    public class AppStateEx : AppState, IAppStateEx
    {
        public AppStateEx(
            IMessenger messenger,
            ILogger<AppStateEx> logger) 
            : base(messenger, logger)
        {
        }

        public string OrganizationId { get; set; } = string.Empty;
    }
}
