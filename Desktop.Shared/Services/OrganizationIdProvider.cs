using CommunityToolkit.Mvvm.Messaging;
using Immense.RemoteControl.Desktop.Shared.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Shared.Services
{
    public interface IOrganizationIdProvider
    {
        string OrganizationId { get; set; }
    }
    public class OrganizationIdProvider : IOrganizationIdProvider
    {
        public string OrganizationId { get; set; } = string.Empty;
    }
}
