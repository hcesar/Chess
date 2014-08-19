using System.Net;

namespace Chess.IO
{
    public class NetworkPlayer : Player
    {
        private ChessConnection connection;

        public override bool IsReady { get { return this.connection.IsConnected; } }

        public NetworkPlayer(IPAddress server)
            : this(new Client(server))
        {
        }

        public NetworkPlayer(ChessConnection connection)
        {
            this.connection = connection;
            connection.PieceMoved += connection_PieceMoved;
            this.StartingFEN = connection.StartingFEN;
        }

        private void connection_PieceMoved(PieceMove move)
        {
            this.Board.Move(move.Source, move.Target, move.PawnPromotedTo);
        }

        public override void OnPreInit()
        {
            if (this.PlayerColor == Chess.PlayerColor.Black)
                this.connection.StartingFEN = this.Board.GetFEN();

            this.connection.Initialize();
            base.OnPreInit();
        }

        protected override void OnTurnCore()
        {
            var lastMove = this.Board.LastMove;
            if (lastMove != null)
                this.connection.SendMove(lastMove.Source, lastMove.Target, lastMove.PawnPromotedTo);

            base.OnTurnCore();
        }

        public string StartingFEN { get; private set; }
    }
}