using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;

namespace Remotely.Server.Pages
{
    public class GetSupportModel : PageModel
    {
        public GetSupportModel(DataService dataService)
        {
            DataService = dataService;
        }


        private DataService DataService { get; }

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

            await DataService.AddAlert(new Remotely.Shared.Models.AlertOptions()
            {
                AlertDeviceID = deviceID,
                AlertMessage = $"{Input.Name} is requesting support.  " +
                    $"Email: {Input.Email}.  " +
                    $"Phone: {Input.Phone}.  " +
                    $"Chat OK: {Input.ChatResponseOk}.",
                ShouldAlert = true
            }, orgID);

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