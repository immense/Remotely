using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Remotely.Agent.Models
{
    public class ChatSession
    {
        public int ProcessID { get; set; }
        public NamedPipeClientStream PipeStream { get; set; }
    }
}
