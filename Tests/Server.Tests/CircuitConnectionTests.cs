#nullable enable
using Immense.RemoteControl.Server.Services;
using Immense.SimpleMessenger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Server.Services.Stores;
using Remotely.Server.Tests.Mocks;
using Remotely.Shared.Extensions;
using Remotely.Shared.Interfaces;
using System.Threading.Tasks;

namespace Remotely.Server.Tests;

[TestClass]
public class CircuitConnectionTests
{
#nullable disable
    private TestData _testData;
    private IDataService _dataService;
    private Mock<IAuthService> _authService;
    private Mock<ISelectedCardsStore> _clientAppState;
    private HubContextFixture<AgentHub, IAgentHubClient> _agentHubContextFixture;
    private Mock<IApplicationConfig> _appConfig;
    private Mock<ICircuitManager> _circuitManager;
    private Mock<IToastService> _toastService;
    private Mock<IExpiringTokenService> _expiringTokenService;
    private Mock<IRemoteControlSessionCache> _remoteControlSessionCache;
    private Mock<IMessenger> _messenger;
    private Mock<IAgentHubSessionCache> _agentSessionCache;
    private Mock<ILogger<CircuitConnection>> _logger;
    private CircuitConnection _circuitConnection;
#nullable enable

    [TestInitialize]
    public async Task Init()
    {
        _testData = new TestData();
        await _testData.Init();

        _dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
        _authService = new Mock<IAuthService>();
        _clientAppState = new Mock<ISelectedCardsStore>();
        _agentHubContextFixture = new HubContextFixture<AgentHub, IAgentHubClient>();
        _appConfig = new Mock<IApplicationConfig>();
        _circuitManager = new Mock<ICircuitManager>();
        _toastService = new Mock<IToastService>();
        _expiringTokenService = new Mock<IExpiringTokenService>();
        _remoteControlSessionCache = new Mock<IRemoteControlSessionCache>();
        _messenger = new Mock<IMessenger>();
        _agentSessionCache = new Mock<IAgentHubSessionCache>();
        _logger = new Mock<ILogger<CircuitConnection>>();

        _circuitConnection = new CircuitConnection(
            _authService.Object,
            _dataService,
            _clientAppState.Object,
            _agentHubContextFixture.HubContextMock.Object,
            _appConfig.Object,
            _circuitManager.Object,
            _toastService.Object,
            _expiringTokenService.Object,
            _remoteControlSessionCache.Object,
            _agentSessionCache.Object,
            _messenger.Object,
            _logger.Object);
    }

    [TestMethod]
    public async Task WakeDevice_GivenUserIsUnauthorized_Fails()
    {
        // A standard user won't have access if they aren't in the same
        // group as the device.
        _circuitConnection.User = _testData.Org1User1;

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { "78E3B5A1E45B" };
        _testData.Org1Device1.DeviceGroupID = _testData.Org1Group1.ID;
        // Online device.
        _testData.Org1Device2.PublicIP = "142.251.33.110";
        // Device in another org that shouldn't receive the command.
        _testData.Org2Device1.PublicIP = "142.251.33.110";


        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device2.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org2Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);

        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device2.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org2Device1.ID, _testData.Org2Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        var wakeResult = await _circuitConnection.WakeDevice(_testData.Org1Device1);
        Assert.IsFalse(wakeResult.IsSuccess);

        _agentSessionCache.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task WakeDevice_GivenMatchingPeerByIp_UsesCorrectPeer()
    {
        _circuitConnection.User = _testData.Org1User1;

        var macAddress = "78E3B5A1E45B";

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { macAddress };
        // Online device.
        _testData.Org1Device2.PublicIP = "142.251.33.110";
        // Device in another org that shouldn't receive the command.
        _testData.Org2Device1.PublicIP = "142.251.33.110";

        // Offline device in the same group as user.
        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        var addToGroupResult = _dataService.AddUserToDeviceGroup(
            _testData.Org1Id,
            _testData.Org1Group1.ID,
            $"{_testData.Org1User1.UserName}",
            out _);

        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device2.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org2Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);

