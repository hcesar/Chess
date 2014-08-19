using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Chess.IO
{
    public class ChessStreamReader
    {
        private Stream stream;
        private BinaryReader reader;
        internal IList<Type> SensorsFound = new List<Type>();

        public int GameDuration { get; private set; }

        public ChessStreamReader(string fileName)
            : this(File.OpenRead(fileName))
        {
        }

        public ChessStreamReader(Stream stream)
        {
            this.stream = stream;
            byte[] duration = new byte[4];
            this.stream.Read(duration, 0, 4);
            this.GameDuration = BitConverter.ToInt32(duration, 0);
            if (this.stream is System.Net.Sockets.NetworkStream)
                this.reader = new BinaryReader(this.stream);
            else
                this.reader = new BinaryReader(new GZipStream(this.stream, CompressionMode.Decompress));
        }

        public ChessAction ReadAction()
        {
            int timePosition = this.reader.ReadInt32();
            byte opcode = this.reader.ReadByte();
            ChessAction action;
            switch (opcode)
            {
                case 1: action = new StartAction(reader);
                    this.SensorsFound = ((StartAction)action).Sensors;
                    break;

                case 2:
                    action = new MoveAction(reader);
                    break;

                case 3:
                    action = new SensorAction(this, reader);
                    break;

                case 4:
                    action = new EndGameAction(reader);
                    break;

                default:
                    throw new NotSupportedException("Action not supported: " + opcode);
            }

            action.TimeDelay = timePosition;
            return action;
        }

        public IEnumerable<ChessAction> ReadAllActions()
        {
            ChessAction action = null;

            while (!(action is EndGameAction))
            {
                action = this.ReadAction();
                yield return action;
            }
        }
    }
}