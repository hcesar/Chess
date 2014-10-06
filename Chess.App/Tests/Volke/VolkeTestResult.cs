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
            this.VolkeTests = new List<TestResult>();
        }

        public List<TestResult> VolkeTests { get; private set; }
    }
}
