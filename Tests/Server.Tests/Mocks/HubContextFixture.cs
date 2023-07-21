using Microsoft.AspNetCore.SignalR;
using Moq;
using Remotely.Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Tests.Mocks;

public class HubContextFixture<T>
    where T : Hub
{
    public HubContextFixture()
    { 
        HubContextMock = new Mock<IHubContext<T>>();
        HubClientsMock = new Mock<IHubClients>();
        GroupManagerMock = new Mock<IGroupManager>();
        SingleClientProxyMock = new Mock<ISingleClientProxy>();
        ClientProxyMock = new Mock<IClientProxy>();

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

    public Mock<IHubContext<T>> HubContextMock { get; }
    public Mock<IHubClients> HubClientsMock { get; }
    public Mock<IGroupManager> GroupManagerMock { get; }
    public Mock<ISingleClientProxy> SingleClientProxyMock { get; }
    public Mock<IClientProxy> ClientProxyMock { get; }
}