        _agentSessionCache
            .Setup(x => x.GetAllDevices())
            .Returns(new[]
            {
                _testData.Org1Device2,
                _testData.Org2Device1
            });

        var connectionId = "HQUSIBxiOwNokVH_mYgGyg";

        _agentSessionCache
            .Setup(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId))
            .Returns(true);

        var wakeResult = await _circuitConnection.WakeDevice(_testData.Org1Device1);

        Assert.IsTrue(addToGroupResult);
        Assert.IsTrue(wakeResult.IsSuccess);


        _agentSessionCache
            .Verify(x => x.GetAllDevices(), Times.Once);

        _agentSessionCache
            .Verify(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId), Times.Once);

        _agentHubContextFixture.HubClientsMock
            .Verify(x => x.Client(connectionId), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock
            .Verify(x =>
                x.WakeDevice(macAddress), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
        _agentSessionCache.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task WakeDevice_GivenMatchingPeerByGroupId_UsesCorrectPeer()
    {
        _circuitConnection.User = _testData.Org1User1;

        var macAddress = "78E3B5A1E45B";

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { macAddress };

        var addToGroupResult = _dataService.AddUserToDeviceGroup(
            _testData.Org1Id,
            _testData.Org1Group1.ID,
            $"{_testData.Org1User1.UserName}",
            out _);

        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);

        // Offline device.
        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        // Online device in the same group and org.  Should relay wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device2.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        // Online device in a different org.  Should not receive wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org2Device1.ID, _testData.Org2Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);


        _agentSessionCache
            .Setup(x => x.GetAllDevices())
            .Returns(new[]
            {
                _testData.Org1Device2,
                _testData.Org2Device1
            });

        var connectionId = "HQUSIBxiOwNokVH_mYgGyg";

        _agentSessionCache
            .Setup(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId))
            .Returns(true);

        var wakeResult = await _circuitConnection.WakeDevice(_testData.Org1Device1);

        Assert.IsTrue(addToGroupResult);
        Assert.IsTrue(wakeResult.IsSuccess);


        _agentSessionCache
            .Verify(x => x.GetAllDevices(), Times.Once);

        _agentSessionCache
            .Verify(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId), Times.Once);

        _agentHubContextFixture.HubClientsMock
            .Verify(x => x.Client(connectionId), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock
            .Verify(x =>
                x.WakeDevice(macAddress), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
        _agentSessionCache.VerifyNoOtherCalls();
    }


    [TestMethod]
    public async Task WakeDevice_GivenNoMatchingGroupOrIp_DoesNotSend()
    {
        _circuitConnection.User = _testData.Org1User1;

        var macAddress = "78E3B5A1E45B";

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { macAddress };
        _testData.Org1Device1.DeviceGroupID = _testData.Org1Group1.ID;
        // Online device, but in a different group.
        _testData.Org1Device2.DeviceGroupID = _testData.Org1Group2.ID;
        // Device in another org that shouldn't receive the command.
        _testData.Org2Device1.DeviceGroupID = _testData.Org2Group1.ID;

        var addToGroupResult = _dataService.AddUserToDeviceGroup(
            _testData.Org1Id,
            _testData.Org1Group1.ID,
            $"{_testData.Org1User1.UserName}",
            out _);

        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device2.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org2Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);


        // Offline device.
        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        // Online device in a different group.  Should not recieve wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device2.ID, _testData.Org1Group2.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        // Online device in a different org.  Should not recieve wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org2Device1.ID, _testData.Org2Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        _agentSessionCache
            .Setup(x => x.GetAllDevices())
            .Returns(new[]
            {
                _testData.Org1Device2,
                _testData.Org2Device1
            });

        var wakeResult = await _circuitConnection.WakeDevice(_testData.Org1Device1);

        Assert.IsTrue(addToGroupResult);
        Assert.IsTrue(wakeResult.IsSuccess);


        _agentSessionCache
            .Verify(x => x.GetAllDevices(), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
        _agentSessionCache.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task WakeDevices_GivenPeerIpMatches_UsesCorrectPeer()
    {
        _circuitConnection.User = _testData.Org1User1;

        var macAddress = "78E3B5A1E45B";

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { macAddress };
        _testData.Org1Device1.DeviceGroupID = _testData.Org1Group1.ID;
        // Online device.
        _testData.Org1Device2.PublicIP = "142.251.33.110";
        // Device in another org that shouldn't receive the command.
        _testData.Org2Device1.PublicIP = "142.251.33.110";

        // Offline device in the same group as user.
        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        var addToGroupResult = _dataService.AddUserToDeviceGroup(
            _testData.Org1Id,
            _testData.Org1Group1.ID,
            $"{_testData.Org1User1.UserName}",
            out _);

        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device2.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org2Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);

        _agentSessionCache
            .Setup(x => x.GetAllDevices())
            .Returns(new[]
            {
                _testData.Org1Device2,
                _testData.Org2Device1
            });

        var connectionId = "HQUSIBxiOwNokVH_mYgGyg";

        _agentSessionCache
            .Setup(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId))
            .Returns(true);

        var wakeResult = await _circuitConnection.WakeDevices(new[] { _testData.Org1Device1 });

        Assert.IsTrue(addToGroupResult);
        Assert.IsTrue(wakeResult.IsSuccess);


        _agentSessionCache
            .Verify(x => x.GetAllDevices(), Times.Once);

        _agentSessionCache
            .Verify(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId), Times.Once);

        _agentHubContextFixture.HubClientsMock
            .Verify(x => x.Client(connectionId), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock
            .Verify(x =>
                x.WakeDevice(macAddress), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
        _agentSessionCache.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task WakeDevices_GivenMatchingPeerByGroupId_UsesCorrectPeer()
    {
        _circuitConnection.User = _testData.Org1User1;

        var macAddress = "78E3B5A1E45B";

        // Offline device.
        _testData.Org1Device1.PublicIP = "142.251.33.110";
        _testData.Org1Device1.MacAddresses = new[] { macAddress };
        _testData.Org1Device1.DeviceGroupID = _testData.Org1Group1.ID;
        // Online device.
        _testData.Org1Device2.DeviceGroupID = _testData.Org1Group1.ID;
        // Device in another org that shouldn't receive the command.
        _testData.Org2Device1.DeviceGroupID = _testData.Org2Group1.ID;

        var addToGroupResult = _dataService.AddUserToDeviceGroup(
            _testData.Org1Id,
            _testData.Org1Group1.ID,
            $"{_testData.Org1User1.UserName}",
            out _);

        var updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org1Device2.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);
        updateResult = await _dataService.AddOrUpdateDevice(_testData.Org2Device1.ToDto());
        Assert.IsTrue(updateResult.IsSuccess);

        // Offline device.
        var addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device1.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        // Online device in the same group and org.  Should relay wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org1Device2.ID, _testData.Org1Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);
        // Online device in a different org.  Should not receive wake command.
        addGroupResult = await _dataService.AddDeviceToGroup(_testData.Org2Device1.ID, _testData.Org2Group1.ID);
        Assert.IsTrue(addGroupResult.IsSuccess);

        _agentSessionCache
            .Setup(x => x.GetAllDevices())
            .Returns(new[]
            {
                _testData.Org1Device2,
                _testData.Org2Device1
            });

        var connectionId = "HQUSIBxiOwNokVH_mYgGyg";

        _agentSessionCache
            .Setup(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId))
            .Returns(true);

        var wakeResult = await _circuitConnection.WakeDevices(new[] { _testData.Org1Device1 });

        Assert.IsTrue(addToGroupResult);
        Assert.IsTrue(wakeResult.IsSuccess);


        _agentSessionCache
            .Verify(x => x.GetAllDevices(), Times.Once);

        _agentSessionCache
            .Verify(x => x.TryGetConnectionId(_testData.Org1Device2.ID, out connectionId), Times.Once);

        _agentHubContextFixture.HubClientsMock
            .Verify(x => x.Client(connectionId), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock
            .Verify(x =>
                x.WakeDevice(macAddress), Times.Once);

        _agentHubContextFixture.SingleClientProxyMock.VerifyNoOtherCalls();
        _agentHubContextFixture.HubContextMock.VerifyNoOtherCalls();
        _agentSessionCache.VerifyNoOtherCalls();
    }
}
