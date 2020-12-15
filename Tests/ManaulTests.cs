using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class ManaulTests
    {
        [TestMethod]
        //[Ignore("Manual test.")]
        public void HandlesCount()
        {
            var startHandles = Process.GetCurrentProcess().HandleCount;

            for (var i = 0; i < 1000; i++)
            {
                var result = Win32Interop.SwitchToInputDesktop();
            }

            var endHandles = Process.GetCurrentProcess().HandleCount;
        }
    }
}
