﻿using Chess.IO;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Chess.App
{
    public partial class ChessForm : Form
    {
        private SpectatorServer spectatorServer;

        public ChessForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            var type = typeof(Sensors.TobiiEyeTracker.EyeTrackerSensor);
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;

            this.menuStrip1.Visible = false;
            this.WindowState = FormWindowState.Maximized;
            Screen myScreen = Screen.FromControl(this);
            this.boardCanvas.Size = new Size(800, 800);
            this.boardCanvas.Location = new Point((myScreen.WorkingArea.Width - boardCanvas.Width) / 2, (myScreen.WorkingArea.Height - boardCanvas.Height) / 2 + 10);

            /*
            var tests = System.IO.Directory.EnumerateFiles("AdHocTests", "*.test").Select(i => new AdHocTest(System.IO.File.OpenRead(i))).ToList();
            foreach (var test in tests.OrderBy(i => i.Name.ToLower()))
            {
                var menuItem = this.testsToolStripMenuItem.DropDownItems.Add(test.Name);
                menuItem.Tag = test;
                menuItem.Click += menuItem_Click;
            }*/

            var result = this.OpenDialog<ParticipantsForm>();
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            var test = ((ToolStripItem)sender).Tag as AdHocTest;
            this.boardCanvas.Visible = true;
            this.boardCanvas.ShowMessage(test.Instruction, closeAction: () => CreateNewGame(test));
        }

        private void CreateNewGame(AdHocTest test)
        {
            int moves = test.PlayerMoves;
            if (moves == 0)
                moves = int.MaxValue;

            var board = this.boardCanvas.StartNew(test.GetPlayer(PlayerColor.White), test.GetPlayer(PlayerColor.Black), test.StartingFEN);
            this.boardCanvas.Board.Start();
            this.boardCanvas.Board.PieceMoved += move => { if (board.CurrentPlayer.PlayerColor == PlayerColor.White) return; moves--; if (moves == 0) board.Stop(); };
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            this.menuStrip1.Visible = e.Y < 30;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var config = NewGameForm.Show(this);
            if (config == null)
                return;

            if (spectatorServer != null)
                ((IDisposable)spectatorServer).Dispose();

            var board = this.boardCanvas.StartNew(config.White, config.Black);
            spectatorServer = new SpectatorServer(board);

            var mouseSensor = new Sensors.MouseSensor(this.boardCanvas);
            var eyeTrackerSensor = new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardCanvas);
            var sensorContainer = new Sensors.SensorContainer(board, mouseSensor, eyeTrackerSensor);

            var writer = new IO.ChessStreamWriter(board, sensorContainer, DateTime.Now.ToString("yyyMMddmmss") + ".chess");

            if (config.White.IsReady && config.Black.IsReady)
                board.Start();
            else
                this.boardCanvas.ShowMessage("Waiting for remote player", board.Start, () => config.White.IsReady && config.Black.IsReady);
        }

        private void fromNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var spectator = new SpectatorClient(ServerDiscovery.FindServers().First().Address);
            CreateNewGame(spectator);
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog() { Filter = "Chess File|*.chess", InitialDirectory = Environment.CurrentDirectory };
            var result = dlg.ShowDialog(this);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var spectator = new SpectatorClient(dlg.OpenFile());
                CreateNewGame(spectator);
            }
        }

        private void CreateNewGame(SpectatorClient spectator)
        {
            var board = this.boardCanvas.StartNew(new Player(), new Player(), spectator.StartingFEN);
            board.Start();
            new System.Threading.Thread(() =>
            {
                ChessAction action = null;
                do
                {
                    try
                    {
                        action = spectator.ReadAction();
                        this.Invoke((Delegate)(Action<Board, ChessAction>)ProcessAction, board, action);
                    }
                    catch
                    {
                        //MessageBox.Show("The server stop responding.");
                        //return;
                        continue;
                    }
                }
                while (!(action is EndGameAction));
            }) { IsBackground = true }.Start();
        }

        private void ProcessAction(Board board, ChessAction action)
        {
            if (action is MoveAction)
            {
                var move = (MoveAction)action;
                if (!board.Move(move.Source, move.Target, move.PromotePawnTo))
                {
                    throw new InvalidOperationException("Bad message.");
                }
            }

            if (action is SensorAction)
            {
                var sensorAction = action as SensorAction;
                var mouseData = sensorAction.Data as Chess.Sensors.MouseSensorData;
                if (mouseData != null)
                    this.boardCanvas.SetMousePosition(mouseData.Location);

                var eyeTracker = sensorAction.Data as Chess.Sensors.TobiiEyeTracker.EyeTrackerSensorData;
                if (eyeTracker != null)
                    this.boardCanvas.SetEyePosition(eyeTracker.LeftPosition);

            }
        }
    }
}