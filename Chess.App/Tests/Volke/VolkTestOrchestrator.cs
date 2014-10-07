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
        private System.Diagnostics.Stopwatch stopWatch;

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
            this.BoardControl.ShowMessage(this.test.Instruction, () => { result.StartDate = DateTime.Now; this.NextTest(); });

            var form = this.BoardControl.Parent;
            this.BoardControl.KeyDown += VolkeTestOrchestrator_KeyDown;
        }

        private void NextTest()
        {
            this.stopWatch = System.Diagnostics.Stopwatch.StartNew();
            this.BoardControl.Clear();

            if (!this.testItems.MoveNext())
            {
                this.result.Elapsed = (long)(DateTime.Now - this.result.StartDate).TotalMilliseconds;
                this.Finish(this.result);
            }
            else
            {
                DateTime now = DateTime.Now;
                this.BoardControl.ShowMessage(this.testItems.Current.Question, () => StartTest(this.currentTest, now));
            }
        }

        private void StartTest(CurrentTest currentTest, DateTime startDate)
        {
            this.currentTest = new CurrentTest(this.testItems.Current, this.BoardControl, this.stopWatch.ElapsedMilliseconds, startDate);
        }

        void VolkeTestOrchestrator_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var key = char.ToLower((char)e.KeyValue);
            if (key != 's' && key != 'n')
                return;

            this.BoardControl.Clear();
            this.currentTest.Dispose();
            string isCorrect = !(this.testItems.Current.IsCorrect ^ key == 's') ? "True" : "False";
            var itemResult = new TestResult
            {
                Id = this.testItems.Current.Id,
                StartDate = currentTest.StartDate,
                RecordFile = this.currentTest.RecordFile,
                Result = isCorrect,
                Elapsed = currentTest.QuestionTime,
                Elapsed2 = currentTest.Stopwatch.ElapsedMilliseconds
            };

            result.VolkeTests.Add(itemResult);

            NextTest();
        }

        private class CurrentTest : IDisposable
        {
            private IO.ChessStreamWriter writer;
            private Sensors.SensorContainer sensorContainer;
            public VolkeTestItem Test { get; set; }
            public string RecordFile { get; private set; }
            public System.Diagnostics.Stopwatch Stopwatch { get; set; }
            public long QuestionTime { get; set; }
            public DateTime StartDate { get; set; }


            public CurrentTest(VolkeTestItem test, BoardControl boardControl, long questionTime, DateTime startDate)
            {
                this.StartDate = startDate;
                this.QuestionTime = questionTime;
                this.Stopwatch = System.Diagnostics.Stopwatch.StartNew();
                this.Test = test;
                var board = boardControl.StartNew(new Player(), new Player(), test.FEN);

                var mouseSensor = new Sensors.MouseSensor(boardControl);
                this.RecordFile = "recorded-files\\" + Guid.NewGuid() + ".chess";
                this.sensorContainer = new Sensors.SensorContainer(board, mouseSensor, new Sensors.TobiiEyeTracker.EyeTrackerSensor(boardControl));
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