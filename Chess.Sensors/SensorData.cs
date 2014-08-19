using System;
using System.Collections.Generic;
using System.IO;

namespace Chess.Sensors
{
    public abstract class SensorData
    {
        public Sensor Sensor { get; internal set; }

        public abstract void Serialize(BinaryWriter writer);

        public static SensorData Deserialize(Type sensorType, BinaryReader reader)
        {
            Func<BinaryReader, SensorData> deserializer = GetDeserializer(sensorType);
            return deserializer(reader);
        }

        private static Dictionary<Type, Func<BinaryReader, SensorData>> s_DeserializerCache = new Dictionary<Type, Func<BinaryReader, SensorData>>();

        private static Func<BinaryReader, SensorData> GetDeserializer(Type sensorType)
        {
            Func<BinaryReader, SensorData> rt;
            if (s_DeserializerCache.TryGetValue(sensorType, out rt))
                return rt;

            rt = (Func<BinaryReader, SensorData>)Delegate.CreateDelegate(typeof(Func<BinaryReader, SensorData>), sensorType, "Deserialize", false);
            s_DeserializerCache.Add(sensorType, rt);
            return rt;
        }

        public abstract Type GetSensorType();
    }
}