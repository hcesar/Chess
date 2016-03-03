using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO.VolkeTest
{
    public class VolkeQuestionMessage : VolkeMessage
    {
        public VolkeQuestionMessage(long timeFrame, System.IO.BinaryReader reader)
            : base(timeFrame)
        {
            int x = reader.ReadInt16();
            int y = reader.ReadInt16();
            int size = reader.ReadInt32();
            var imageBytes = reader.ReadBytes(size);

            this.Point = new Point(x, y);
            this.Image = Image.FromStream(new MemoryStream(imageBytes));
        }

        public Image Image { get; private set; }
        public Point Point { get; private set; }
    }
}
