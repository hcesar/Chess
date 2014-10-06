using Chess.App.Tests;
using Chess.App.Tests.AdHoc;
using Chess.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Chess.App
{
    public partial class ChessForm : Form
    {
        private SpectatorServer spectatorServer;
        private IList<Test> tests;
        private Participant currentParticipant;

        public ChessForm()
        {
            this.boardControl = new Chess.App.BoardControl();
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            //var type = typeof(Sensors.TobiiEyeTracker.EyeTrackerSensor);
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;

            this.menuStrip1.Visible = false;
            this.WindowState = FormWindowState.Maximized;
            Screen myScreen = Screen.FromControl(this);
            this.boardControl.Size = new Size(800, 800);
            this.boardControl.Location = new Point((myScreen.WorkingArea.Width - boardControl.Width) / 2, (myScreen.WorkingArea.Height - boardControl.Height) / 2 + 10);

            /*
            var tests = System.IO.Directory.EnumerateFiles("AdHocTests", "*.test").Select(i => new AdHocTest(System.IO.File.OpenRead(i))).ToList();
            foreach (var test in tests.OrderBy(i => i.Name.ToLower()))
            {
                var menuItem = this.testsToolStripMenuItem.DropDownItems.Add(test.Name);
                menuItem.Tag = test;
                menuItem.Click += menuItem_Click;
            }*/

            tests = Test.LoadAll();
            var participant = this.OpenDialog<ParticipantsForm, Participant>();
            currentParticipant = participant;
            if (!Directory.Exists("recorded-files"))
                Directory.CreateDirectory("recorded-files");

            ShowTest();
        }


        private void ShowTest(int index = 0)
        {
            if (this.tests.Count <= index)
            {
                if (index == 0)
                    return;

                Participant.Update(currentParticipant);
                this.boardControl.Clear();
                return;
            }

            var test = this.tests[index];
            var orchestrator = test.GetOrchestrator(this.boardControl);
            orchestrator.Finished += (sender, result) => { currentParticipant.Tests.Add(result); this.ShowTest(index + 1); };
            orchestrator.Start();
        }
             
        #region Legacy
        private void menuItem_Click(object sender, EventArgs e)
        {
            var test = ((ToolStripItem)sender).Tag as AdHocTest;
            this.boardControl.Visible = true;
            this.boardControl.ShowMessage(test.Instruction, closeAction: () => CreateNewGame(test));
        }

        private void CreateNewGame(AdHocTest test)
        {
            int moves = test.PlayerMoves;
            if (moves == 0)
                moves = int.MaxValue;

            var board = this.boardControl.StartNew(test.GetPlayer(PlayerColor.White), test.GetPlayer(PlayerColor.Black), test.FEN);
            this.boardControl.Board.Start();
            this.boardControl.Board.PieceMoved += move => { if (board.CurrentPlayer.PlayerColor == PlayerColor.White) return; moves--; if (moves == 0) board.Stop(); };
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

            var board = this.boardControl.StartNew(config.White, config.Black);
            spectatorServer = new SpectatorServer(board);

            var mouseSensor = new Sensors.MouseSensor(this.boardControl);
            var eyeTrackerSensor = new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardControl);
            var sensorContainer = new Sensors.SensorContainer(board, mouseSensor, eyeTrackerSensor);

            var writer = new IO.ChessStreamWriter(board, sensorContainer, DateTime.Now.ToString("yyyMMddmmss") + ".chess");

            if (config.White.IsReady && config.Black.IsReady)
                board.Start();
            else
                this.boardControl.ShowMessage("Waiting for remote player", board.Start, () => config.White.IsReady && config.Black.IsReady);
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
            var board = this.boardControl.StartNew(new Player(), new Player(), spectator.StartingFEN);
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
                    this.boardControl.SetMousePosition(mouseData.Location);

                var eyeTracker = sensorAction.Data as Chess.Sensors.TobiiEyeTracker.EyeTrackerSensorData;
                if (eyeTracker != null)
                    this.boardControl.SetEyePosition(eyeTracker.LeftPosition);

            }
        }
        #endregion Legacy
    }
}