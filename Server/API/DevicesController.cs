using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Shared.Models;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Authorize]
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

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Device>> Get()
        {
            var user = await UserManager.GetUserAsync(User);
            var devices = DataService.GetAllDevicesForUser(user.Id);
            return devices;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Device> Get(string id)
        {
            var user = await UserManager.GetUserAsync(User);
            return DataService.GetDeviceForUser(user.Id, id);
        }
    }
}
