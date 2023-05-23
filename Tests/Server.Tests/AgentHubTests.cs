using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
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
        private TestData _testData;

        public IDataService DataService { get; private set; }

        [TestMethod]
        [DoNotParallelize]
        public async Task DeviceCameOnline_BannedByName()
        {
            var circuitManager = new Mock<ICircuitManager>();
            var circuitConnection = new Mock<ICircuitConnection>();
            circuitManager.Setup(x => x.Connections).Returns(new[] { circuitConnection.Object });
            circuitConnection.Setup(x => x.User).Returns(_testData.Admin1);
            var appConfig = new Mock<IApplicationConfig>();
            var viewerHub = new Mock<IHubContext<ViewerHub>>();
            var expiringTokenService = new Mock<IExpiringTokenService>();
            var serviceSessionCache = new Mock<IServiceHubSessionCache>();
            var logger = new Mock<ILogger<AgentHub>>();

            appConfig.Setup(x => x.BannedDevices).Returns(new string[] { _testData.Device1.DeviceName });

            var hub = new AgentHub(
                DataService, 
                appConfig.Object, 
                serviceSessionCache.Object, 
                viewerHub.Object, 
                circuitManager.Object, 
                expiringTokenService.Object,
                logger.Object);

            var hubClients = new Mock<IHubCallerClients>();
            var caller = new Mock<ISingleClientProxy>();
            hubClients.Setup(x => x.Caller).Returns(caller.Object);
            hub.Clients = hubClients.Object;

            Assert.IsFalse(await hub.DeviceCameOnline(_testData.Device1));
            hubClients.Verify(x => x.Caller, Times.Once);
            caller.Verify(x => x.SendCoreAsync("UninstallAgent", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // TODO: Checking of device ban should be pulled out into
        // a separate service that's better testable.
        [TestMethod]
        [DoNotParallelize]
        public async Task DeviceCameOnline_BannedById()
        {
            var circuitManager = new Mock<ICircuitManager>();
            var circuitConnection = new Mock<ICircuitConnection>();
            circuitManager.Setup(x => x.Connections).Returns(new[] { circuitConnection.Object });
            circuitConnection.Setup(x => x.User).Returns(_testData.Admin1);
            var appConfig = new Mock<IApplicationConfig>();
            var viewerHub = new Mock<IHubContext<ViewerHub>>();
            var expiringTokenService = new Mock<IExpiringTokenService>();
            var serviceSessionCache = new Mock<IServiceHubSessionCache>();
            var logger = new Mock<ILogger<AgentHub>>();

            appConfig.Setup(x => x.BannedDevices).Returns(new string[] { _testData.Device1.ID });

            var hub = new AgentHub(
                DataService,
                appConfig.Object,
                serviceSessionCache.Object,
                viewerHub.Object,
                circuitManager.Object,
                expiringTokenService.Object,
                logger.Object);

            var hubClients = new Mock<IHubCallerClients>();
            var caller = new Mock<ISingleClientProxy>();
            hubClients.Setup(x => x.Caller).Returns(caller.Object);
            hub.Clients = hubClients.Object;

            Assert.IsFalse(await hub.DeviceCameOnline(_testData.Device1));
            hubClients.Verify(x => x.Caller, Times.Once);
            caller.Verify(x => x.SendCoreAsync("UninstallAgent", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
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
