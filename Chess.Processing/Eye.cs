using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Processing
{
    [Flags]
    public enum Eye
    {
        Left = 1,
        Right = 2,
        Both = 3,
        Average = 4
    }
}
