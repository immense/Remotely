using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Services;
using Remotely.Shared.Dtos;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Tests;

[TestClass]
public class DataServiceTests
{
    private readonly string _newDeviceID = "NewDeviceName";
    private IDataService _dataService = null!;
    private TestData _testData = null!;

    [TestMethod]
    public async Task AddAlert()
    {
        await _dataService.AddAlert(_testData.Org1Device1.ID, _testData.Org1Id, "Test Message");

        var alerts = _dataService.GetAlerts(_testData.Org1Admin1.Id);

        Assert.AreEqual("Test Message", alerts.First().Message);
    }

    [TestMethod]
    public async Task AddOrUpdateDevice()
    {
        var storedDevice = (await _dataService.GetDevice(_newDeviceID)).Value;

        Assert.IsNull(storedDevice);

        var newDevice = new DeviceClientDto()
        {
            ID = _newDeviceID,
            OrganizationID = _testData.Org1Id,
            DeviceName = Environment.MachineName,
            Is64Bit = Environment.Is64BitOperatingSystem
        };

        var result = await _dataService.AddOrUpdateDevice(newDevice);
        Assert.IsTrue(result.IsSuccess);

        storedDevice = (await _dataService.GetDevice(_newDeviceID)).Value;

        Assert.AreEqual(_newDeviceID, storedDevice!.ID);
        Assert.AreEqual(Environment.MachineName, storedDevice.DeviceName);
        Assert.AreEqual(Environment.Is64BitOperatingSystem, storedDevice.Is64Bit);
    }

    [TestMethod]
    public async Task CreateDevice()
    {
        var deviceOptions = new DeviceSetupOptions()
        {
            DeviceID = Guid.NewGuid().ToString(),
            DeviceAlias = "Spare Laptop",
            OrganizationID = _testData.Org1Id
        };

        // First call should create and return device.
        var savedDevice = (await _dataService.CreateDevice(deviceOptions)).Value!;
        Assert.IsInstanceOfType(savedDevice, typeof(Device));

        // Second call with same DeviceUuid should fail.
        var secondSave = await _dataService.CreateDevice(deviceOptions);
        Assert.IsFalse(secondSave.IsSuccess);
    }

