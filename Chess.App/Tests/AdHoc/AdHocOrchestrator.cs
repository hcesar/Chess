using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.AdHoc
{
    class AdHocOrchestrator : TestOrchestrator
    {
        private AdHocTest test;
        private string fileName; 
        private System.Diagnostics.Stopwatch stopWatch;
        private Sensors.SensorContainer sensorContainer;
        private IO.ChessStreamWriter writer;
        private DateTime startDate;

        public AdHocOrchestrator(BoardControl control, AdHocTest test) : base(control)
        {
            this.startDate = DateTime.Now;
            this.test = test;
        }

        public override void Start()
        {
            this.BoardControl.ShowMessage(this.test.Instruction, StartInternal);
        }

        public void StartInternal()
        {
            int moves = test.PlayerMoves;
            if (moves == 0)
                moves = int.MaxValue;

            var board = this.BoardControl.StartNew(test.GetPlayer(PlayerColor.White), test.GetPlayer(PlayerColor.Black), test.FEN);

            var mouseSensor = new Sensors.MouseSensor(this.BoardControl);
            this.sensorContainer = new Sensors.SensorContainer(board, mouseSensor, new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.BoardControl));
            this.fileName = "recorded-files\\" + Guid.NewGuid() + ".chess";
            this.writer = new IO.ChessStreamWriter(board, sensorContainer, fileName);

            board.PieceMoved += move => { if (board.CurrentPlayer.PlayerColor == PlayerColor.White) return; moves--; if (moves == 0) this.Finish(board); };
            board.Start();
            this.stopWatch = System.Diagnostics.Stopwatch.StartNew();
        }

        private void Finish(Board board)
        {
            this.sensorContainer.Stop();
            this.writer.Dispose();

            var moves = board.History.Reverse().Select(i => new AdHocPieceMove(i.Source, i.Target)).ToList();
            this.Finish(new AdHocTestResult(moves) { RecordFile = this.fileName, Elapsed = this.stopWatch.ElapsedMilliseconds, StartDate = startDate });
        }
    }
}
