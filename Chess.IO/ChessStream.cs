using System;
using System.Collections.Generic;

namespace Chess.IO
{
    public class ChessStream : IChessStream, IDisposable
    {
        private System.Collections.Concurrent.ConcurrentQueue<ChessAction> actions = new System.Collections.Concurrent.ConcurrentQueue<ChessAction>();

        public Sensors.SensorContainer Sensors { get; private set; }

        public Board Board { get; private set; }

        public event Action<ChessAction> ActionAvailable;

        public int Count { get { return this.actions.Count; } }

        public ChessStream(Board board, Sensors.SensorContainer sensors)
        {
            this.Board = board;
            this.Board.Checkmate += (player) => this.Enqueue(new EndGameAction(player));
            this.Board.Stalemate += (reason) => this.Enqueue(new EndGameAction(reason));
            this.Sensors = sensors;

            this.Board.PieceMoved += board_PieceMoved;
            if (sensors != null)
                this.Sensors.SensorDataReceived += sensors_SensorDataReceived;

            this.actions.Enqueue(new StartAction(board, sensors));
        }

        private void Enqueue(ChessAction action)
        {
            if (action is EndGameAction)
                this.Sensors.Stop();

            this.actions.Enqueue(action);
            this.OnActionAvailable();
        }

        private void sensors_SensorDataReceived(Sensors.Sensor sensor, Sensors.SensorData data)
        {
            this.Enqueue(new SensorAction(data));
        }

        private void board_PieceMoved(PieceMove move)
        {
            this.Enqueue(new MoveAction(move.Source, move.Target, move.PawnPromotedTo));
        }

        private void OnActionAvailable()
        {
            if (this.ActionAvailable == null)
                return;
            ChessAction action;
            while (this.actions.TryDequeue(out action))
                this.ActionAvailable(action);
        }

        void IDisposable.Dispose()
        {
            this.Sensors.Stop();
        }

        IList<Type> IChessStream.SensorsType
        {
            get { return this.Sensors.GetSensorsType(); }
        }
    }
}