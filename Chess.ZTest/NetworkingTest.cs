using Chess.IO;
using System;
using System.Linq;
using System.Net;

namespace Chess.ZTest
{
    internal class NetworkingTest
    {
        private static void Test()
        {
            new System.Threading.Thread(StartServer).Start();

            System.Threading.Thread.Sleep(100);
            var client = new Client(IPAddress.Loopback);
            var white = new NetworkPlayer(client);
            var board = new Board(white, new ComputerPlayer(), white.StartingFEN);
            board.Start();

            Console.ReadLine();
        }

        private static void StartServer()
        {
            var server = new Server();
            var board = new Board(new ComputerPlayer(), new NetworkPlayer(server));

            int c = 0;
            while (board.Players.Values.Any(i => !i.IsReady))
            {
                if (++c % 1000 == 0)
                    Console.WriteLine("Waiting...");
                System.Threading.Thread.Sleep(1);
            }

            board.Start();
        }
    }
}