using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IShutdownService
    {
        Task Shutdown();
    }
}
