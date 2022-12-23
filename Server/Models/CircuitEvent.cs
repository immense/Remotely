using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Models
{
    public class CircuitEvent
    {
        public CircuitEvent(CircuitEventName eventName, params object[] args)
        {
            EventName = eventName;
            Params = args;
        }

        public CircuitEventName EventName { get; set; }

        // TODO: This is a bad shortcut.  Make events for each type.
        public object[] Params { get; set; }
    }
}
