using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Sensors
{
    public class SensorDataEntry<TSensorData>
    {
        public SensorDataEntry(int timeFrame, TSensorData sensorData)
        {
            this.TimeFrame = timeFrame;
            this.SensorData = sensorData;
        }

        public int TimeFrame { get; private set; }
        public TSensorData SensorData { get; private set; }
    }
}
