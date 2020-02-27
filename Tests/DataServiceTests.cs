using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private DataService DataService { get; set; }

        [TestInitialize]
        public async Task TestInit()
        {
            await TestData.PopulateTestData();
            DataService = IoCActivator.ServiceProvider.GetRequiredService<DataService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestData.ClearData();
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

            var devices = DataService.GetAllDevices(TestData.Admin1.OrganizationID);

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

            Assert.IsTrue(orgIDs.All(x => x == TestData.Admin1.OrganizationID));
        }


        [TestMethod]
        [DoNotParallelize]
        public void UpdateOrganizationName()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(TestData.Admin1.Organization.OrganizationName));
            DataService.UpdateOrganizationName(TestData.Admin1.OrganizationID, "Test Org");
            Assert.AreEqual(TestData.Admin1.Organization.OrganizationName, "Test Org");
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

            DataService.UpdateDevice(TestData.Device1.ID, "", "", groupID);
            DataService.AddUserToDeviceGroup(TestData.Admin1.OrganizationID, groupID, TestData.User1.UserName, out _);

            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.Admin2.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User1.UserName).Count() == 2);
            Assert.IsTrue(DataService.GetDevicesForUser(TestData.User2.UserName).Count() == 1);
        }
    }
}
