using System;

namespace Chess.IO
{
    public class EndGameAction : ChessAction
    {
        private StalemateReason stalemateReason;
        private PlayerColor looser;
        private bool hasCheckmate;

        public EndGameAction(StalemateReason reason)
        {
            this.stalemateReason = reason;
            this.hasCheckmate = false;
        }

        public EndGameAction(PlayerColor looser)
        {
            this.looser = looser;
            this.hasCheckmate = true;
        }

        public override byte OpCode
        {
            get { return 4; }
        }

        public override void Write(ChessStreamWriter chessWriter, System.IO.BinaryWriter writer)
        {
            writer.Write(this.hasCheckmate);
            writer.Write((this.hasCheckmate ? (byte)this.looser : (byte)this.stalemateReason));
        }

        public EndGameAction(System.IO.BinaryReader reader)
        {
            this.hasCheckmate = reader.ReadBoolean();
            if (this.hasCheckmate)
                this.looser = (PlayerColor)reader.ReadByte();
            else
                this.stalemateReason = (StalemateReason)reader.ReadByte();
        }

        public override string ToString()
        {
            return string.Format("EndGame: {0}", this.hasCheckmate ? this.looser.Opponent() + " wins" : "Stalemate: " + this.stalemateReason.GetDescription());
        }
    }
}