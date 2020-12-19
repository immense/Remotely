using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IChatUiService
    {
        event EventHandler ChatWindowClosed;

        void ShowChatWindow(string organizationName, StreamWriter writer);
        void ReceiveChat(ChatMessage chatMessage);
    }
}
