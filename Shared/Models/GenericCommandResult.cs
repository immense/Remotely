using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.Models
{
    public class GenericCommandResult
    {
        public string DeviceID { get; set; }
        public string CommandContextID { get; set; }
        public string CommandType { get; set; }
        public string StandardOutput { get; set; }
        public string ErrorOutput { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
