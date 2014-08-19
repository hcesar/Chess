using System;
using System.Net;
using System.ServiceModel;

namespace Chess.IO
{
    public class Client : ChessConnection, IChessClientCallback, IDisposable
    {
        private IChessServerService server;

        public Client(IPAddress ipAddress)
        {
            var myBinding = new NetTcpBinding(SecurityMode.None);
            var myEndpoint = new EndpointAddress("net.tcp://" + ipAddress + ":8001/Chess");
            var myChannelFactory = new DuplexChannelFactory<IChessServerService>(this, myBinding, myEndpoint);
            this.server = myChannelFactory.CreateChannel();
            this.IsConnected = true;
            this.StartingFEN = server.GetStartingFEN();

            Console.WriteLine("CLIENT: Connected to the server!");
        }

        public override void Initialize()
        {
            this.server.Connect();
            base.Initialize();
        }

        public override void SendMove(Square source, Square target, Type piecePromotedTo)
        {
            Console.WriteLine("SERVER: Moving {0}{1}", source, target);
            server.ReceiveMove(source, target, piecePromotedTo);
        }

        void IChessClientCallback.ReceiveMove(Square from, Square target, Type promotePawnTo)
        {
            Console.WriteLine("CLIENT: Moving {0}{1}", from, target);
            this.OnReceiveMove(from, target, promotePawnTo);
        }

        void IDisposable.Dispose()
        {
        }
    }
}