using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Shared.Messages;

using Remotely.Desktop.Shared.Enums;

public sealed class ButtonActionMessage
{
    public ButtonAction Action { get; set; }
}
