using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Remotely.Server.Pages
{
    [Authorize]
    public class EditDeviceModel : PageModel
    {
        public EditDeviceModel(IDataService dataService)
        {
            DataService = dataService;
        }

        public string AgentVersion { get; set; }
        public List<SelectListItem> DeviceGroups { get; } = new List<SelectListItem>();
        public string DeviceName { get; set; }
        public string DeviceID { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public bool SaveSucessful { get; set; }


        private IDataService DataService { get; }

        public IActionResult OnGet(string deviceID, bool success)
        {
            var user = DataService.GetUserByName(User.Identity.Name);
            var targetDevice = DataService.GetDevice(deviceID);
            if (targetDevice == null)
            {
                return Page();
            }
            else if (!DataService.DoesUserHaveAccessToDevice(deviceID, user))
            {
                DataService.WriteEvent($"Edit device attempted by unauthorized user.  Device ID: {deviceID}.  User Name: {user.UserName}.",
                    EventType.Warning,
                    targetDevice.OrganizationID);
                return Unauthorized();
            }
            else
            {
                SaveSucessful = success;
            }
            PopulateViewModel(deviceID);

            return Page();
        }

        public IActionResult OnPost(string deviceID)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewModel(deviceID);
                return Page();
            }

            DataService.UpdateDevice(deviceID, Input.Tags, Input.Alias, Input.DeviceGroupID, Input.Notes);

            return RedirectToPage("EditDevice", new { deviceID, success = true });
        }

        private void PopulateViewModel(string deviceID)
        {
            var user = DataService.GetUserByName(User.Identity.Name);
            if (user != null)
            {
                var device = DataService.GetDevice(user.OrganizationID, deviceID);
                DeviceName = device?.DeviceName;
                DeviceID = device?.ID;
                AgentVersion = device?.AgentVersion;
                Input.Alias = device?.Alias;
                Input.DeviceGroupID = device?.DeviceGroupID;
                Input.Tags = device?.Tags;
                Input.Notes = device?.Notes;

            }
            var groups = DataService.GetDeviceGroups(User.Identity.Name);
            DeviceGroups.AddRange(groups.Select(x => new SelectListItem(x.Name, x.ID)));
        }
        public class InputModel
        {

            [StringLength(100)]
            public string Alias { get; set; }

            public string DeviceGroupID { get; set; }

            [StringLength(200)]
            public string Tags { get; set; }

            public string Notes { get; set; }
        }
    }
}
