using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_Desktop.Models
{
    public class Viewer
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public bool HasControl { get; set; }
    }
}
