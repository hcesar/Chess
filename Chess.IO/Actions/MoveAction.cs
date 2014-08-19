using System;
using System.IO;

namespace Chess.IO
{
    public class MoveAction : ChessAction
    {
        public Square Source { get; private set; }

        public Square Target { get; private set; }

        public Type PromotePawnTo { get; private set; }

        public override byte OpCode
        {
            get { return 2; }
        }

        public MoveAction(Square source, Square target, Type promotePawnTo)
        {
            this.Source = source;
            this.Target = target;
            this.PromotePawnTo = promotePawnTo;
        }

        public override void Write(ChessStreamWriter chessWriter, BinaryWriter writer)
        {
            writer.Write((byte)this.Source);
            writer.Write((byte)this.Target);

            if (this.PromotePawnTo != null)
            {
                writer.Write(true);
                writer.Write(this.PromotePawnTo.AssemblyQualifiedName);
            }
            else
                writer.Write(false);
        }

        public MoveAction(BinaryReader reader)
        {
            this.Source = (Square)reader.ReadByte();
            this.Target = (Square)reader.ReadByte();
            bool hasPromotion = reader.ReadBoolean();
            if (hasPromotion)
            {
                string pieceType = reader.ReadString();
                this.PromotePawnTo = Type.GetType(pieceType);
            }
            else if (Source == Square.A2 && Target == Square.A1)
            {
                this.PromotePawnTo = typeof(Pieces.Queen);
            }
        }

        public override string ToString()
        {
            return string.Format("Move: {0}{1}", this.Source, this.Target);
        }
    }
}