    [TestMethod]
    public void DeviceGroupPermissions()
    {
        Assert.AreEqual(2, _dataService.GetDevicesForUser(_testData.Org1Admin1.UserName!).Length);
        Assert.AreEqual(2, _dataService.GetDevicesForUser(_testData.Org1Admin2.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org1User1.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org1User2.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org2User1.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org2User2.UserName!).Length);

        Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1Admin1));
        Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1Admin2));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1User1));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1User2));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org2User1));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org2User2));

        var groupID = _testData.Org1Group1.ID;
        _dataService.AddUserToDeviceGroup(_testData.Org1Id, groupID, _testData.Org1User1.UserName!, out _);
        _testData.Org1Device1.DeviceGroupID = groupID;
        _dataService.UpdateDevice(_testData.Org1Device1.ID, "", "", groupID, "");

        Assert.AreEqual(2, _dataService.GetDevicesForUser(_testData.Org1Admin1.UserName!).Length);
        Assert.AreEqual(2, _dataService.GetDevicesForUser(_testData.Org1Admin2.UserName!).Length);
        Assert.AreEqual(1, _dataService.GetDevicesForUser(_testData.Org1User1.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org1User2.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org2User1.UserName!).Length);
        Assert.AreEqual(0, _dataService.GetDevicesForUser(_testData.Org2User2.UserName!).Length);

        Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1Admin1));
        Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1Admin2));
        Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1User1));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org1User2));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org2User1));
        Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Org1Device1.ID, _testData.Org2User2));

        var allDevices = _dataService.GetAllDevices(_testData.Org1Id).Select(x => x.ID).ToArray();
        Assert.AreEqual(2, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org1Admin1).Length);
        Assert.AreEqual(2, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org1Admin2).Length);
        Assert.AreEqual(1, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org1User1).Length);
        Assert.AreEqual(0, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org1User2).Length);
        Assert.AreEqual(0, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org2User1).Length);
        Assert.AreEqual(0, _dataService.FilterDeviceIdsByUserPermission(allDevices, _testData.Org2User2).Length);
    }

    [TestMethod]
    public async Task GetPendingScriptRuns_GivenMultipleRunsQueued_ReturnsOnlyLatest()
    {
        var now = Time.Now;

        var savedScript = new SavedScript()
        {
            Content = "Get-ChildItem",
            Creator = _testData.Org1Admin1,
            CreatorId = _testData.Org1Admin1.Id,
            Name = "GCI",
            Organization = _testData.Org1Admin1.Organization,
            OrganizationID = _testData.Org1Id,
            Shell = ScriptingShell.PSCore
        };

        await _dataService.AddOrUpdateSavedScript(savedScript, _testData.Org1Admin1.Id);

        var scriptRun = new ScriptRun()
        {
            Devices = new() { _testData.Org1Device1 },
            InputType = ScriptInputType.ScheduledScript,
            SavedScriptId = savedScript.Id,
            Initiator = _testData.Org1Admin1.UserName,
            RunAt = now,
            OrganizationID = _testData.Org1Id,
            Organization = _testData.Org1Admin1.Organization,
            RunOnNextConnect = true
        };

        await _dataService.AddScriptRun(scriptRun);

        scriptRun.Id = 0;
        scriptRun.RunAt = now.AddMinutes(1);

        await _dataService.AddScriptRun(scriptRun);

        Time.Adjust(TimeSpan.FromMinutes(2));

        var pendingRuns = await _dataService.GetPendingScriptRuns(_testData.Org1Device1.ID);

        Assert.AreEqual(1, pendingRuns.Count());
        Assert.AreEqual(2, pendingRuns.First().Id);

        var dto = new ScriptResultDto()
        {
            DeviceID = _testData.Org1Device1.ID,
            InputType = ScriptInputType.ScheduledScript,
            SavedScriptId = savedScript.Id,
            ScriptRunId = scriptRun.Id,
            Shell = ScriptingShell.PSCore,
            ScriptInput = "echo test"
        };

        var scriptResult = (await _dataService.AddScriptResult(dto)).Value!;

        await _dataService.AddScriptResultToScriptRun(scriptResult.ID, scriptRun.Id);

        pendingRuns = await _dataService.GetPendingScriptRuns(_testData.Org1Device1.ID);

        Assert.AreEqual(0, pendingRuns.Count());
    }


    [TestCleanup]
    public void TestCleanup()
    {
        _testData.ClearData();
    }

    [TestInitialize]
    public async Task TestInit()
    {
        _testData = new TestData();
        await _testData.Init();
        _dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
    }

    [TestMethod]
    public async Task UpdateOrganizationName()
    {
        Assert.AreEqual("Org1", _testData.Org1Admin1.Organization!.OrganizationName);
        await _dataService.UpdateOrganizationName(_testData.Org1Id, "Test Org");
        var updatedOrg = (await _dataService.GetOrganizationById(_testData.Org1Id)).Value;
        Assert.AreEqual("Test Org", updatedOrg!.OrganizationName);
    }

    [TestMethod]
    public async Task UpdateServerAdmins()
    {
        var currentAdmins = _dataService.GetServerAdmins();

        Assert.AreEqual(1, currentAdmins.Count);

        await _dataService.SetIsServerAdmin(_testData.Org1Admin2.Id, true, _testData.Org1Admin1.Id);

        currentAdmins = _dataService.GetServerAdmins();
        Assert.AreEqual(2, currentAdmins.Count);
        Assert.IsTrue(currentAdmins.Contains(_testData.Org1Admin1.UserName!));
        Assert.IsTrue(currentAdmins.Contains(_testData.Org1Admin2.UserName!));

        // Shouldn't be able to change themselves.
        await _dataService.SetIsServerAdmin(_testData.Org1Admin2.Id, false, _testData.Org1Admin2.Id);
        currentAdmins = _dataService.GetServerAdmins();
        Assert.AreEqual(2, currentAdmins.Count);

        // Non-admins shouldn't be able to change admins.
        await _dataService.SetIsServerAdmin(_testData.Org1User1.Id, false, _testData.Org1Admin2.Id);
        currentAdmins = _dataService.GetServerAdmins();
        Assert.AreEqual(2, currentAdmins.Count);

        await _dataService.SetIsServerAdmin(_testData.Org1Admin2.Id, false, _testData.Org1Admin1.Id);
        currentAdmins = _dataService.GetServerAdmins();
        Assert.AreEqual(1, currentAdmins.Count);
        Assert.AreEqual(_testData.Org1Admin1.UserName, currentAdmins[0]);
    }

    [TestMethod]
    public async Task VerifyInitialData()
    {
        Assert.IsNotNull((await _dataService.GetUserByName(_testData.Org1Admin1.UserName!)).Value);
        Assert.IsNotNull((await _dataService.GetUserByName(_testData.Org1Admin2.UserName!)).Value);
        Assert.IsNotNull((await _dataService.GetUserByName(_testData.Org1User1.UserName!)).Value);
        Assert.IsNotNull((await _dataService.GetUserByName(_testData.Org1User2.UserName!)).Value);
        Assert.AreEqual(2, _dataService.GetOrganizationCount());

        var devices1 = _dataService.GetAllDevices(_testData.Org1Id);
        Assert.AreEqual(2, devices1.Length);
        Assert.IsTrue(devices1.Any(x => x.ID == "Org1Device1"));
        Assert.IsTrue(devices1.Any(x => x.ID == "Org1Device2"));

        var devices2 = _dataService.GetAllDevices(_testData.Org2Id);
        Assert.AreEqual(2, devices2.Length);
        Assert.IsTrue(devices2.Any(x => x.ID == "Org2Device1"));
        Assert.IsTrue(devices2.Any(x => x.ID == "Org2Device2"));

        var org1Ids = new string[]
        {
            _testData.Org1Group1.OrganizationID,
            _testData.Org1Group2.OrganizationID,
            _testData.Org1Admin1.OrganizationID,
            _testData.Org1Admin2.OrganizationID,
            _testData.Org1User1.OrganizationID,
            _testData.Org1User2.OrganizationID,
            _testData.Org1Device1.OrganizationID,
            _testData.Org1Device2.OrganizationID
        };

        Assert.IsTrue(org1Ids.All(x => x == _testData.Org1Id));

        var org2Ids = new string[]
{
            _testData.Org2Group1.OrganizationID,
            _testData.Org2Group2.OrganizationID,
            _testData.Org2Admin1.OrganizationID,
            _testData.Org2Admin2.OrganizationID,
            _testData.Org2User1.OrganizationID,
            _testData.Org2User2.OrganizationID,
            _testData.Org2Device1.OrganizationID,
            _testData.Org2Device2.OrganizationID
};

        Assert.IsTrue(org2Ids.All(x => x == _testData.Org2Id));
    }
}
