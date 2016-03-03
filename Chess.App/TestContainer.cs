using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.App
{
    public class TestContainer
    {
        public Tests.TestOrchestrator Orchestrator { get; set; }
        public Sensors.SensorContainer SensorContainer { get; set; }

        public TestContainer(Tests.TestOrchestrator orchestrator, Sensors.SensorContainer sensorContainer)
        {
            this.Orchestrator = orchestrator;
            this.SensorContainer = sensorContainer;
        }
    }
}
