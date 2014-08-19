using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Chess.IO
{
    public class ChessStreamWriter : IDisposable
    {
        private Stopwatch sw;
        private Stream outputStream;
        private IChessStream chessStream;
        private BinaryWriter writer;
        private long lastAction = 0;
        private IList<Type> sensors;

        public ChessStreamWriter(Board board, Sensors.SensorContainer sensors, string destinationFile)
            : this(board, sensors, File.OpenWrite(destinationFile))
        {
        }

        public ChessStreamWriter(Board board, Sensors.SensorContainer sensors, Stream outputStream)
            : this(new ChessStream(board, sensors), outputStream)
        {
        }

        public ChessStreamWriter(IChessStream chessStream, Stream outputStream)
        {
            this.sensors = chessStream.SensorsType;

            outputStream.Write(new byte[4], 0, 4);

            this.chessStream = chessStream;
            this.outputStream = outputStream;
            this.sw = Stopwatch.StartNew();

            if (outputStream is System.Net.Sockets.NetworkStream)
                this.writer = new BinaryWriter(outputStream);
            else
                this.writer = new BinaryWriter(new GZipStream(outputStream, CompressionLevel.Optimal));
            this.chessStream.ActionAvailable += Persist;
        }

        private void Persist(ChessAction action)
        {
            if (action.TimeDelay >= 0)
                writer.Write(action.TimeDelay);
            else
                writer.Write((int)(sw.ElapsedMilliseconds - lastAction));

            this.lastAction = sw.ElapsedMilliseconds;
            writer.Write(action.OpCode);
            action.Write(this, writer);
            writer.Flush();

            if (action is EndGameAction)
                ((IDisposable)this).Dispose();
        }

        internal int GetSensorIndex(Type sensorType)
        {
            for (int i = 0; i < this.sensors.Count; i++)
                if (this.sensors[i] == sensorType)
                    return i;

            throw new InvalidOperationException("Sensor not found: " + sensorType.Name);
        }

        void IDisposable.Dispose()
        {
            this.chessStream.ActionAvailable -= Persist;
            this.writer.Flush();

            if (this.outputStream.CanSeek)
            {
                this.outputStream.Flush();
                long position = this.outputStream.Position;
                this.outputStream.Seek(0, SeekOrigin.Begin);
                this.outputStream.Write(BitConverter.GetBytes((int)sw.Elapsed.TotalSeconds), 0, 4);
                //this.outputStream.Write(BitConverter.GetBytes(3756), 0, 4);
                this.outputStream.Flush();
                this.outputStream.Seek(position, SeekOrigin.Begin);
            }

            this.writer.Close();
            this.outputStream.Close();
            this.chessStream.Dispose();
            this.writer = null;
        }
    }
}