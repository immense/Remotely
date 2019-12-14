using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IScreenCaster
    {
        Task BeginScreenCasting(ScreenCastRequest screenCastRequest);
    }
}
