using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.Volke
{
    public class VolkeTestResult : TestResult
    {
        public VolkeTestResult()
        {
            this.Result = new List<TestResult>();
        }

        public new IList<TestResult> Result { get; private set; }
    }
}
