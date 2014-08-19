using System;
using System.ServiceModel;

namespace Chess.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Server : ChessConnection, IChessServerService, IDisposable
    {
        private IChessClientCallback client;
        private ServiceHost host;

        public Server()
        {
            ServerDiscovery.StartListening();

            this.host = new ServiceHost(this);
            host.AddServiceEndpoint(typeof(IChessServerService), new NetTcpBinding(SecurityMode.None), "net.tcp://localhost:8001/Chess");
            Console.WriteLine("SERVER: Host created!");
            host.Open();
        }

        public override void SendMove(Square source, Square target, Type piecePromotedTo)
        {
            Console.WriteLine("SERVER: Moving {0}{1}", source, target);
            client.ReceiveMove(source, target, piecePromotedTo);
        }

        string IChessServerService.GetStartingFEN()
        {
            int timeout = 5000;
            while (this.StartingFEN == null && --timeout > 0)
                System.Threading.Thread.Sleep(1);

            string fen = this.StartingFEN;
            if (fen == null)
                throw new TimeoutException("Could not get FEN.");

            return fen;
        }

        void IChessServerService.ReceiveMove(Square from, Square target, Type promotePawnTo)
        {
            Console.WriteLine("CLIENT: Moving {0}{1}", from, target);
            this.OnReceiveMove(from, target, promotePawnTo);
        }

        public void Connect()
        {
            Console.WriteLine("SERVER: Client connected!");
            this.IsConnected = true;
            client = OperationContext.Current.GetCallbackChannel<IChessClientCallback>();
        }

        void IDisposable.Dispose()
        {
        }
    }
}