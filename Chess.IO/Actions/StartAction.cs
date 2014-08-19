using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chess.IO
{
    public class StartAction : ChessAction
    {
        public string FEN { get; private set; }

        public IList<Type> Sensors { get; private set; }

        public override byte OpCode
        {
            get { return 1; }
        }

        public StartAction(Board board, Sensors.SensorContainer sensors)
        {
            this.FEN = board.GetFEN();
            this.Sensors = sensors.GetSensorsType();
        }

        public StartAction(BinaryReader reader)
        {
            this.FEN = reader.ReadString();
            int count = reader.ReadInt32();
            this.Sensors = Enumerable.Range(0, count)
                                .Select(i => Chess.Sensors.Sensor.Load(reader.ReadString()))
                                .ToList();
        }

        public override void Write(ChessStreamWriter chessWriter, BinaryWriter writer)
        {
            writer.Write(this.FEN);
            writer.Write(this.Sensors.Count);
            foreach (var sensor in this.Sensors)
                writer.Write(sensor.Name);
        }

        public override string ToString()
        {
            return "Start: " + this.FEN;
        }
    }
}