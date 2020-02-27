using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            IoCActivator.Activate();
        }


        private DataService DataService { get; set; }

        private DataService GetDataService()
        {
            //var contextOptions = new DbContextOptions<ApplicationDbContext>();
            //var appDbContext = new ApplicationDbContext(contextOptions);
            //var appConfig = new ApplicationConfig(new ConfigurationRoot());
            //var dataService = new DataService(appDbContext);

            //return dataService;
            return null;
        }

        [TestMethod]
        public async Task Test()
        {
            var dataService = IoCActivator.ServiceProvider.GetRequiredService<DataService>();
            var userManager = IoCActivator.ServiceProvider.GetRequiredService <UserManager<RemotelyUser>>();

            Assert.IsNull(dataService.GetUserByName(TestData.TestAdmin1.UserName));
            Assert.AreEqual(0, dataService.GetOrganizationCount());

            await userManager.CreateAsync(TestData.TestAdmin1);

            Assert.IsNotNull(dataService.GetUserByName(TestData.TestAdmin1.UserName));
            Assert.AreEqual(1, dataService.GetOrganizationCount());
        }

        [TestMethod]
        public void Test2()
        {
            var test = IoCActivator.ServiceProvider;
        }

    }

}
