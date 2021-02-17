using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class BrandingModel : PageModel
    {
        private readonly IDataService _dataService;

        public BrandingModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public RemotelyUser CurrentUser { get; set; }
        public Organization Organization { get; set; }

        [BindProperty]
        [DisplayName("Product Name")]
        [StringLength(25)]
        [Required]
        public string ProductName { get; set; }

        [BindProperty]
        [DisplayName("Icon")]
        public IFormFile Icon { get; set; }


        [BindProperty]
        public ColorPickerModel TitleForegroundPicker { get; set; } = new ColorPickerModel();

        [BindProperty]
        public ColorPickerModel TitleBackgroundPicker { get; set; } = new ColorPickerModel();

        [BindProperty]
        public ColorPickerModel TitleButtonPicker { get; set; } = new ColorPickerModel();



        [TempData]
        public string StatusMessage { get; set; }

        public string Base64Icon { get; set; }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = _dataService.GetUserByName(User.Identity.Name);
                Organization = CurrentUser.Organization;

                var brandingInfo = await _dataService.GetBrandingInfo(Organization.ID);


                ProductName = brandingInfo.Product;

                if (brandingInfo?.Icon?.Any() == true)
                {
                    Base64Icon = Convert.ToBase64String(brandingInfo.Icon);
                }

                TitleForegroundPicker.Red = brandingInfo.TitleForegroundRed;
                TitleForegroundPicker.Green = brandingInfo.TitleForegroundGreen;
                TitleForegroundPicker.Blue = brandingInfo.TitleForegroundBlue;

                TitleBackgroundPicker.Red = brandingInfo.TitleBackgroundRed;
                TitleBackgroundPicker.Green = brandingInfo.TitleBackgroundGreen;
                TitleBackgroundPicker.Blue = brandingInfo.TitleBackgroundBlue;

                TitleButtonPicker.Red = brandingInfo.ButtonForegroundRed;
                TitleButtonPicker.Green = brandingInfo.ButtonForegroundGreen;
                TitleButtonPicker.Blue = brandingInfo.ButtonForegroundBlue;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUser = _dataService.GetUserByName(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _dataService.UpdateBrandingInfo(CurrentUser.OrganizationID, ProductName, Icon, TitleForegroundPicker, TitleBackgroundPicker, TitleButtonPicker);

            StatusMessage = "Branding information saved!";
            return RedirectToPage();
        }
    }
}
