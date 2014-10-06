using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Tests.Volke
{
    public class VolkeTestOrchestrator : TestOrchestrator
    {
        private VolkeTestResult result;
        private VolkeTest test;

        private IEnumerator<VolkeTestItem> testItems;
        private CurrentTest currentTest;

        public VolkeTestOrchestrator(BoardControl boardControl, VolkeTest test)
            : base(boardControl)
        {
            this.test = test;
            this.testItems = test.Items.GetEnumerator();
        }

        public override void Start()
        {
            this.result = new VolkeTestResult();

            this.BoardControl.Visible = true;
            this.BoardControl.ShowMessage(this.test.Instruction, NextTest);

            var form = this.BoardControl.Parent;
            this.BoardControl.KeyDown += VolkeTestOrchestrator_KeyDown;
        }

        private void NextTest()
        {
            this.BoardControl.Clear();

            if (!this.testItems.MoveNext())
            {
                this.Finish(this.result);
            }
            else
            {
                this.BoardControl.ShowMessage(this.testItems.Current.Question, () => StartTest(this.currentTest));
            }
        }

        private void StartTest(CurrentTest currentTest)
        {
            this.currentTest = new CurrentTest(this.testItems.Current, this.BoardControl);
        }

        void VolkeTestOrchestrator_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var key = char.ToLower((char)e.KeyValue);
            if (key != 's' && key != 'n')
                return;

            this.BoardControl.Clear();
            this.currentTest.Dispose();
            string isCorrect = !(this.testItems.Current.IsCorrect ^ key == 's') ? "True" : "False";
            result.VolkeTests.Add(new TestResult { Name = this.testItems.Current.Id, RecordFile = this.currentTest.RecordFile, Result = isCorrect, Elapsed = currentTest.Stopwatch.ElapsedMilliseconds });

            NextTest();
        }

        private class CurrentTest : IDisposable
        {
            private IO.ChessStreamWriter writer;
            private Sensors.SensorContainer sensorContainer;
            public VolkeTestItem Test { get; set; }
            public string RecordFile { get; private set; }
            public System.Diagnostics.Stopwatch Stopwatch { get; set; }


            public CurrentTest(VolkeTestItem test, BoardControl boardControl)
            {
                this.Stopwatch = System.Diagnostics.Stopwatch.StartNew();
                this.Test = test;
                var board = boardControl.StartNew(new Player(), new Player(), test.FEN);

                var mouseSensor = new Sensors.MouseSensor(boardControl);
                this.RecordFile = "recorded-files\\" + Guid.NewGuid() + ".chess";
                this.sensorContainer = new Sensors.SensorContainer(board, mouseSensor);
                this.writer = new IO.ChessStreamWriter(board, sensorContainer, this.RecordFile);

            }

            public void Dispose()
            {
                this.sensorContainer.Stop();
                this.writer.Dispose();
            }

        }
    }
}