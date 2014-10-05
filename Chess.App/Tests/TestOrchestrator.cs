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

        public TestOrchestrator(BoardControl boardControl)
        {
            this.BoardControl = boardControl;
        }


        public abstract void Start();
    }
}
