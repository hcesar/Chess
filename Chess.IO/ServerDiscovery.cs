using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO
{
    public static class ServerDiscovery
    {
        private const string QUERY_MESSAGE = "Is Anybody there?";
        private const string RESPONSE_MESSAGE = "ChessServer here!";

        public static IList<IPEndPoint> FindServers()
        {
            var rt = new List<IPEndPoint>();

            foreach (var ip in GetLocalIPs())
            {
                UdpClient client = new UdpClient();

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 8005);
                client.Send(Encoding.UTF8.GetBytes(QUERY_MESSAGE), QUERY_MESSAGE.Length, new IPEndPoint(IPAddress.Broadcast, 8002));
                var receiveTask = client.ReceiveAsync();

                receiveTask.Wait(500);
                if (!receiveTask.IsCompleted || Encoding.UTF8.GetString(receiveTask.Result.Buffer) != RESPONSE_MESSAGE)
                    continue;

                rt.Add(receiveTask.Result.RemoteEndPoint);
            }

            return rt;
        }

        private static IList<UdpClient> s_Clients = new List<UdpClient>();

        public static void StartListening()
        {
            foreach (var client in s_Clients)
                client.Close();

            s_Clients.Clear();
            foreach (var ip in GetLocalIPs())
                new Task(StartListening, ip).Start();
        }

        private static void StartListening(object state)
        {
            IPAddress ip = (IPAddress)state;
            var client = new UdpClient(new IPEndPoint(ip, 8002));
            s_Clients.Add(client);

            IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                IPEndPoint sender = any;
                byte[] data = client.Receive(ref sender);
                if (Encoding.UTF8.GetString(data) == QUERY_MESSAGE)
                    client.Send(Encoding.UTF8.GetBytes(RESPONSE_MESSAGE), RESPONSE_MESSAGE.Length, sender);
            }
        }

        private static IEnumerable<IPAddress> GetLocalIPs()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork))
                yield return ip;
        }
    }
}