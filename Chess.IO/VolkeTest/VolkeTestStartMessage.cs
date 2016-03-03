using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO.VolkeTest
{
    public class VolkeTestStartMessage : VolkeMessage
    {
        public string FEN { get; private set; }

        public VolkeTestStartMessage(long timeFrame, System.IO.BinaryReader reader)
            : base(timeFrame)
        {
            this.FEN = reader.ReadString();
        }
    }
}
