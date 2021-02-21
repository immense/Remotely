using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.IO;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public class AgentsModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<RemotelyUser> _userManager;

        public AgentsModel(
            UserManager<RemotelyUser> userManager, 
            IDataService dataService,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _dataService = dataService;
            _hostEnvironment = hostEnvironment;
        }

        public string OrganizationId { get; private set; }
        public bool IsServerUrlEmbedded { get; private set; }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                OrganizationId = _dataService.GetOrganizationById(currentUser.OrganizationID)?.ID;
            }
            else
            {
                OrganizationId = (await _dataService.GetDefaultOrganization())?.ID;
            }

            var appFilePath = Path.Combine(
                _hostEnvironment.WebRootPath,
                "Downloads",
                "Win-x64",
                "ClickOnce",
                "Remotely_Desktop.application");

            try
            {
                await ClickOnceMiddleware.AppFileLock.WaitAsync();
                var appContent = await System.IO.File.ReadAllTextAsync(appFilePath);
                IsServerUrlEmbedded = appContent.Contains($"{Request.Scheme}://{Request.Host}");
            }
            finally
            {
                ClickOnceMiddleware.AppFileLock.Release();
            }
        }
    }
}