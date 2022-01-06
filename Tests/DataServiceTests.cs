using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Agent.Interfaces;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private IDataService _dataService;
        private Mock<IDeviceInformationService> _deviceInfo;
        private TestData _testData;
        private string _newDeviceID = "NewDeviceName";

        [TestMethod]
        public async Task AddAlert()
        {
            await _dataService.AddAlert(_testData.Device1.ID, _testData.OrganizationID, "Test Message");

            var alerts = _dataService.GetAlerts(_testData.Admin1.Id);

            Assert.AreEqual("Test Message", alerts.First().Message);
        }

        [TestMethod]
        public async Task AddOrUpdateDevice()
        {
            var storedDevice = _dataService.GetDevice(_newDeviceID);

            Assert.IsNull(storedDevice);

            var newDevice = await _deviceInfo.Object.CreateDevice(_newDeviceID, _testData.OrganizationID);
            Assert.IsTrue(_dataService.AddOrUpdateDevice(newDevice, out _));

            storedDevice = _dataService.GetDevice(_newDeviceID);

            Assert.AreEqual(_newDeviceID, storedDevice.ID);
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
                OrganizationID = _testData.OrganizationID
            };

            // First call should create and return device.
            var savedDevice = await _dataService.CreateDevice(deviceOptions);
            Assert.IsInstanceOfType(savedDevice, typeof(Device));

            // Second call with same DeviceUuid should return null;
            var secondSave = await _dataService.CreateDevice(deviceOptions);
            Assert.IsNull(secondSave);
        }

        [TestMethod]
        public void DeviceGroupPermissions()
        {
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.Admin1.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.Admin2.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.User1.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.User2.UserName).Count() == 2);

            var groupID = _dataService.GetDeviceGroups(_testData.Admin1.UserName).First().ID;

            _dataService.UpdateDevice(_testData.Device1.ID, "", "", groupID, "", Shared.Enums.WebRtcSetting.Default);
            _dataService.AddUserToDeviceGroup(_testData.OrganizationID, groupID, _testData.User1.UserName, out _);

            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.Admin1.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.Admin2.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.User1.UserName).Count() == 2);
            Assert.IsTrue(_dataService.GetDevicesForUser(_testData.User2.UserName).Count() == 1);

            Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Device1.ID, _testData.Admin1));
            Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Device1.ID, _testData.Admin2));
            Assert.IsTrue(_dataService.DoesUserHaveAccessToDevice(_testData.Device1.ID, _testData.User1));
            Assert.IsFalse(_dataService.DoesUserHaveAccessToDevice(_testData.Device1.ID, _testData.User2));

            var allDevices = _dataService.GetAllDevices(_testData.OrganizationID).Select(x => x.ID).ToArray();

            Assert.AreEqual(2, _dataService.FilterDeviceIDsByUserPermission(allDevices, _testData.Admin1).Length);
            Assert.AreEqual(2, _dataService.FilterDeviceIDsByUserPermission(allDevices, _testData.Admin2).Length);
            Assert.AreEqual(2, _dataService.FilterDeviceIDsByUserPermission(allDevices, _testData.User1).Length);
            Assert.AreEqual(1, _dataService.FilterDeviceIDsByUserPermission(allDevices, _testData.User2).Length);
        }

        [TestMethod]
        public async Task GetPendingScriptRuns_GivenMultipleRunsQueued_ReturnsOnlyLatest()
        {
            var now = Time.Now;

            var savedScript = new SavedScript()
            {
                Content = "Get-ChildItem",
                Creator = _testData.Admin1,
                CreatorId = _testData.Admin1.Id,
                Name = "GCI",
                Organization = _testData.Admin1.Organization,
                OrganizationID = _testData.OrganizationID,
                Shell = Shared.Enums.ScriptingShell.PSCore
            };

            await _dataService.AddOrUpdateSavedScript(savedScript, _testData.Admin1.Id);

            var scriptRun = new ScriptRun()
            {
                Devices = new() { _testData.Device1 },
                InputType = Shared.Enums.ScriptInputType.ScheduledScript,
                SavedScriptId = savedScript.Id,
                Initiator = _testData.Admin1.UserName,
                RunAt = now,
                OrganizationID = _testData.OrganizationID,
                Organization = _testData.Admin1.Organization,
                RunOnNextConnect = true
            };

            await _dataService.AddScriptRun(scriptRun);

            scriptRun.Id = 0;
            scriptRun.RunAt = now.AddMinutes(1);

            await _dataService.AddScriptRun(scriptRun);

            Time.Adjust(TimeSpan.FromMinutes(2));

            var pendingRuns = await _dataService.GetPendingScriptRuns(_testData.Device1.ID);

            Assert.AreEqual(1, pendingRuns.Count);
            Assert.AreEqual(2, pendingRuns[0].Id);

            var scriptResult = new ScriptResult()
            {
                DeviceID = _testData.Device1.ID,
                InputType = Shared.Enums.ScriptInputType.ScheduledScript,
                OrganizationID = _testData.OrganizationID,
                SavedScriptId = savedScript.Id,
                ScriptRunId = scriptRun.Id,
                Shell = Shared.Enums.ScriptingShell.PSCore
            };

            _dataService.AddOrUpdateScriptResult(scriptResult);

            await _dataService.AddScriptResultToScriptRun(scriptResult.ID, scriptRun.Id);

            pendingRuns = await _dataService.GetPendingScriptRuns(_testData.Device1.ID);

            Assert.AreEqual(0, pendingRuns.Count);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            _testData.ClearData();
        }

        [TestInitialize]
        public void TestInit()
        {
            _testData = new TestData();
            _dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();

            var newDevice = new Device()
            {
                ID = _newDeviceID,
                DeviceName = Environment.MachineName,
                Is64Bit = Environment.Is64BitOperatingSystem,
                OrganizationID = _testData.OrganizationID
            };

            _deviceInfo = new Mock<IDeviceInformationService>();
            _deviceInfo
                .Setup(x => x.CreateDevice(_newDeviceID, _testData.OrganizationID))
                .Returns(Task.FromResult(newDevice));
        }

        [TestMethod]
        public void UpdateOrganizationName()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(_testData.Admin1.Organization.OrganizationName));
            _dataService.UpdateOrganizationName(_testData.OrganizationID, "Test Org");
            var updatedOrg = _dataService.GetOrganizationById(_testData.OrganizationID);
            Assert.AreEqual("Test Org", updatedOrg.OrganizationName);
        }

        [TestMethod]
        public async Task UpdateServerAdmins()
        {
            var currentAdmins = _dataService.GetServerAdmins();

            Assert.AreEqual(1, currentAdmins.Count);

            await _dataService.SetIsServerAdmin(_testData.Admin2.Id, true, _testData.Admin1.Id);

            currentAdmins = _dataService.GetServerAdmins();
            Assert.AreEqual(2, currentAdmins.Count);
            Assert.IsTrue(currentAdmins.Contains(_testData.Admin1.UserName));
            Assert.IsTrue(currentAdmins.Contains(_testData.Admin2.UserName));

            // Shouldn't be able to change themselves.
            await _dataService.SetIsServerAdmin(_testData.Admin2.Id, false, _testData.Admin2.Id);
            currentAdmins = _dataService.GetServerAdmins();
            Assert.AreEqual(2, currentAdmins.Count);

            // Non-admins shouldn't be able to change admins.
            await _dataService.SetIsServerAdmin(_testData.User1.Id, false, _testData.Admin2.Id);
            currentAdmins = _dataService.GetServerAdmins();
            Assert.AreEqual(2, currentAdmins.Count);

            await _dataService.SetIsServerAdmin(_testData.Admin2.Id, false, _testData.Admin1.Id);
            currentAdmins = _dataService.GetServerAdmins();
            Assert.AreEqual(1, currentAdmins.Count);
            Assert.AreEqual(_testData.Admin1.UserName, currentAdmins[0]);
        }

        [TestMethod]
        public void VerifyInitialData()
        {
            Assert.IsNotNull(_dataService.GetUserByNameWithOrg(_testData.Admin1.UserName));
            Assert.IsNotNull(_dataService.GetUserByNameWithOrg(_testData.Admin2.UserName));
            Assert.IsNotNull(_dataService.GetUserByNameWithOrg(_testData.User1.UserName));
            Assert.IsNotNull(_dataService.GetUserByNameWithOrg(_testData.User2.UserName));
            Assert.AreEqual(1, _dataService.GetOrganizationCount());

            var devices = _dataService.GetAllDevices(_testData.OrganizationID);

            Assert.AreEqual(2, devices.Count());
            Assert.IsTrue(devices.Any(x => x.ID == "Device1"));
            Assert.IsTrue(devices.Any(x => x.ID == "Device2"));

            var orgIDs = new string[]
            {
                _testData.Group1.OrganizationID,
                _testData.Group2.OrganizationID,
                _testData.Admin1.OrganizationID,
                _testData.Admin2.OrganizationID,
                _testData.User1.OrganizationID,
                _testData.User2.OrganizationID,
                _testData.Device1.OrganizationID,
                _testData.Device2.OrganizationID
            };

            Assert.IsTrue(orgIDs.All(x => x == _testData.OrganizationID));
        }
    }
}
