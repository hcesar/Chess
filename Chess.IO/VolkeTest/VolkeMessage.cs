using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.IO
{
    public class VolkeMessage
    {
        public long Timestamp { get; set; }

        public VolkeMessage(long timeFrame)
        {
            this.Timestamp = timeFrame;
        }
    }
}
