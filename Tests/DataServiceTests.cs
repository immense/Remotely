using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private IDataService DataService { get; set; }

        [TestMethod]
        [DoNotParallelize]
        public async Task AddAlert()
        {
            var options = new AlertOptions()
            {
                AlertDeviceID = TestData.Device1.ID,
                AlertMessage = "Test Message",
                ShouldAlert = true
            };
            await DataService.AddAlert(options, TestData.OrganizationID);

            var alerts = DataService.GetAlerts(TestData.Admin1.Id);

            Assert.AreEqual("Test Message", alerts.First().Message);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task AddOrUpdateDevice()
        {
            var newDeviceID = "NewDeviceName";
            var storedDevice = DataService.GetDevice(newDeviceID);

            Assert.IsNull(storedDevice);

            var newDevice = await DeviceInformation.Create(newDeviceID, TestData.OrganizationID);
            Assert.IsTrue(DataService.AddOrUpdateDevice(newDevice, out _));

            storedDevice = DataService.GetDevice(newDeviceID);

            Assert.AreEqual(newDeviceID, storedDevice.ID);
            Assert.AreEqual(Environment.MachineName, storedDevice.DeviceName);
            Assert.AreEqual(Environment.Is64BitOperatingSystem, storedDevice.Is64Bit);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task CreateDevice()
        {
            var deviceOptions = new DeviceSetupOptions()
            {
                DeviceID = Guid.NewGuid().ToString(),
                DeviceAlias = "Spare Laptop",
                OrganizationID = TestData.OrganizationID
            };

            // First call should create and return device.
            var savedDevice = await DataService.CreateDevice(deviceOptions);
            Assert.IsInstanceOfType(savedDevice, typeof(Device));

            // Second call with same DeviceUuid should return null;
            var secondSave = await DataService.CreateDevice(deviceOptions);
            Assert.IsNull(secondSave);
        }

        [TestMethod]
        [DoNotParallelize]
        public void DeviceGroupPermissions()
        {
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin2.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User2.UserName).Count() == 2);

            var groupID = DataService.GetDeviceGroups(TestData.Admin1.UserName).First().ID;

            DataService.UpdateDevice(TestData.Device1.ID, "", "", groupID, "");
            DataService.AddUserToDeviceGroup(TestData.OrganizationID, groupID, TestData.User1.UserName, out _);

            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin2.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User2.UserName).Count() == 1);

            Assert.IsTrue(DataService.DoesUserHaveAccessToDevice(TestData.Device1.ID, TestData.Admin1));
            Assert.IsTrue(DataService.DoesUserHaveAccessToDevice(TestData.Device1.ID, TestData.Admin2));
            Assert.IsTrue(DataService.DoesUserHaveAccessToDevice(TestData.Device1.ID, TestData.User1));
            Assert.IsFalse(DataService.DoesUserHaveAccessToDevice(TestData.Device1.ID, TestData.User2));

            var allDevices = DataService.GetAllDevices(TestData.OrganizationID).Select(x => x.ID).ToArray();

            Assert.AreEqual(2, DataService.FilterDeviceIDsByUserPermission(allDevices, TestData.Admin1).Count());
            Assert.AreEqual(2, DataService.FilterDeviceIDsByUserPermission(allDevices, TestData.Admin2).Count());
            Assert.AreEqual(2, DataService.FilterDeviceIDsByUserPermission(allDevices, TestData.User1).Count());
            Assert.AreEqual(1, DataService.FilterDeviceIDsByUserPermission(allDevices, TestData.User2).Count());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestData.ClearData();
        }

        [TestInitialize]
        public async Task TestInit()
        {
            await TestData.PopulateTestData();
            DataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
        }
        [TestMethod]
        [DoNotParallelize]
        public void UpdateOrganizationName()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(TestData.Admin1.Organization.OrganizationName));
            DataService.UpdateOrganizationName(TestData.OrganizationID, "Test Org");
            Assert.AreEqual(TestData.Admin1.Organization.OrganizationName, "Test Org");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateServerAdmins()
        {
            var currentAdmins = DataService.GetServerAdmins();
            Assert.AreEqual(1, currentAdmins.Count);
            var newAdmins = new List<string>()
            {
                TestData.Admin2.UserName
            };

            await DataService.UpdateServerAdmins(newAdmins, TestData.Admin1.UserName);

            currentAdmins = DataService.GetServerAdmins();
            Assert.AreEqual(2, currentAdmins.Count);
            Assert.IsTrue(currentAdmins.Contains(TestData.Admin1.UserName));
            Assert.IsTrue(currentAdmins.Contains(TestData.Admin2.UserName));

            await DataService.UpdateServerAdmins(newAdmins, TestData.Admin2.UserName);
            currentAdmins = DataService.GetServerAdmins();
            Assert.AreEqual(1, currentAdmins.Count);
            Assert.AreEqual(TestData.Admin2.UserName, currentAdmins[0]);
        }

        [TestMethod]
        [DoNotParallelize]
        public void VerifyInitialData()
        {
            Assert.IsNotNull(DataService.GetUserByName(TestData.Admin1.UserName));
            Assert.IsNotNull(DataService.GetUserByName(TestData.Admin2.UserName));
            Assert.IsNotNull(DataService.GetUserByName(TestData.User1.UserName));
            Assert.IsNotNull(DataService.GetUserByName(TestData.User2.UserName));
            Assert.AreEqual(1, DataService.GetOrganizationCount());

            var devices = DataService.GetAllDevices(TestData.OrganizationID);

            Assert.AreEqual(2, devices.Count());
            Assert.IsTrue(devices.Any(x => x.ID == "Device1"));
            Assert.IsTrue(devices.Any(x => x.ID == "Device2"));

            var orgIDs = new string[]
            {
                TestData.Group1.OrganizationID,
                TestData.Group2.OrganizationID,
                TestData.Admin1.OrganizationID,
                TestData.Admin2.OrganizationID,
                TestData.User1.OrganizationID,
                TestData.User2.OrganizationID,
                TestData.Device1.OrganizationID,
                TestData.Device2.OrganizationID
            };

            Assert.IsTrue(orgIDs.All(x => x == TestData.OrganizationID));
        }
    }
}
