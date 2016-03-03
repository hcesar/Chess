using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App.Tests.Volke
{
    public class VolkeTest : Test
    {
        private string testFile;

        public string TestsFile
        {
            get { return this.testFile; }
            set
            {
                this.testFile = value;
                var file = new FileInfo(value);
                this.Items = Randomize(file.ToEntityList<VolkeTestItem>("/Tests/*"));
            }
        }

        private IList<VolkeTestItem> Randomize(IList<VolkeTestItem> list)
        {
            return list;
        }
        public IList<VolkeTestItem> Items { get; private set; }

        public VolkeTest()
        {
        }

        public override TestOrchestrator GetOrchestrator(Form parentForm, BoardControl boardControl, Sensors.SensorContainer sensorContainer)
        {
            return new VolkeTestOrchestrator(parentForm, boardControl, this, 0, sensorContainer);
        }

        public override TestOrchestrator GetOrchestrator(Form parentForm, BoardControl boardControl, int idParticipant, Sensors.SensorContainer sensorContainer)
        {
            return new VolkeTestOrchestrator(parentForm, boardControl, this, idParticipant, sensorContainer);
        }


    }
}
