using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Remotely_Library.Models
{
    public class PSError
    {
        public string Exception { get; set; }
        public string StackTrace { get; set; }

    }
}
