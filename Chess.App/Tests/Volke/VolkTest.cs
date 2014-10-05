using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.Volk
{
    public class VolkeTest : Test
    {
        public IList<VolkeTestItem> Items { get; private set; }

        public VolkeTest()
        {
            var file = new FileInfo("tests.volke.xml");
            this.Items = file.ToEntityList<VolkeTestItem>("/Tests/*");
        }

        public override TestOrchestrator GetOrchestrator(BoardControl boardControl)
        {
            return new VolkeTestOrchestrator(boardControl, this);
        }
    }   
}
