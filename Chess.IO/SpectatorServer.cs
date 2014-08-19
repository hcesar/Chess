using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chess.IO
{
    public class SpectatorServer : IDisposable
    {
        public const int DEFAULTPORT = 8003;
        private Sensors.SensorContainer sensors;
        private TcpListener listener;
        private Thread listenerThread;
        private Board board;

        public SpectatorServer(Board board, Sensors.SensorContainer sensors = null)
        {
            ServerDiscovery.StartListening();
            this.board = board;
            this.sensors = sensors;

            listenerThread = new Thread(StartListening) { IsBackground = true };
            listenerThread.Start();
        }

        private void StartListening(object obj)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, DEFAULTPORT);
                listener.Start();
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var writer = new ChessStreamWriter(this.board, this.sensors, client.GetStream());
                }
            }
            catch
            {
                return;
            }
        }

        void IDisposable.Dispose()
        {
            this.listener.Stop();
            if (this.sensors != null)
                this.sensors.Stop();
        }
    }
}