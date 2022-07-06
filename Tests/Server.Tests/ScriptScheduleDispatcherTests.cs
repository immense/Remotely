using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class ScriptScheduleDispatcherTests
    {
        private ScriptSchedule _schedule1;
        private Mock<IDataService> _dataService;
        private Mock<ICircuitConnection> _circuitConnection;
        private Mock<ILogger<ScriptScheduleDispatcher>> _logger;
        private ScriptScheduleDispatcher _dispatcher;
        private TestData _testData;
        private SavedScript _savedScript;

        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();

            _savedScript = new SavedScript()
            {
                Id = Guid.NewGuid()
            };

            _schedule1 = new()
            {
                CreatedAt = Time.Now,
                CreatorId = _testData.User1.Id,
                Devices = new List<Device>()
                    {
                        _testData.Device1
                    },
                DeviceGroups = new List<DeviceGroup>()
                {
                    new DeviceGroup()
                    {
                        Devices = new List<Device>()
                        {
                            _testData.Device2
                        }
                    }
                },
                Interval = RepeatInterval.Hourly,
                Name = "_scheduleName",
                Id = 5,
                NextRun = Time.Now.AddMinutes(1),
                OrganizationID = _testData.User1.OrganizationID,
                SavedScriptId = _savedScript.Id
            };

            var scriptSchedules = new List<ScriptSchedule>()
            { 
                _schedule1
            };

            _dataService = new Mock<IDataService>();
            _dataService.Setup(x => x.GetScriptSchedulesDue()).Returns(Task.FromResult(scriptSchedules));
            _dataService.Setup(x => x.GetDevices(It.Is<IEnumerable<string>>(x =>
                x.Contains(_testData.Device1.ID) &&
                x.Contains(_testData.Device2.ID)
            ))).Returns(new List<Device>() { _testData.Device1, _testData.Device2 });

            _circuitConnection = new Mock<ICircuitConnection>();
            _logger = new Mock<ILogger<ScriptScheduleDispatcher>>();
            _dispatcher = new ScriptScheduleDispatcher(_dataService.Object, _circuitConnection.Object, _logger.Object);
        }

        [TestMethod]
        public async Task DispatchPendingScriptRuns_GivenNoSchedulesDue_DoesNothing()
        {
            await _dispatcher.DispatchPendingScriptRuns();

            _dataService.Verify(x => x.GetScriptSchedulesDue(), Times.Once);
            _dataService.VerifyNoOtherCalls();
            _circuitConnection.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DispatchPendingScriptRuns_GivenSchedulesDue_CreatesScriptRuns()
        {
            Assert.IsFalse(_schedule1.LastRun.HasValue);

            Time.Adjust(TimeSpan.FromMinutes(5));

            await _dispatcher.DispatchPendingScriptRuns();

            Assert.IsTrue(_schedule1.LastRun.HasValue);

            _dataService.Verify(x => x.GetScriptSchedulesDue(), Times.Once);
            _dataService.Verify(x => x.AddOrUpdateScriptSchedule(_schedule1), Times.Once);
            _dataService.Verify(x => x.GetDevices(It.Is<IEnumerable<string>>(x => 
                x.Contains(_schedule1.Devices.First().ID))));
            _dataService.Verify(x => x.AddScriptRun(It.Is<ScriptRun>(x => 
                x.ScheduleId == _schedule1.Id &&
                x.Devices.Exists(d => d.ID == _testData.Device1.ID) &&
                x.Devices.Exists(d => d.ID == _testData.Device2.ID))));
            _dataService.VerifyNoOtherCalls();

            _circuitConnection.Verify(x => x.RunScript(
                It.IsAny<IEnumerable<string>>(),
                _savedScript.Id,
                It.IsAny<int>(),
                ScriptInputType.ScheduledScript,
                true
            ), Times.Once);
            _circuitConnection.VerifyNoOtherCalls();
        }
    }
}
