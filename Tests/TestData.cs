using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Tests
{
    public class TestData
    {
        public static RemotelyUser TestAdmin1 { get; } = new RemotelyUser()
        {
            UserName = "testadmin1@test.com",
            IsAdministrator = true
        };
    }
}
