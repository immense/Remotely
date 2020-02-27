using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Agent.Installer.Win.Services;
using System.Linq;
using System.Runtime.InteropServices;

namespace Remotely.Tests
{
    [TestClass]
    public class InstallerTests
    {
        [TestMethod]
        public void CoreVersionTest()
        {
            var description = RuntimeInformation.FrameworkDescription;
            var version = description.Split().Last();
            Assert.AreEqual(InstallerService.CoreRuntimeVersion, version);
        }
    }
}
