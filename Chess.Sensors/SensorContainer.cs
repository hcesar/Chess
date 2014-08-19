using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Sensors
{
    public class SensorContainer
    {
        public event SensorDataReceived SensorDataReceived;

        private List<Sensor> sensors = new List<Sensor>();
        private Board board;

        public SensorContainer(Board board)
        {
            this.board = board;
        }

        public SensorContainer(Board board, params Sensor[] sensors)
        {
            this.board = board;
            foreach (var sensor in sensors)
                this.Add(sensor);
        }

        private void OnSensorData(Sensor sensor, SensorData data)
        {
            if (this.SensorDataReceived != null)
                this.SensorDataReceived(sensor, data);
        }

        public void Add(Sensor sensor)
        {
            sensor.SetBoard(this.board);
            lock (this.sensors)
                this.sensors.Add(sensor);

            sensor.DataAvailable += (data) => OnSensorData(sensor, data);
        }

        public void Stop()
        {
            foreach (var sensor in this.sensors)
                sensor.Stop();
        }

        public IList<Type> GetSensorsType()
        {
            return this.sensors.Select(i => i.GetType()).ToList();
        }
    }
}