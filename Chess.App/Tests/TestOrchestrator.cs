using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App.Tests
{
    public abstract class TestOrchestrator
    {
        public Form ParentForm { get; private set; }
        public BoardControl BoardControl { get; private set; }
        public event EventHandler<TestResult> Finished;

        public TestOrchestrator(Form parentForm, BoardControl boardControl)
        {
            this.ParentForm = parentForm;
            this.BoardControl = boardControl;
        }


        public abstract void Start();

        protected void Finish(TestResult result)
        {
            if (this.Finished != null)
                this.Finished(this, result);
        }

        public virtual void SendKey(Keys key)
        {
        }
    }
}
