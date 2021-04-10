using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSharingController : ControllerBase
    {
        private readonly IDataService _dataService;

        public FileSharingController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ExpiringTokenFilter))]
        public ActionResult Get(string id)
        {
            var sharedFile = _dataService.GetSharedFiled(id);
            if (sharedFile != null)
            {
                return File(sharedFile.FileContents, sharedFile.ContentType, sharedFile.FileName);
            }
            return NotFound();
        }

        [HttpPost]
        [ServiceFilter(typeof(ExpiringTokenFilter))]
        [RequestSizeLimit(AppConstants.MaxUploadFileSize)]
        public async Task<IEnumerable<string>> Post()
        {
            if (Request?.Form?.Files?.Count !> 0)
            {
                return Array.Empty<string>();
            }

            var fileIDs = new List<string>();
            foreach (var file in Request.Form.Files)
            {
                var orgID = User.Identity.IsAuthenticated ? _dataService.GetUserByNameWithOrg(User.Identity.Name).OrganizationID : null;
                var id = await _dataService.AddSharedFile(file, orgID);
                fileIDs.Add(id);
            }
            return fileIDs;
        }
    }
}
