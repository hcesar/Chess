using Chess.App.Tests;
using Chess.IO;
using Chess.Sensors;
using Chess.Sensors.TobiiEyeTracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App
{
    public partial class SidePlayer : Form
    {
        private Random random = new Random();
        public BoardControl BoardControl { get { return this.boardControl; } }

        public SidePlayer()
        {
            InitializeComponent();
            //this.Enabled = false;
            this.boardControl.BackColor = System.Drawing.Color.Black;
            this.KeyPreview = true;
        }

        internal void StartPlayer(BoardControl sourceBoardControl, Sensors.SensorContainer sensorContainer)
        {
            sw = System.Diagnostics.Stopwatch.StartNew();
            sensorContainer.SensorDataReceived += sensorContainer_SensorDataReceived;

            sourceBoardControl.GameStarted += sourceBoardControl_GameStarted;
            sourceBoardControl.MessageShowed += sourceBoardControl_MessageShowed;
            try
            {
                this.Invoke(this.boardControl.Show);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            this.KeyDown -= SidePlayer_KeyDown;
            this.KeyDown += SidePlayer_KeyDown;
        }

        void SidePlayer_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if (key == Keys.S || key == Keys.N || key == Keys.Space)
            {
                e.Handled = true;
                this.testContainer.Orchestrator.BoardControl.FindForm().Invoke(() => this.testContainer.Orchestrator.SendKey(key), true);
            }
        }

        void sourceBoardControl_GameStarted(Board board)
        {
            this.Invoke(() =>
               {
                   this.boardControl.StartNew(new Player(), new Player(), board.StartFen);
               });
        }

        void sourceBoardControl_MessageShowed(string message)
        {
            this.Invoke(() =>
            {
                this.boardControl.Clear();
                this.boardControl.ShowMessage(message);
            });
        }

        System.Diagnostics.Stopwatch sw;

        void sensorContainer_SensorDataReceived(Sensors.Sensor sensor, Sensors.SensorData data)
        {
            if (data is MouseSensorData)
                this.Invoke(() => this.boardControl.SetMousePosition(((MouseSensorData)data).Location));

            if (data is EyeTrackerSensorData)
                ShowEyeSensor(((EyeTrackerSensorData)data).LeftPosition, ((EyeTrackerSensorData)data).RightPosition);
        }

        private LookPoint.Filters.WeightOOTriangular filter = new LookPoint.Filters.WeightOOTriangular();
        private int count = 0;
        private TestContainer testContainer;
        private void ShowEyeSensor(Point point1, Point point2)
        {
            if (sw.ElapsedMilliseconds < 100)
                return;
            sw.Restart();
            var x = (point1.X + point2.X) / 2;
            var y = (point1.Y + point2.Y) / 2;

            var fp = filter.feed(++count * 20, new PointF((float)x, (float)y));
            var point = new Point((int)fp.X, (int)fp.Y);
            this.Invoke(() => this.boardControl.SetEyePosition(point));
        }

        protected override void OnClosed(EventArgs e)
        {
            System.Environment.Exit(0);
            base.OnClosed(e);
        }

        public ChessForm ChessForm { get; set; }

        private void btnNewTest_Click(object sender, EventArgs e)
        {
            var participant = this.OpenDialog<ParticipantsForm, Participant>();
            if (!Directory.Exists("recorded-files"))
                Directory.CreateDirectory("recorded-files");

            if (participant == null)
                return;

            this.testContainer = this.ChessForm.Invoke<TestContainer>(() => this.ChessForm.StartTest(participant));
            this.StartPlayer(this.ChessForm.Controls.Cast<Control>().Where(i => i is BoardControl).Cast<BoardControl>().First(), this.testContainer.SensorContainer);
            this.Focus();
        }

        private void btnCalibration_Click(object sender, EventArgs e)
        {
            Process.Start("calibration.exe");
        }
        /*
        public static bool MoveToMonitor(IntPtr windowHandle, int monitor)
        { 
            monitor = monitor - 1;
            return SetWindowPos(windowHandle, IntPtr.Zero, Screen.AllScreens[monitor].WorkingArea.Left,
                Screen.AllScreens[monitor].WorkingArea.Top, 1000, 800, /*SetWindowPosFlags.SWP_NOZORDER* / 0x0004 | /*SetWindowPosFlags.SWP_NOREDRAW* / 0x0008);
        }

        [DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
			SetWindowPosFlags uFlags);*/

        private new void Focus()
        {
            if (Application.OpenForms[this.Name] == null)
            {
                this.Show();
            }
            else
            {
                Application.OpenForms[this.Name].Focus();
            }
        }
    }
}
