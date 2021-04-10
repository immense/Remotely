using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remotely.Server.Areas.Identity.Pages.Account.Manage;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    public class TestData
    {
        public TestData()
        {
            Init().Wait();
        }

        public RemotelyUser Admin1 { get; } = new RemotelyUser()
        {
            UserName = "admin1@test.com",
            IsAdministrator = true,
            IsServerAdmin = true,
            Organization = new Organization(),
            UserOptions = new RemotelyUserOptions()
        };

        public RemotelyUser Admin2 { get; private set; } 

        public Device Device1 { get; private set; } = new Device()
        {
            ID = "Device1",
            DeviceName = "Device1Name"
        };

        public Device Device2 { get; private set; } = new Device()
        {
            ID = "Device2",
            DeviceName = "Device2Name"
        };

        public DeviceGroup Group1 { get; private set; } = new DeviceGroup()
        {
            Name = "Group1"
        };

        public DeviceGroup Group2 { get; private set; } = new DeviceGroup()
        {
            Name = "Group2"
        };

        public string OrganizationID { get; private set; }

        public RemotelyUser User1 { get; private set; }

        public RemotelyUser User2 { get; private set; }

        public void ClearData()
        {
            var dbContext = IoCActivator.ServiceProvider.GetRequiredService<AppDb>();
            dbContext.Devices.RemoveRange(dbContext.Devices.ToList());
            dbContext.DeviceGroups.RemoveRange(dbContext.DeviceGroups.ToList());
            dbContext.Users.RemoveRange(dbContext.Users.ToList());
            dbContext.Organizations.RemoveRange(dbContext.Organizations.ToList());
            dbContext.SaveChanges();

        }

        private async Task Init()
        {
            ClearData();

            var dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
            var userManager = IoCActivator.ServiceProvider.GetRequiredService<UserManager<RemotelyUser>>();
            var emailSender = IoCActivator.ServiceProvider.GetRequiredService<IEmailSenderEx>();

            await userManager.CreateAsync(Admin1);

            await dataService.CreateUser("admin2@test.com", true, Admin1.OrganizationID);
            Admin2 = dataService.GetUserByNameWithOrg("admin2@test.com");

            await dataService.CreateUser("testuser1@test.com", false, Admin1.OrganizationID);
            User1 = dataService.GetUserByNameWithOrg("testuser1@test.com");

            await dataService.CreateUser("testuser2@test.com", false, Admin1.OrganizationID);
            User2 = dataService.GetUserByNameWithOrg("testuser2@test.com");

            Device1.OrganizationID = Admin1.OrganizationID;
            dataService.AddOrUpdateDevice(Device1, out _);
            Device2.OrganizationID = Admin1.OrganizationID;
            dataService.AddOrUpdateDevice(Device2, out _);

            dataService.AddDeviceGroup(Admin1.OrganizationID, Group1, out _, out _);
            dataService.AddDeviceGroup(Admin1.OrganizationID, Group2, out _, out _);
            var deviceGroups = dataService.GetDeviceGroups(Admin1.UserName);
            Group1 = deviceGroups.First(x => x.Name == Group1.Name);
            Group2 = deviceGroups.First(x => x.Name == Group2.Name);

            OrganizationID = Admin1.OrganizationID;
        }
    }
}
