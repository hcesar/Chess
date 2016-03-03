﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Sensors
{
    public abstract class Sensor
    {
        private static readonly IList<Type> AllSensors = AppDomain.CurrentDomain.GetAssemblies()
                                                    .SelectMany(i => i.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Sensor)) || type.IsSubclassOf(typeof(PassiveSensor))))
                                                    .ToList();


        public event Action<SensorData> DataAvailable;

        protected bool IsRunning { get; private set; }

        public Sensor()
        {
            this.IsRunning = true;
        }

        protected void OnDataAvailable(SensorData data)
        {
            if (!this.IsRunning)
                return;

            lock (this)
            {
                if (this.DataAvailable != null)
                    this.DataAvailable(data);
            }
        }

        public void Stop()
        {
            this.IsRunning = false;
        }

        public static void Init()
        {
        }

        public static Type Load(string sensorName)
        {
            var sensor = AllSensors.FirstOrDefault(i => i.Name == sensorName);

            if (sensor == null)
                throw new InvalidOperationException("Sensor not found: " + sensorName);

            return sensor;
        }
    }
}