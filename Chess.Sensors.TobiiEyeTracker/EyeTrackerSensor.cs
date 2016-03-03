using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace Chess.Sensors.TobiiEyeTracker
{
    public class EyeTrackerSensor : Sensor
    {
        Stopwatch sw = Stopwatch.StartNew();
        public static bool IsOnline { get; private set; }
        private static EyeTracker tracker;
        private Control boardControl;

        public EyeTrackerSensor(Control boardControl)
        {
            this.boardControl = boardControl;
            new Thread(Start).Start();
        }

        private void Start()
        {
            try
            {
                tracker = new EyeTracker(new Uri("tet-tcp://TX300-010104404713.local."));

                //new Thread(tracker.RunEventLoop) { IsBackground = true }.Start();
                tracker.RunEventLoopOnInternalThread(new CompletionStatusCallback((err) => { Console.WriteLine(err); }));

                tracker.Connect();
                tracker.StartTracking();

                tracker.EyeTrackerError += EyeTrackerError;
                tracker.GazeData += EyeTrackerGazeData;

                IsOnline = true;
            }
            catch
            {
                IsOnline = false;
            }
        }



        private void EyeTrackerGazeData(object sender, GazeDataEventArgs e)
        {
            var gazeData = e.GazeData;

            var point1 = e.GazeData.Left.GazePointOnDisplayNormalized;
            var point2 = e.GazeData.Right.GazePointOnDisplayNormalized;
            var p1 = new Point((int)(point1.X * Screen.PrimaryScreen.WorkingArea.Width), (int)(point1.Y * Screen.PrimaryScreen.WorkingArea.Height));
            var p2 = new Point((int)(point2.X * Screen.PrimaryScreen.WorkingArea.Width), (int)(point2.Y * Screen.PrimaryScreen.WorkingArea.Height));

            //var p1 = new Point((int)gazeData.Left.GazePointOnDisplayNormalized.X, (int)gazeData.Left.GazePointOnDisplayNormalized.Y);
            p1 = (Point)this.boardControl.Invoke((Delegate)(Func<object>)(() => this.boardControl.PointToClient(p1)));
            p2 = (Point)this.boardControl.Invoke((Delegate)(Func<object>)(() => this.boardControl.PointToClient(p2)));

            //var p2 = new Point((int)gazeData.Right.GazePointOnDisplayNormalized.X, (int)gazeData.Right.GazePointOnDisplayNormalized.Y);
            //p2 = (Point)this.boardControl.Invoke((Delegate)(Func<object>)(() => this.boardControl.PointToClient(p2)));

            //System.Diagnostics.Debug.Write("\r"+ p1);

            this.OnDataAvailable(new EyeTrackerSensorData(p1, p2));
        }

        private void EyeTrackerError(object sender, EyeTrackerErrorEventArgs e)
        {
            Console.WriteLine(e.Message + ":" + e.ErrorCode);
        }

        public static SensorData Deserialize(System.IO.BinaryReader reader)
        {
            return new EyeTrackerSensorData(new Point(reader.ReadInt16(), reader.ReadInt16()), new Point(reader.ReadInt16(), reader.ReadInt16()));
        }
    }

    public class EyeTrackerSensorData : SensorData
    {
        public Point LeftPosition { get; private set; }
        public Point RightPosition { get; private set; }

        public EyeTrackerSensorData(Point leftPosition, Point rightPosition)
        {
            this.LeftPosition = leftPosition;
            this.RightPosition = rightPosition;
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write((short)this.LeftPosition.X);
            writer.Write((short)this.LeftPosition.Y);
            writer.Write((short)this.RightPosition.X);
            writer.Write((short)this.RightPosition.Y);
        }

        public override string ToString()
        {
            return this.LeftPosition.ToString() + " - " + this.RightPosition;
            //return new Point(this.X, this.Y).ToString();
        }

        public override Type GetSensorType()
        {
            return typeof(EyeTrackerSensor);
        }
    }
}
