using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ApiTokensModel : PageModel
    {
        public ApiTokensModel(DataService dataService)
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

        private DataService DataService { get; }

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
                var newToken = await DataService.CreateApiToken(User.Identity.Name, Input.TokenName);
                NewTokenKey = Guid.Parse(newToken.Token);
                NewTokenSecret = newToken.Secret;
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
