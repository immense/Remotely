using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Data;

namespace Remotely.Server.Pages
{
    [Authorize]
    public class EditDeviceModel : PageModel
    {
        public EditDeviceModel(DataService dataService)
        {
            DataService = dataService;
        }

        public string AgentVersion { get; set; }
        public List<SelectListItem> DeviceGroups { get; } = new List<SelectListItem>();
        public string DeviceName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public bool SaveSucessful { get; set; }


        private DataService DataService { get; }

        public void OnGet(string deviceID, bool success)
        {
            SaveSucessful = success;
            var user = DataService.GetUserByName(User.Identity.Name);
            if (user != null)
            {
                var device = DataService.GetDeviceForUser(user.Id, deviceID);
                DeviceName = device?.DeviceName;
                AgentVersion = device.AgentVersion;
                Input.Alias = device?.Alias;
                Input.DeviceGroupID = device?.DeviceGroupID;
                Input.Tags = device?.Tags;

            }
            var groups = DataService.GetDeviceGroupsForUserName(User.Identity.Name);
            DeviceGroups.AddRange(groups.Select(x => new SelectListItem(x.Name, x.ID)));
        }

        public IActionResult OnPost(string deviceID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DataService.UpdateDevice(deviceID, Input.Tags, Input.Alias, Input.DeviceGroupID);

            return RedirectToPage("EditDevice", new { deviceID, success = true });
        }

        public class InputModel
        {
            [StringLength(100)]
            public string Alias { get; set; }

            public string DeviceGroupID { get; set; }

            [StringLength(200)]
            public string Tags { get; set; }
        }
    }
}
