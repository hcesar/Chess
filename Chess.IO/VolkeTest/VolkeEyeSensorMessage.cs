using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO.VolkeTest
{
    public class VolkeEyeSensorMessage : VolkeMessage
    {
        public VolkeEyeSensorMessage(long timeFrame, System.IO.BinaryReader reader)
            : base(timeFrame)
        {
            short zero = 0;
            this.LeftPosition = new Point((int)Math.Max(reader.ReadInt16(), zero), (int)Math.Max(reader.ReadInt16(), zero));
            this.RightPosition = new Point((int)Math.Max(reader.ReadInt16(), zero), (int)Math.Max(reader.ReadInt16(), zero));
        }

        public VolkeEyeSensorMessage(long timeFrame, Point leftPosition, Point rightPosition)
            : base(timeFrame)
        {
            this.LeftPosition = leftPosition;
            this.RightPosition = rightPosition;
        }

        public Point LeftPosition { get; private set; }
        public Point RightPosition { get; private set; }

        public int X
        {
            get
            {
                int x1 = this.LeftPosition.X == 0 ? this.RightPosition.X : this.LeftPosition.X;
                int x2 = this.RightPosition.X == 0 ? this.LeftPosition.X : this.RightPosition.X;

                return (x1 + x2) / 2;
            }
        }

        public int Y
        {
            get
            {
                int y1 = this.LeftPosition.Y == 0 ? this.RightPosition.Y : this.LeftPosition.Y;
                int y2 = this.RightPosition.Y == 0 ? this.LeftPosition.Y : this.RightPosition.Y;

                return (y1 + y2) / 2;
            }
        }
    }
}
