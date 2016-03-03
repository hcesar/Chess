using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chess.App.Controls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;

namespace Chess.App.Tests.Volke
{
    public class VolkeTestOrchestrator : TestOrchestrator
    {
        private VolkeTestResult result;
        private VolkeTest test;

        private int idParticipant;

        private IEnumerator<VolkeTestItem> testItems;
        private CurrentTest currentTest;
        private System.Diagnostics.Stopwatch stopWatch;
        private VolkeRecorder recorder;
        private Sensors.SensorContainer sensors;

        public VolkeTestOrchestrator(Form parentForm, BoardControl boardControl, VolkeTest test, int idParticipant, Sensors.SensorContainer sensors)
            : base(parentForm, boardControl)
        {
            this.test = test;
            this.testItems = test.Items.GetEnumerator();
            this.idParticipant = idParticipant;
            this.sensors = sensors;
        }

        public override void Start()
        {
            recorder = new VolkeRecorder(this.BoardControl, this.idParticipant, this.sensors);

            this.result = new VolkeTestResult();

            EEG.SendCommand(EEGCommand.ENABLE, idParticipant.ToString("000"));

            this.BoardControl.Visible = true;
            this.BoardControl.ShowMessage(this.test.Instruction, () => { EEG.SendCommand(EEGCommand.START, idParticipant.ToString("000")); result.StartDate = DateTime.Now; this.NextTest(); }, null, 800, 500);
            recorder.StartRecording();
        }

        private void NextTest()
        {
            this.stopWatch = System.Diagnostics.Stopwatch.StartNew();
            this.BoardControl.Clear();

            if (!this.testItems.MoveNext())
            {
                EEG.SendCommand(EEGCommand.STOP);
                this.result.ElapsedAnswer = (long)(DateTime.Now - this.result.StartDate).TotalMilliseconds;
                EEG.SendCommand(EEGCommand.DISABLE);
                this.Finish(this.result);
                recorder.Finish();
            }
            else
            {
                this.currentTest = null;
                DateTime now = DateTime.Now;
                this.BoardControl.ShowMessage((int.Parse(this.testItems.Current.Id) + 1) + ": " + this.testItems.Current.Question, () => StartTest(this.currentTest, now, this.result.StartDate));
                recorder.NextQuestion();
            }
        }

        private void StartTest(CurrentTest currentTest, DateTime startDate, DateTime testStart)
        {
            recorder.StartTestItem(this.testItems.Current.FEN);
            this.currentTest = new CurrentTest(this.testItems.Current, this.BoardControl, DateTime.Now, startDate, testStart);
        }

        public override void SendKey(Keys key)
        {
            if (key != Keys.S && key != Keys.N && key != Keys.Space)
                return;

            if (key == Keys.Space)
            {
                this.BoardControl.CloseDialog();
                return;
            }

            if (this.currentTest == null)
                return;

            this.BoardControl.Clear();
            this.currentTest.Dispose();
            bool isCorrect = !(this.testItems.Current.IsCorrect ^ key == Keys.S);


            DateTime now = DateTime.Now;
            var itemResult = new TestResult
            {
                Id = this.testItems.Current.Id,
                RecordFile = this.currentTest.RecordFile,
                Result = isCorrect ? "Correct" : "Incorrect",
                StartDate = currentTest.StartDate,
                DateQuestion = currentTest.QuestionDate,
                DateAnswer = now,
                TimestampQuestion = currentTest.QuestionDate.Ticks,
                TimestampAnswer = now.Ticks,
                ElapsedQuestion = (long)(currentTest.QuestionDate - currentTest.overallStartDate).TotalMilliseconds,
                ElapsedAnswer = (long)(now - currentTest.overallStartDate).TotalMilliseconds
            };

            result.VolkeTests.Add(itemResult);
            if (isCorrect)
                result.CorrectAnswers++;

            NextTest();
        }

        private enum MessageType : byte
        {
            Start = 1,
            TestStart = 2,
            SensorData = 3,
            Message = 4,
        }

        private class CurrentTest : IDisposable
        {
            private IO.ChessStreamWriter writer;
            private Sensors.SensorContainer sensorContainer;
            public VolkeTestItem Test { get; set; }
            public string RecordFile { get; private set; }
            public System.Diagnostics.Stopwatch Stopwatch { get; set; }
            public DateTime QuestionDate { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime overallStartDate { get; set; }


            public CurrentTest(VolkeTestItem test, BoardControl boardControl, DateTime questionDate, DateTime startDate, DateTime overallStartDate)
            {
                this.StartDate = startDate;
                this.QuestionDate = questionDate;
                this.overallStartDate = overallStartDate;
                this.Stopwatch = System.Diagnostics.Stopwatch.StartNew();
                this.Test = test;
                var board = boardControl.StartNew(new Player(), new Player(), test.FEN);

                //var mouseSensor = new Sensors.MouseSensor(boardControl);
                this.RecordFile = "recorded-files\\" + Guid.NewGuid() + ".chess";
                this.sensorContainer = new Sensors.SensorContainer();
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