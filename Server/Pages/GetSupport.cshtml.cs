using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Remotely.Server.Pages;

public class GetSupportModel : PageModel
{
    private readonly IDataService _dataService;
    private readonly IEmailSenderEx _emailSender;

    public GetSupportModel(IDataService dataService, IEmailSenderEx emailSender)
    {
        _dataService = dataService;
        _emailSender = emailSender;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost(string deviceId)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var deviceResult = await _dataService.GetDevice(deviceId);
        if (!deviceResult.IsSuccess)
        {
            StatusMessage = "Device not found.";
            return Page();
        }

        var orgId = deviceResult.Value.OrganizationID;
        
        var alertParts = new string[]
        {
            $"{Input.Name} is requesting support.",
            $"Device ID: {deviceId}",
            $"Email: {Input.Email}.",
            $"Phone: {Input.Phone}.",
            $"Chat OK: {Input.ChatResponseOk}."
        };

        var alertMessage = string.Join("  ", alertParts);
        await _dataService.AddAlert(deviceId, orgId, alertMessage);

        var orgUsers = await _dataService.GetAllUsersInOrganization(orgId);
        var emailMessage = string.Join("<br />", alertParts);
        foreach (var user in orgUsers)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }
            await _emailSender.SendEmailAsync(user.Email, "Support Request", emailMessage);
        }

        StatusMessage = "We got it!  Someone will contact you soon.";

        return RedirectToPage("GetSupport", new { deviceId });
    }

    public class InputModel
    {
        [StringLength(150)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool ChatResponseOk { get; set; }
    }
}