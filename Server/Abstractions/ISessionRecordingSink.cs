using Immense.RemoteControl.Server.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Server.Abstractions;

/// <summary>
/// The service is responsible for storing session recordings.
/// </summary>
public interface ISessionRecordingSink
{
    /// <summary>
    /// Sink a live webm stream to persistent storage. 
    /// </summary>
    /// <param name="webmStream"></param>
    /// <param name="hubCallerContext"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    Task SinkWebmStream(
        IAsyncEnumerable<byte[]> webmStream,
        RemoteControlSession session);
}
