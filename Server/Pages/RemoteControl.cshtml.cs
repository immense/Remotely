using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Auth;

namespace Remotely.Server.Pages
{
    [ServiceFilter(typeof(RemoteControlFilterAttribute))]
    public class RemoteControlModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
