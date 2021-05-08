using System;
using System.Net;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public interface IWebClientEx : IDisposable
    {
        void SetRequestTimeout(int requestTimeoutMs);
        Task DownloadFileTaskAsync(string address, string fileName);
    }

    public class WebClientEx : WebClient, IWebClientEx
    {
        private int _requestTimeout;

        public void SetRequestTimeout(int requestTimeoutMs)
        {
            _requestTimeout = requestTimeoutMs;
        }
        
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            webRequest.Timeout = _requestTimeout;
            return webRequest;
        }
    }

}