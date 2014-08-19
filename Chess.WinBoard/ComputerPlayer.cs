using System;

namespace Chess
{
    public class ComputerPlayer : Player
    {
        private WinBoard engine;
        private AILevel aiLevel;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public ComputerPlayer(AILevel level = AILevel.Medium)
        {
            this.aiLevel = level;
        }

        public override void OnInitialize()
        {
            engine = new WinBoard(this.PlayerColor, this.aiLevel, this.Board.StartFen, "stockfish");
            engine.OnMove += engine_OnMove;
            //engine.OnGameEnded += () => { if (this.Board.Turn == Chess.PlayerColor.Black || this.Board.Turn == Chess.PlayerColor.White) throw new InvalidOperationException("Board did not recognize the game ending."); };
            engine.OnStalemate += (reason) => { this.Board.OnStalemate(reason); };
            engine.OnResign += () => { this.Board.Resign(); };
            engine.Start();
        }

        private void engine_OnMove(Square source, Square target, Type pawnPromotedTo)
        {
            if (stopwatch.Elapsed.TotalSeconds < 1)
                System.Threading.Thread.Sleep((int)(1000 - stopwatch.ElapsedMilliseconds));

            this.Board.Move(source, target, pawnPromotedTo);
        }

        protected override void OnTurnCore()
        {
            var lastMove = this.Board.LastMove;
            if (lastMove != null)
            {
                stopwatch.Restart();
                var promotion = lastMove.HasPromotion ? Piece.GetNotation(lastMove.PawnPromotedTo) : "";
                engine.Move(lastMove.Source.ToString().ToLower() + lastMove.Target.ToString().ToLower() + promotion);
            }
        }

        protected override void OnDispose()
        {
            this.engine.Dispose();
        }
    }
}