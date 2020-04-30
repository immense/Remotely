using System.Collections.Generic;
using Remotely.Shared.Models;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
   
        public DevicesController(DataService dataService, UserManager<RemotelyUser> userManager)
        {
            this.DataService = dataService;
            this.UserManager = userManager;
        }
        private DataService DataService { get; set; }
        private UserManager<RemotelyUser> UserManager { get; set; }

        
        [HttpGet]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public IEnumerable<Device> Get()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (User.Identity.IsAuthenticated)
            {
                return DataService.GetDevicesForUser(User.Identity.Name);
            }

            return DataService.GetAllDevices(orgID);
        }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("{id}")]
        public Device Get(string id)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var device = DataService.GetDevice(orgID, id);

            if (User.Identity.IsAuthenticated &&
                !DataService.DoesUserHaveAccessToDevice(id, DataService.GetUserByName(User.Identity.Name)))
            {
                return null;
            }
            return device;
        }
    }
}
