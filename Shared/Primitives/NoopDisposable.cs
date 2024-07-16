using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Primitives;
public class NoopDisposable : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
