using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class AgentHubTests
    {
        public IDataService DataService { get; private set; }

        [TestMethod]
        [DoNotParallelize]
        public async Task DeviceCameOnline_BannedByName()
        {
            var appConfig = new Mock<IApplicationConfig>();
            var browserHub = new Mock<IHubContext<BrowserHub>>();
            var viewerHub = new Mock<IHubContext<ViewerHub>>();

            appConfig.Setup(x => x.BannedDevices).Returns(new string[] { TestData.Device1.DeviceName });

            var hub = new AgentHub(DataService, appConfig.Object, browserHub.Object, viewerHub.Object);

            var hubClients = new Mock<IHubCallerClients>();
            var caller = new Mock<IClientProxy>();
            hubClients.Setup(x => x.Caller).Returns(caller.Object);
            hub.Clients = hubClients.Object;

            Assert.IsFalse(await hub.DeviceCameOnline(TestData.Device1));
            hubClients.Verify(x => x.Caller, Times.Once);
            caller.Verify(x => x.SendCoreAsync("UninstallAgent", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task DeviceCameOnline_BannedById()
        {
            var appConfig = new Mock<IApplicationConfig>();
            var browserHub = new Mock<IHubContext<BrowserHub>>();
            var viewerHub = new Mock<IHubContext<ViewerHub>>();

            appConfig.Setup(x => x.BannedDevices).Returns(new string[] { TestData.Device1.ID });

            var hub = new AgentHub(DataService, appConfig.Object, browserHub.Object, viewerHub.Object);

            var hubClients = new Mock<IHubCallerClients>();
            var caller = new Mock<IClientProxy>();
            hubClients.Setup(x => x.Caller).Returns(caller.Object);
            hub.Clients = hubClients.Object;

            Assert.IsFalse(await hub.DeviceCameOnline(TestData.Device1));
            hubClients.Verify(x => x.Caller, Times.Once);
            caller.Verify(x => x.SendCoreAsync("UninstallAgent", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task DeviceCameOnline_NotBanned()
        {
            var appConfig = new Mock<IApplicationConfig>();

            var browserHub = new Mock<IHubContext<BrowserHub>>();
            var browserHubClients = new Mock<IHubClients>();
            var browserClientsProxy = new Mock<IClientProxy>();
            browserHubClients.Setup(x => x.Clients(It.IsAny<IReadOnlyList<string>>())).Returns(browserClientsProxy.Object);
            browserHub.Setup(x => x.Clients).Returns(browserHubClients.Object);

            var viewerHub = new Mock<IHubContext<ViewerHub>>();

            appConfig.Setup(x => x.BannedDevices).Returns(new string[0]);

            var hub = new AgentHub(DataService, appConfig.Object, browserHub.Object, viewerHub.Object);

            hub.Context = new CallerContext();
            //hub.Context.Items.Add("Device", TestData.Device1);

            var agentHubClients = new Mock<IHubCallerClients>();
            var agentHubCaller = new Mock<IClientProxy>();
            var agentClientsProxy = new Mock<IClientProxy>();
            agentHubClients.Setup(x => x.Caller).Returns(agentHubCaller.Object);
            agentHubClients.Setup(x => x.Clients(It.IsAny<IReadOnlyList<string>>())).Returns(agentClientsProxy.Object);
            hub.Clients = agentHubClients.Object;

            Assert.IsTrue(await hub.DeviceCameOnline(TestData.Device1));
            agentHubClients.Verify(x => x.Caller, Times.Never);
            agentHubCaller.Verify(x => x.SendCoreAsync("UninstallAgent", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Never);

            browserClientsProxy.Verify(x => x.SendCoreAsync("DeviceCameOnline", 
                It.Is<object[]>(x => x[0] == TestData.Device1),
                It.IsAny<CancellationToken>()), 
                Times.Once);
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

        private class CallerContext : HubCallerContext
        {
            public override string ConnectionId => "test-id";

            public override string UserIdentifier => null;

            public override ClaimsPrincipal User => null;

            public override IDictionary<object, object> Items { get; } = new Dictionary<object, object>();

            public override IFeatureCollection Features { get; } = new FeatureCollection();

            public override CancellationToken ConnectionAborted => CancellationToken.None;

            public override void Abort()
            {
                
            }
        }
    }
}
