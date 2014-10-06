using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.Volk
{
    public class VolkeTestOrchestrator : TestOrchestrator
    {
        private VolkeTest test;

        public VolkeTestOrchestrator(BoardControl boardControl, VolkeTest test)
            : base(boardControl)
        {
            this.test = test;
        }

        public override void Start()
        {
            this.BoardControl.Visible = true;
            this.BoardControl.ShowMessage(this.test.Instruction, StartTests);
        }

        private void StartTests()
        {
            var currentTest = this.test.Items[0];
            this.BoardControl.SetSideInstruction(currentTest.Question);
            var board = this.BoardControl.StartNew(new Player(), new Player(), currentTest.FEN);
        }
    }

}
