using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Remotely.Shared.Models
{
    public class PSCoreCommandResult
    {
        public string CommandResultID { get; set; }
        public string DeviceID { get; set; }
        [NotMapped]
        public List<string> VerboseOutput { get; set; }
        [NotMapped]
        public List<string> DebugOutput { get; set; }
        [NotMapped]
        public List<string> ErrorOutput { get; set; }
        [NotMapped]
        public string HostOutput { get; set; }
        [NotMapped]
        public List<string> InformationOutput { get; set; }
        [NotMapped]
        public List<string> WarningOutput { get; set; }
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}
