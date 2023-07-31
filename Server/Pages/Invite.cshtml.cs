using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using System.Threading.Tasks;

namespace Remotely.Server.Pages;

[Authorize]
public class InviteModel : PageModel
{
    private readonly IDataService _dataService;

    public InviteModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public bool Success { get; set; }

    public class InputModel
    {
        public string? InviteID { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public void OnGet(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            ModelState.AddModelError("MissingID", "No invititation ID is specified.");
            return;
        }

        Input.InviteID = id;
    }

    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input?.InviteID))
        {
            Success = false;
            ModelState.AddModelError("MissingID", "No invititation ID is specified.");
            return Page();
        }

        var result = await _dataService.JoinViaInvitation($"{User.Identity?.Name}", Input.InviteID);
        if (!result.IsSuccess)
        {
            Success = false;
            ModelState.AddModelError("InviteIDNotFound", "The invitation ID wasn't found or is for another account.");
        }

        Success = true;
        return Page();
    }
}