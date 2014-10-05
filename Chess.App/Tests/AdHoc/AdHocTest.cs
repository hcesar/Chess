using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Chess.App.Tests
{
    public class AdHocTest : Test
    {
        public string FEN { get; set; }

        public int PlayerMoves { get; set; }

        public AILevel AILevel { get; set; }

        public TimeSpan WaitBeforeMove { get; set; }

        public PlayerColor PlayerColor { get; set; }

        public Player GetPlayer(PlayerColor color)
        {
            if (this.PlayerColor == color)
                return new HumanPlayer();
            //return new ComputerPlayer(AILevel.Hard);

            return new ComputerPlayer(this.AILevel < AILevel.Easy ? AILevel.Medium : this.AILevel);
        }

        public override TestOrchestrator GetOrchestrator(BoardControl boardControl)
        {
            throw new NotImplementedException();
        }
    }
}