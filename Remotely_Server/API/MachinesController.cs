using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely_Library.Models;
using Remotely_Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely_Server.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MachinesController : Controller
    {
   
        public MachinesController(DataService dataService, UserManager<RemotelyUser> userManager)
        {
            this.DataService = dataService;
            this.UserManager = userManager;
        }
        private DataService DataService { get; set; }
        private UserManager<RemotelyUser> UserManager { get; set; }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Machine>> Get()
        {
            var user = await UserManager.GetUserAsync(User);
            var machines = DataService.GetAllMachines(user.Id);
            return machines;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Machine> Get(string id)
        {
            var user = await UserManager.GetUserAsync(User);
            var machines = DataService.GetAllMachines(user.Id);
            return machines.FirstOrDefault(x=>x.ID == id);
        }
    }
}
