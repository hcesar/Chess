using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO.VolkeTest
{
    public class VolkeMouseSensor : VolkeMessage
    {
        public Point Point { get; private set; }

        public VolkeMouseSensor(long timeFrame, System.IO.BinaryReader reader)
            : base(timeFrame)
        {
            int x = reader.ReadInt16();
            int y = reader.ReadInt16();
            this.Point = new Point(x, y);
        }
    }
}
