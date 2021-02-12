using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public class AgentsModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly UserManager<RemotelyUser> _userManager;
        public AgentsModel(UserManager<RemotelyUser> userManager, IDataService dataService)
        {
            _userManager = userManager;
            _dataService = dataService;
        }

        public Organization CurrentOrganization { get; private set; }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                CurrentOrganization = _dataService.GetOrganizationById(currentUser.OrganizationID);
            }
        }
    }
}