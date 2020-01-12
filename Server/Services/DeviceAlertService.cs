using Remotely.Server.Data;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class DeviceAlertService
    {
        public DeviceAlertService(DataService dataService, ApplicationConfig appConfig)
        {
            DataService = dataService;
            AppConfig = appConfig;
        }

        public DataService DataService { get; }
        public ApplicationConfig AppConfig { get; }

        public bool ShouldSendAlert(Device device, out DeviceAlert alert)
        {
            if (true) // Check
            {
                //DataService.AddDeviceAlert(alert);
                //TrySendAlertEmail(alert);
                //TrySendAlertWebRequest(alert);
                alert = new DeviceAlert();
                return true;
            }
            else
            {
                alert = null;
                return false;
            }
        }
    }
}
