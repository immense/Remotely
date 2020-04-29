using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.Models
{
    public class IceServerModel
    {
        public string Url { get; set; }
        public string TurnPassword { get; set; }
        public string TurnUsername { get; set; }
    }
}
