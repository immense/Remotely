using Immense.RemoteControl.Server.Hubs;
using Immense.RemoteControl.Server.Services;
using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Extensions;
using Remotely.Shared.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Tests;

[TestClass]
public class AgentHubTests
{
    private TestData _testData = null!;
    private IDataService _dataService = null!;

    [TestMethod]
    [DoNotParallelize]
    public async Task DeviceCameOnline_BannedByName()
    {
        var circuitManager = new Mock<ICircuitManager>();
        var circuitConnection = new Mock<ICircuitConnection>();
        circuitManager.Setup(x => x.Connections).Returns(new[] { circuitConnection.Object });
        circuitConnection.Setup(x => x.User).Returns(_testData.Org1Admin1);
        var appConfig = new Mock<IApplicationConfig>();
        var viewerHub = new Mock<IHubContext<ViewerHub>>();
        var expiringTokenService = new Mock<IExpiringTokenService>();
        var serviceSessionCache = new Mock<IAgentHubSessionCache>();
        var remoteControlSessions = new Mock<IRemoteControlSessionCache>();
        var messenger = new Mock<IMessenger>();
        var logger = new Mock<ILogger<AgentHub>>();

        appConfig.Setup(x => x.BannedDevices).Returns(new string[] { $"{_testData.Org1Device1.DeviceName}" });

        var hub = new AgentHub(
            _dataService,
            appConfig.Object,
            serviceSessionCache.Object,
            viewerHub.Object,
            circuitManager.Object,
            expiringTokenService.Object,
            remoteControlSessions.Object,
            messenger.Object,
            logger.Object);

        var hubClients = new Mock<IHubCallerClients<IAgentHubClient>>();
        var caller = new Mock<IAgentHubClient>();
        hubClients.Setup(x => x.Caller).Returns(caller.Object);
        hub.Clients = hubClients.Object;

        Assert.IsFalse(await hub.DeviceCameOnline(_testData.Org1Device1.ToDto()));
        hubClients.Verify(x => x.Caller, Times.Once);
        caller.Verify(x => x.UninstallAgent(), Times.Once);
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
        circuitConnection.Setup(x => x.User).Returns(_testData.Org1Admin1);
        var appConfig = new Mock<IApplicationConfig>();
        var viewerHub = new Mock<IHubContext<ViewerHub>>();
        var expiringTokenService = new Mock<IExpiringTokenService>();
        var serviceSessionCache = new Mock<IAgentHubSessionCache>();
        var remoteControlSessions = new Mock<IRemoteControlSessionCache>();
        var messenger = new Mock<IMessenger>();
        var logger = new Mock<ILogger<AgentHub>>();

        appConfig.Setup(x => x.BannedDevices).Returns(new string[] { _testData.Org1Device1.ID });

        var hub = new AgentHub(
            _dataService,
            appConfig.Object,
            serviceSessionCache.Object,
            viewerHub.Object,
            circuitManager.Object,
            expiringTokenService.Object,
            remoteControlSessions.Object,
            messenger.Object,
            logger.Object);

        var hubClients = new Mock<IHubCallerClients<IAgentHubClient>>();
        var caller = new Mock<IAgentHubClient>();
        hubClients.Setup(x => x.Caller).Returns(caller.Object);
        hub.Clients = hubClients.Object;

        Assert.IsFalse(await hub.DeviceCameOnline(_testData.Org1Device1.ToDto()));
        hubClients.Verify(x => x.Caller, Times.Once);
        caller.Verify(x => x.UninstallAgent(), Times.Once);
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

    private class CallerContext : HubCallerContext
    {
        public override string ConnectionId => "test-id";

        public override string? UserIdentifier => null;
        public override ClaimsPrincipal? User => null;

        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

        public override IFeatureCollection Features { get; } = new FeatureCollection();

        public override CancellationToken ConnectionAborted => CancellationToken.None;

        public override void Abort()
        {

        }
    }
}
