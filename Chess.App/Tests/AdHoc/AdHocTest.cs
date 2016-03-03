using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App.Tests.AdHoc
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

        public override TestOrchestrator GetOrchestrator(Form parentForm, BoardControl boardControl, Sensors.SensorContainer sensorContainer)
        {
            return new AdHocOrchestrator(parentForm, boardControl, this);
        }

        public override TestOrchestrator GetOrchestrator(Form parentForm, BoardControl boardControl, int idParticipant, Sensors.SensorContainer sensorContainer)
        {
            return new AdHocOrchestrator(parentForm, boardControl, this);
        }
    }
}