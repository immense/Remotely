using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely.Server.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public ErrorModel(DataService dataService)
        {
            this.DataService = dataService;
        }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private DataService DataService { get; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            try
            {
                var user = DataService.GetUserByName(User.Identity.Name);
                var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
                if (feature?.Error != null)
                {
                    var error = feature.Error;
                    while (error != null)
                    {
                        var logEntry = new Remotely.Shared.Models.EventLog()
                        {
                            EventType = Remotely.Shared.Models.EventTypes.Error,
                            Message = error.Message,
                            Source = error.Source,
                            StackTrace = error.StackTrace,
                            OrganizationID = user?.OrganizationID
                        };
                        DataService.WriteEvent(logEntry);
                        error = error.InnerException;
                    }
                }
            }
            catch { }
        }
    }
}
