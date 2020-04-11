using System;
using System.Collections.Generic;

namespace Remotely.Shared.Models
{
    public class PSCoreCommandResult
    {
        public string CommandResultID { get; set; }
        public string DeviceID { get; set; }
        public List<string> VerboseOutput { get; set; }
        public List<string> DebugOutput { get; set; }
        public List<string> ErrorOutput { get; set; }
        public string HostOutput { get; set; }
        public List<string> InformationOutput { get; set; }
        public List<string> WarningOutput { get; set; }
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}
