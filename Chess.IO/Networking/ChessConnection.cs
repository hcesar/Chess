using System;

namespace Chess.IO
{
    public abstract class ChessConnection
    {
        internal string StartingFEN { get; set; }

        public event PieceMoved PieceMoved;

        public bool IsConnected { get; protected set; }

        public abstract void SendMove(Square source, Square target, Type piecePromotedTo);

        protected void OnReceiveMove(Square source, Square target, Type piecePromotedTo)
        {
            this.PieceMoved(new PieceMove(source, target, piecePromotedTo));
        }

        public virtual void Initialize()
        {
        }
    }
}