using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSharingController : ControllerBase
    {
        public FileSharingController(IDataService dataService)
        {
            DataService = dataService;
        }
        public IDataService DataService { get; set; }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var sharedFile = DataService.GetSharedFiled(id);
            // Shared files expire after a minute and become locked.
            if (sharedFile != null && sharedFile.Timestamp.AddMinutes(1) > DateTimeOffset.Now)
            {
                return File(sharedFile.FileContents, sharedFile.ContentType, sharedFile.FileName);
            }
            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public async Task<List<string>> Post()
        {
            var fileIDs = new List<string>();
            foreach (var file in Request.Form.Files)
            {
                var orgID = User.Identity.IsAuthenticated ? DataService.GetUserByName(User.Identity.Name).OrganizationID : null;
                var id = await DataService.AddSharedFile(file, orgID);
                fileIDs.Add(id);
            }
            return fileIDs;
        }
    }
}
