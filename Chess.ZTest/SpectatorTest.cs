using Chess.IO;
using System;
using System.Net;
using System.Threading;

namespace Chess.ZTest
{
    internal class SpectatorTest
    {
        private static void _Main(string[] args)
        {
            StartNewGame();
            Thread.Sleep(500);
            SpectatorClient client = new SpectatorClient(IPAddress.Loopback);

            ChessAction action;
            do
            {
                action = client.ReadAction();
                Console.WriteLine(action);
            }
            while (!(action is EndGameAction));

            while (true)
                System.Threading.Thread.Sleep(1);
        }

        private static void StartNewGame()
        {
            Console.WriteLine("**********************");
            Console.WriteLine("****** New Game ******");
            Console.WriteLine("**********************");
            new Thread(StartNewGameCore) { IsBackground = true }.Start();
        }

        private static void StartNewGameCore()
        {
            var board = new Board(new ComputerPlayer(AILevel.Easy), new ComputerPlayer(AILevel.Easy));
            //board.Check += p => Console.WriteLine("Check");
            board.Checkmate += p => { Console.WriteLine("Checkmate: {0} wins", p == PlayerColor.Black ? PlayerColor.White : p); StartNewGame(); };
            board.Stalemate += (reason) => { Console.WriteLine("Stalemate: " + reason); StartNewGame(); };
            board.Start();

            var server = new SpectatorServer(board);

            while (board.IsActive)
                System.Threading.Thread.Sleep(1);
        }
    }
}