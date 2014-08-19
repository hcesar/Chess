using System.IO;

namespace Chess.IO
{
    public abstract class ChessAction
    {
        public ChessAction()
        {
            this.TimeDelay = -1;
        }

        public abstract byte OpCode { get; }

        public int TimeDelay { get; set; }

        public abstract void Write(ChessStreamWriter chessWriter, BinaryWriter writer);
    }
}