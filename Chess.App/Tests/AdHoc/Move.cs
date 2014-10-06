using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.AdHoc
{
    public class AdHocPieceMove
    {
        public AdHocPieceMove(Square from, Square to)
        {
            this.From = from;
            this.To = to;
        }

        public Square From { get; set; }
        public Square To { get; set; }
    }
}
