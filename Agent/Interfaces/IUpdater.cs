using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces
{
    public interface IUpdater
    {
        Task BeginChecking();
        Task CheckForUpdates();
        Task InstallLatestVersion();
    }
}