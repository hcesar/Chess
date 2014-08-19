using Chess.Sensors;
using System;
using System.IO;

namespace Chess.IO
{
    public class SensorAction : ChessAction
    {
        public override byte OpCode
        {
            get { return 3; }
        }

        public Sensors.SensorData Data { get; private set; }

        public SensorAction(Sensors.SensorData data)
        {
            this.Data = data;
        }

        public override void Write(ChessStreamWriter chessWriter, BinaryWriter writer)
        {
            var type = this.Data.GetSensorType();
            int index = chessWriter.GetSensorIndex(type);

            writer.Write((byte)index);
            this.Data.Serialize(writer);
        }

        public SensorAction(ChessStreamReader chessReader, BinaryReader reader)
        {
            byte typeIndex = reader.ReadByte();
            Type sensorType = chessReader.SensorsFound[typeIndex];

            this.Data = SensorData.Deserialize(sensorType, reader);
        }
    }
}