using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public class GetSupportModel : PageModel
    {
        public GetSupportModel(IDataService dataService)
        {
            DataService = dataService;
        }


        private IDataService DataService { get; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost(string deviceID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orgID = DataService.GetDevice(deviceID)?.OrganizationID;

            var alertMessage = $"{Input.Name} is requesting support.  " +
                    $"Email: {Input.Email}.  " +
                    $"Phone: {Input.Phone}.  " +
                    $"Chat OK: {Input.ChatResponseOk}.";

            await DataService.AddAlert(deviceID, orgID, alertMessage);

            StatusMessage = "We got it!  Someone will contact you soon.";

            return RedirectToPage("GetSupport", new { deviceID });
        }

        public class InputModel
        {
            [StringLength(150)]
            [Required]
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public bool ChatResponseOk { get; set; }
        }
    }
}