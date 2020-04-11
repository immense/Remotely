using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely.Server.Pages
{
    [Authorize("RemoteControlPolicy")]
    public class RemoteControlModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}