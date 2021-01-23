using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IChatClientService
    {
        Task StartChat(string requesterID, string organizationName);
    }
}
