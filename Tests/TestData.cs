using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Server.Areas.Identity.Pages.Account.Manage;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    public class TestData
    {
        public static RemotelyUser Admin1 { get; } = new RemotelyUser()
        {
            UserName = "admin1@test.com",
            IsAdministrator = true,
            IsServerAdmin = true,
            Organization = new Organization(),
            UserOptions = new RemotelyUserOptions()
        };

        public static RemotelyUser Admin2 { get; private set; } = new RemotelyUser()
        {
            UserName = "admin2@test.com",
            IsAdministrator = true,
            Organization = new Organization(),
            UserOptions = new RemotelyUserOptions()
        };

        public static RemotelyUser User1 { get; private set; } = new RemotelyUser()
        {
            UserName = "testuser1@test.com",
            IsAdministrator = false,
            Organization = new Organization(),
            UserOptions = new RemotelyUserOptions()
        };

        public static RemotelyUser User2 { get; private set; } = new RemotelyUser()
        {
            UserName = "testuser2@test.com",
            IsAdministrator = false,
            Organization = new Organization(),
            UserOptions = new RemotelyUserOptions()
        };

        public static DeviceGroup Group1 { get; private set; } = new DeviceGroup()
        {
            Name = "Group1"
        };

        public static DeviceGroup Group2 { get; private set; } = new DeviceGroup()
        {
            Name = "Group2"
        };

        public static Device Device1 { get; private set; } = new Device()
        {
            ID = "Device1",
            DeviceName = "Device1Name"
        };

        public static Device Device2 { get; private set; } = new Device()
        {
            ID = "Device2",
            DeviceName = "Device2Name"
        };

        public static string OrganizationID { get; private set; }

        public static void ClearData()
        {
            var dbContext = IoCActivator.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Devices.RemoveRange(dbContext.Devices.ToList());
            dbContext.DeviceGroups.RemoveRange(dbContext.DeviceGroups.ToList());
            dbContext.Users.RemoveRange(dbContext.Users.ToList());
            dbContext.SaveChanges();

        }

        public static async Task PopulateTestData()
        {
            var dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
            var userManager = IoCActivator.ServiceProvider.GetRequiredService<UserManager<RemotelyUser>>();
            var emailSender = IoCActivator.ServiceProvider.GetRequiredService<IEmailSenderEx>();
            var organizationModel = new OrganizationModel(dataService, userManager, emailSender);


            await userManager.CreateAsync(Admin1);

            organizationModel.Input.UserEmail = Admin2.UserName;
            organizationModel.Input.IsAdmin = true;
            await organizationModel.AddUser(Admin1);
            Admin2 = await userManager.FindByNameAsync(Admin2.UserName);

            organizationModel.Input.UserEmail = User1.UserName;
            organizationModel.Input.IsAdmin = false;
            await organizationModel.AddUser(Admin1);
            User1 = await userManager.FindByNameAsync(User1.UserName);

            organizationModel.Input.UserEmail = User2.UserName;
            organizationModel.Input.IsAdmin = false;
            await organizationModel.AddUser(Admin1);
            User2 = await userManager.FindByNameAsync(User2.UserName);

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
