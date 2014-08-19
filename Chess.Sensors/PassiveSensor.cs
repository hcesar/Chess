using System.Threading;

namespace Chess.Sensors
{
    public abstract class PassiveSensor : Sensor
    {
        private int frequency;

        public PassiveSensor(int frequencyInMilliseconds)
        {
            this.frequency = frequencyInMilliseconds;
            new Thread(Pool) { IsBackground = true }.Start();
        }

        private void Pool()
        {
            while (true)
            {
                if (!this.IsRunning)
                    return;

                var data = this.GetSensorData();
                if (data != null)
                    this.OnDataAvailable(data);

                Thread.Sleep(this.frequency);
            }
        }

        protected abstract SensorData GetSensorData();
    }
}