using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chess.Sensors;
using System.Drawing;
using System.Drawing.Imaging;

namespace Chess.App.Tests.Volke
{
    internal class VolkeRecorder
    {
        private Control boardControl;
        private BinaryWriter writer;
        private Stopwatch stopWatch;
        private SensorContainer sensors;

        public VolkeRecorder(Control boardControl, int participandId, SensorContainer sensors)
        {
            this.stopWatch = Stopwatch.StartNew();
            this.boardControl = boardControl;

            this.writer = new BinaryWriter(File.Create(participandId + ".volke"));
            sensors.SensorDataReceived += (sensor, data) => WriteMessage(data);
            //this.sensor = new Chess.Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardControl);
            //sensor.DataAvailable += data => WriteMessage(data);
        }

        public void StartRecording()
        {
            this.WriteDialog();
        }

        public void NextQuestion()
        {
            this.WriteDialog();
        }

        public void StartTestItem(string fen)
        {
            this.WriteMessage(VolkeMessageType.TestStart, fen);
        }

        public void Finish()
        {
            this.sensors.Stop();
            this.writer.Flush();
            this.writer.Close();
        }
        private System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();


        private void WriteMessage(SensorData data)
        {
            if (data is Sensors.TobiiEyeTracker.EyeTrackerSensorData)
            {
                //if (stopwatch.ElapsedMilliseconds < 10) return;
                //stopwatch.Restart();
            }

            lock (writer)
            {
                VolkeMessageType messageType;

                if (data is MouseSensorData)
                    messageType = VolkeMessageType.MouseSensorData;
                else if (data is Sensors.TobiiEyeTracker.EyeTrackerSensorData)
                    messageType = VolkeMessageType.EyeSensorData;
                else
                    return;

                writer.Write(stopWatch.ElapsedMilliseconds);
                writer.Write((byte)messageType);
                data.Serialize(writer);
                writer.Flush();
            }
        }

        private void WriteMessage(VolkeMessageType messageType, string data)
        {
            lock (writer)
            {
                writer.Write(stopWatch.ElapsedMilliseconds);
                writer.Write((byte)messageType);
                writer.Write(data);
                writer.Flush();
            }
        }

        private void WriteDialog()
        {
            var msg = this.boardControl.Controls.OfType<MessageDialog>().First();
            var bmp = new Bitmap(msg.Width, msg.Height);
            msg.DrawToBitmap(bmp, msg.ClientRectangle);

            lock (writer)
            {
                var point = msg.Location;
                writer.Write(stopWatch.ElapsedMilliseconds);
                writer.Write((byte)VolkeMessageType.Question);
                writer.Write((short)point.X);
                writer.Write((short)point.Y);
                var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                writer.Write((int)ms.Length);
                writer.Write(ms.ToArray());
                writer.Flush();
            }
        }
    }

    internal enum VolkeMessageType : byte
    {
        TestStart = 1,
        MouseSensorData = 2,
        Question = 3,
        EyeSensorData = 4,
    }
}
