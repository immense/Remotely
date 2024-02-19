using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Dtos;
using Remotely.Shared.Entities;
using Remotely.Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Tests;

public class TestData
{
    #region Organization1
    public Organization Org1 => Org1Admin1.Organization!;

    public RemotelyUser Org1Admin1 { get; } = new()
    {
        UserName = "org1admin1@test.com",
        IsAdministrator = true,
        IsServerAdmin = true,
        Organization = new Organization() { OrganizationName = "Org1" },
        UserOptions = new RemotelyUserOptions()
    };

    public RemotelyUser Org1Admin2 { get; private set; } = null!;

    public Device Org1Device1 { get; private set; } = null!;

    public Device Org1Device2 { get; private set; } = null!;

    public DeviceGroup Org1Group1 { get; private set; } = new DeviceGroup()
    {
        Name = "Org1Group1"
    };

    public DeviceGroup Org1Group2 { get; private set; } = new DeviceGroup()
    {
        Name = "Org1Group2"
    };

    public string Org1Id => Org1.ID;
    public RemotelyUser Org1User1 { get; private set; } = null!;
    public RemotelyUser Org1User2 { get; private set; } = null!;
    #endregion



    #region Organization2
    public Organization Org2 => Org2Admin1.Organization!;

    public RemotelyUser Org2Admin1 { get; } = new()
    {
        UserName = "org2admin1@test.com",
        IsAdministrator = true,
        IsServerAdmin = false,
        Organization = new Organization() { OrganizationName = "Org2" },
        UserOptions = new RemotelyUserOptions()
    };

    public RemotelyUser Org2Admin2 { get; private set; } = null!;

    public Device Org2Device1 { get; private set; } = null!;

    public Device Org2Device2 { get; private set; } = null!;

    public DeviceGroup Org2Group1 { get; private set; } = new DeviceGroup()
    {
        Name = "Org2Group1"
    };

    public DeviceGroup Org2Group2 { get; private set; } = new DeviceGroup()
    {
        Name = "Org2Group2"
    };

    public string Org2Id => Org2.ID;
    public RemotelyUser Org2User1 { get; private set; } = null!;
    public RemotelyUser Org2User2 { get; private set; } = null!;
    #endregion

    public void ClearData()
    {
        using var scope = IoCActivator.ServiceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDb>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

    }

    public async Task Init()
    {
        ClearData();

        using var scope = IoCActivator.ServiceProvider.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RemotelyUser>>();
        var dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
        var emailSender = IoCActivator.ServiceProvider.GetRequiredService<IEmailSenderEx>();

        // Organization 1
        await userManager.CreateAsync(Org1Admin1);

        await dataService.CreateUser("org1admin2@test.com", true, Org1Admin1.OrganizationID);
        Org1Admin2 = (await dataService.GetUserByName("org1admin2@test.com")).Value!;

        await dataService.CreateUser("org1testuser1@test.com", false, Org1Admin1.OrganizationID);
        Org1User1 = (await dataService.GetUserByName("org1testuser1@test.com")).Value!;

        await dataService.CreateUser("org1testuser2@test.com", false, Org1Admin1.OrganizationID);
        Org1User2 = (await dataService.GetUserByName("org1testuser2@test.com")).Value!;

        var device1 = new DeviceClientDto()
        {
            ID = "Org1Device1",
            DeviceName = "Org1Device1Name",
            OrganizationID = Org1Id
        };
        var device2 = new DeviceClientDto()
        {
            ID = "Org1Device2",
            DeviceName = "Org1Device2Name",
            OrganizationID = Org1Id
        };
        Org1Device1 = (await dataService.AddOrUpdateDevice(device1)).Value!;
        Org1Device2 = (await dataService.AddOrUpdateDevice(device2)).Value!;

        await dataService.AddDeviceGroup(Org1Admin1.OrganizationID, Org1Group1);
        await dataService.AddDeviceGroup(Org1Admin1.OrganizationID, Org1Group2);
        var deviceGroups1 = dataService.GetDeviceGroups(Org1Admin1.UserName!);
        Org1Group1 = deviceGroups1.First(x => x.Name == Org1Group1.Name);
        Org1Group2 = deviceGroups1.First(x => x.Name == Org1Group2.Name);


        // Organization 2
        await userManager.CreateAsync(Org2Admin1);

        await dataService.CreateUser("org2admin2@test.com", true, Org2Admin1.OrganizationID);
        Org2Admin2 = (await dataService.GetUserByName("org2admin2@test.com")).Value!;

        await dataService.CreateUser("org2testuser1@test.com", false, Org2Admin1.OrganizationID);
        Org2User1 = (await dataService.GetUserByName("org2testuser1@test.com")).Value!;

        await dataService.CreateUser("org2testuser2@test.com", false, Org2Admin1.OrganizationID);
        Org2User2 = (await dataService.GetUserByName("org2testuser2@test.com")).Value!;

        var device3 = new DeviceClientDto()
        {
            ID = "Org2Device1",
            DeviceName = "Org2Device1Name",
            OrganizationID = Org2Id
        };
        var device4 = new DeviceClientDto()
        {
            ID = "Org2Device2",
            DeviceName = "Org2Device2Name",
            OrganizationID = Org2Id
        };
        Org2Device1 = (await dataService.AddOrUpdateDevice(device3)).Value!;
        Org2Device2 = (await dataService.AddOrUpdateDevice(device4)).Value!;

        await dataService.AddDeviceGroup(Org2Admin1.OrganizationID, Org2Group1);
        await dataService.AddDeviceGroup(Org2Admin1.OrganizationID, Org2Group2);
        var deviceGroups2 = dataService.GetDeviceGroups(Org2Admin1.UserName!);
        Org2Group1 = deviceGroups2.First(x => x.Name == Org2Group1.Name);
        Org2Group2 = deviceGroups2.First(x => x.Name == Org2Group2.Name);
    }
}
