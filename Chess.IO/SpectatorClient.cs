using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chess.IO
{
    public class SpectatorClient
    {
        private ChessStreamReader chessReader;

        public string StartingFEN { get; private set; }

        public int GameDuration { get; private set; }

        public int Speed { get; set; }

        public bool IsLive { get; private set; }

        public SpectatorClient(IPAddress server)
        {
            var client = new TcpClient();
            client.Connect(new IPEndPoint(server, SpectatorServer.DEFAULTPORT));
            Start(client.GetStream());
        }

        public SpectatorClient(string file)
        {
            Start(File.OpenRead(file));
        }

        public SpectatorClient(Stream stream)
        {
            Start(stream);
        }

        private void Start(Stream stream)
        {
            this.Speed = 1;
            this.chessReader = new ChessStreamReader(stream);

            var start = this.chessReader.ReadAction();
            if (!(start is StartAction))
                throw new InvalidOperationException("Expected: StartPositionAction");

            this.StartingFEN = ((StartAction)start).FEN;
            this.GameDuration = this.chessReader.GameDuration;
            this.IsLive = this.GameDuration == 0;
        }

        public ChessAction ReadAction()
        {
            var action = this.chessReader.ReadAction();

            if (!this.IsLive)
                Thread.Sleep(action.TimeDelay);

            return action;
        }
    }
}