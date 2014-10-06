using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.AdHoc
{
    public class AdHocTestResult : TestResult
    {
        public AdHocTestResult()
        {
            this.Moves = new List<AdHocPieceMove>();
        }

        public AdHocTestResult(List<AdHocPieceMove> moves)
        {
            this.Moves = moves;
        }

        public List<AdHocPieceMove> Moves { get; set; }
    }
}
