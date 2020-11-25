using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ApiTokensModel : PageModel
    {
        public ApiTokensModel(IDataService dataService)
        {
            DataService = dataService;
        }

        public IEnumerable<ApiToken> ApiTokens { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public bool IsAdmin { get; private set; }

        [TempData]
        public string Message { get; set; }

        [TempData]
        public Guid NewTokenKey { get; set; }

        [TempData]
        public string NewTokenSecret { get; set; }

        private IDataService DataService { get; }

        public void OnGet()
        {
            PopulateViewModel();
        }

        public async Task<IActionResult> OnPostRenameAsync()
        {
            if (ModelState.IsValid &&
                !string.IsNullOrWhiteSpace(Input.TokenId) &&
                !string.IsNullOrWhiteSpace(Input.TokenName))
            {
                await DataService.RenameApiToken(User.Identity.Name, Input.TokenId, Input.TokenName);
                Message = "Token renamed.";
            }
            PopulateViewModel();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(Input.TokenName))
            {
                var secret = PasswordGenerator.GeneratePassword(24);
                var secretHash = new PasswordHasher<RemotelyUser>().HashPassword(null, secret);

                var newToken = await DataService.CreateApiToken(User.Identity.Name, Input.TokenName, secretHash);
                NewTokenKey = Guid.Parse(newToken.Token);
                NewTokenSecret = secret;
                Message = "New token created.";
            }
            PopulateViewModel();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(Input.TokenId))
            {
                await DataService.DeleteApiToken(User.Identity.Name, Input.TokenId);
                Message = "Token deleted.";
            }
            PopulateViewModel();
            return RedirectToPage();
        }

        private void PopulateViewModel()
        {
            var currentUser = DataService.GetUserByName(User.Identity.Name);
            IsAdmin = currentUser.IsAdministrator;
            ApiTokens = DataService.GetAllApiTokens(currentUser.Id);
            Input.TokenId = null;
            Input.TokenName = null;
        }

        public class InputModel
        {
            [StringLength(200)]
            public string TokenName { get; set; }

            public string TokenId { get; set; }
        }
    }
}
