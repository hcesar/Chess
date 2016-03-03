using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App.Tests.Volke
{
    public class VolkePlayer
    {
        private BoardControl boardControl;
        private BoardControl boardControlSidePlayer;
        private BinaryReader reader;
        private Timer timer;
        private int lastMessage = 0;

        public event Action<VolkePlayer> End;

        public VolkePlayer(BoardControl boardControl, BoardControl boardControlSidePlayer)
        {
            this.boardControl = boardControl;
            this.boardControl.Clear();
            this.boardControl.Visible = true;
            this.boardControlSidePlayer = boardControlSidePlayer;

            
                this.boardControlSidePlayer.Clear();
                this.boardControlSidePlayer.Visible = true;
           

            timer = new Timer();
            timer.Tick += ProcessMessage;
            timer.Enabled = true;
        }

        public void Play(string file)
        {
            var ms = new MemoryStream(File.ReadAllBytes(file));
            reader = new BinaryReader(ms);
            ReadMessage();
        }

        int eyeTrackingMessageCount = 0;
        private void ReadMessage()
        {
            if (reader.BaseStream.Length == reader.BaseStream.Position)
            {
                if (this.End != null)
                    this.End(this);

                return;
            }
            //int div = 3;
            // if (lastMessage < 60 * 1000 || (lastMessage > 61 * 1000 && lastMessage < 110 * 1000))
            //    div = 100;

            int waitTime = (int)reader.ReadInt64();
            int interval = (waitTime - lastMessage) / 1;
            lastMessage = waitTime;

            //Console.Write("\r" + waitTime);

            var messageType = (VolkeMessageType)reader.ReadByte();
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            if (messageType == VolkeMessageType.EyeSensorData)
            {
                eyeTrackingMessageCount++;

                if (eyeTrackingMessageCount == 50)
                {
                    eyeTrackingMessageCount = 0;
                }

                if (eyeTrackingMessageCount == 0)
                {
                    timer.Interval = 1;
                    timer.Start();
                    return;
                }
                else
                {
                    ProcessMessage(this, EventArgs.Empty);
                    return;
                }
            }

            if (interval == 0)
            {
                ProcessMessage(this, EventArgs.Empty);
            }
            else
            {
                timer.Interval = interval;
                timer.Start();
            }
        }
        private void ProcessMessage(object sender, EventArgs e)
        {
            timer.Stop();
            var messageType = (VolkeMessageType)reader.ReadByte();

            switch (messageType)
            {
                case VolkeMessageType.TestStart:
                    ShowBoard();
                    break;
                case VolkeMessageType.MouseSensorData:
                    ShowMouseSensor();
                    break;
                case VolkeMessageType.EyeSensorData:
                    ShowEyeSensor();
                    break;
                case VolkeMessageType.Question:
                    ShowQuestion();
                    break;
                default:
                    throw new NotSupportedException();
            }
            ReadMessage();
        }

        private void ShowQuestion()
        {
            Console.WriteLine("Question");
            int x = reader.ReadInt16();
            int y = reader.ReadInt16();
            int size = reader.ReadInt32();
            var imageBytes = reader.ReadBytes(size);

            this.boardControl.Clear();
            this.boardControlSidePlayer.Clear();

            var point = new Point(x, y);
            var image = Image.FromStream(new MemoryStream(imageBytes));

            this.boardControl.Image = new Bitmap(this.boardControl.Width, this.boardControl.Height);
            using (var g = this.boardControl.Image.GetGraphics())
            {
                g.Graphics.DrawImage(image, point);
            }

            using (var g = this.boardControlSidePlayer.Image.GetGraphics())
            {
                g.Graphics.DrawImage(image, point);
            }
        }

        private void ShowMouseSensor()
        {
            int x = reader.ReadInt16();
            int y = reader.ReadInt16();
            //this.boardControl.SetMousePosition(new Point(x, y));
        }

        private System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
        private LookPoint.Filters.WeightOOTriangular filter = new LookPoint.Filters.WeightOOTriangular();
        private int count = 0;
        private void ShowEyeSensor()
        {
            var point1 = new Point(reader.ReadInt16(), reader.ReadInt16());
            var point2 = new Point(reader.ReadInt16(), reader.ReadInt16());

            if (stopWatch.ElapsedMilliseconds < 50)
                return;

            var x = (point1.X + point2.X) / 2;
            var y = (point1.Y + point2.Y) / 2;

            var fp = filter.feed(++count * 20, new PointF((float)x, (float)y));
            var point = new Point((int)fp.X, (int)fp.Y);
            stopWatch.Restart();
            this.boardControlSidePlayer.SetEyePosition(point);
        }

        private void ShowBoard()
        {
            var fen = reader.ReadString();
            this.boardControl.StartNew(new HumanPlayer(), new HumanPlayer(), fen);
            this.boardControlSidePlayer.StartNew(new HumanPlayer(), new HumanPlayer(), fen);
        }
    }
}