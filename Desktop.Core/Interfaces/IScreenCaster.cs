using Remotely.Shared.Models;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IScreenCaster
    {
        Task BeginScreenCasting(ScreenCastRequest screenCastRequest);
    }
}
