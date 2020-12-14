using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IChatHostService
    {
        Task StartChat(string requesterID, string organizationName);
    }
}
