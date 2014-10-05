using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.Volk
{
    public class VolkeTestItem
    {
        public string FEN { get; set; }
        public string Question { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionLevel { get; set; }
    }
}
