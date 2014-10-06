using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests
{
    public abstract class TestOrchestrator
    {
        public BoardControl BoardControl { get; private set; }
        public event EventHandler<TestResult> Finished;

        public TestOrchestrator(BoardControl boardControl)
        {
            this.BoardControl = boardControl;
        }


        public abstract void Start();

        protected void Finish(TestResult result)
        {
            if (this.Finished != null)
                this.Finished(this, result);
        }
    }
}
