using Microsoft.AspNetCore.SignalR;
using Moq;
using Remotely.Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Tests.Mocks;

public class HubContextFixture<THub, THubClient>
    where THub : Hub<THubClient>
    where THubClient : class
{
    public HubContextFixture()
    { 
        
        HubContextMock = new Mock<IHubContext<THub, THubClient>>();
        HubClientsMock = new Mock<IHubClients<THubClient>>();
        GroupManagerMock = new Mock<IGroupManager>();
        SingleClientProxyMock = new Mock<THubClient>();
        ClientProxyMock = new Mock<THubClient>();
        
        HubContextMock
            .Setup(x => x.Clients)
            .Returns(HubClientsMock.Object);

        HubContextMock
          .Setup(x => x.Groups)
          .Returns(GroupManagerMock.Object);

        HubClientsMock
            .Setup(x => x.Client(It.IsAny<string>()))
            .Returns(SingleClientProxyMock.Object);

        HubClientsMock
            .Setup(x => x.Group(It.IsAny<string>()))
            .Returns(ClientProxyMock.Object);
    }

    public Mock<IHubContext<THub, THubClient>> HubContextMock { get; }
    public Mock<IHubClients<THubClient>> HubClientsMock { get; }
    public Mock<IGroupManager> GroupManagerMock { get; }
    public Mock<THubClient> SingleClientProxyMock { get; }
    public Mock<THubClient> ClientProxyMock { get; }
}
