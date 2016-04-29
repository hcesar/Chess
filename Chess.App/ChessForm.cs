using Chess.App.Tests;
using Chess.App.Controls;
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
using System.Text;
using Chess.Sensors;
using Chess.Sensors.TobiiEyeTracker;
using Chess.App.Tests.Volke;

namespace Chess.App
{
    public partial class ChessForm : Form
    {
        private SpectatorServer spectatorServer;
        private IList<Test> tests;
        public Participant currentParticipant;
        private SidePlayer sidePlayer;

        public ChessForm(SidePlayer sidePlayer)
        {
            this.sidePlayer = sidePlayer;
            this.boardControl = new Chess.App.BoardControl();
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            try
            {
                FormLoad();
            }
            catch
            {
            }
        }

        void FormLoad()
        {
            this.WindowState = FormWindowState.Maximized;
            var type = typeof(Sensors.TobiiEyeTracker.EyeTrackerSensor);
            this.TopMost = false;
            this.sidePlayer.Invoke(() => sidePlayer.TopMost = true);
            this.FormBorderStyle = FormBorderStyle.None;

            MaximizeToSecondaryMonitor();

            this.menuStrip1.Visible = false;
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

            // Check if EEG is connected
            //DialogResult dgResult = MessageBox.Show("Is the iCelera equipment connected?", "Chess Lab - EEG Connection", MessageBoxButtons.YesNo);
            //if (dgResult == DialogResult.Yes)
            //EEG.Connected = true;

            tests = Test.LoadAll();


            /*
            var volkQuestions = ((Chess.App.Tests.Volke.VolkeTest)tests[0]).Items;


            foreach (var subjectPath in Directory.EnumerateFiles(".", "*.volke"))
            {
                var subject = subjectPath.Replace(".\\", "");

                var report = Report.Report.Get(subject);
                for (int i = 0; i < report.Boards.Count; i++)
                {
                    report.Boards[i].Test = volkQuestions[i];
                    report.Boards[i].GenerateHeatMap(subject);
                }
            }

            System.Environment.Exit(0);*/

            //System.Diagnostics.Debugger.Launch();

            var args = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
            // args = "renato.volke";
            // args = "2.volke";
            if (args != null && args.EndsWith(".volke", StringComparison.InvariantCultureIgnoreCase))
            {
                var player = new Tests.Volke.VolkePlayer(this.boardControl, this.sidePlayer.BoardControl);
                player.Play(args);
                player.End += (p) => this.Close();
                return;
            }


            /*
            if (!Directory.Exists("imgs"))
                Directory.CreateDirectory("imgs");

            var bmp = new Bitmap(850, 850);
            var bmp2 = new Bitmap(800, 800);
            var sb = new StringBuilder();
            foreach (Chess.App.Tests.Volke.VolkeTestItem test in tests.Cast<Chess.App.Tests.Volke.VolkeTest>().First().Items)
            {
                int id = int.Parse(test.Id) + 1;
                this.boardControl.StartNew(new Player(), new Player(), test.FEN);
                this.boardControl.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
                using (var g = Graphics.FromImage(bmp2))
                {
                    g.DrawImageUnscaledAndClipped(bmp, new Rectangle(new Point(-15, 0), bmp2.Size));
                }
                //bmp2.Save("imgs\\img_TesteProficiencia" + (id) + ".png", System.Drawing.Imaging.ImageFormat.Png);
                sb
                    .AppendFormat("\\noindent\r\n{0}) {1} \\\\ \r\n", id, test.Question)
                    .AppendFormat("Categoria: {0} \\\\ \r\n", GetQuestionCategory(test.QuestionLevel))
                    .AppendFormat("Resposta: {0} \\\\ \r\n", test.IsCorrect ? "Sim" : "Não")
                    .Append(@"\begin{figure}[H]
	\centering
	\includegraphics[width=0.5\linewidth]{Images/TesteProficiencia/img_TesteProficiencia" + id + @".png}
\end{figure}").AppendLine("\r\n");

                if (id % 2 == 0)
                    sb.AppendLine("\\newpage");

            }

            Clipboard.SetText(sb.ToString());*/
            /*
            var participant = this.OpenDialog<ParticipantsForm, Participant>();
            currentParticipant = participant;
            if (!Directory.Exists("recorded-files"))
                Directory.CreateDirectory("recorded-files");

            ShowTest();
            var mouseSensor = new Sensors.MouseSensor(this.boardControl);
            var eyeTrackerSensor = new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardControl);
            var sensorContainer = new Sensors.SensorContainer(mouseSensor, eyeTrackerSensor);
            sidePlayer.StartPlayer(this.boardControl, sensorContainer);
             * */
        }

        public TestContainer StartTest(Participant participant)
        {
            this.currentParticipant = participant;
            var mouseSensor = new Sensors.MouseSensor(this.boardControl);
            var eyeTrackerSensor = new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardControl);
            var sensorContainer = new Sensors.SensorContainer(mouseSensor, eyeTrackerSensor);

            var test = this.tests[0];
            var orchestrator = test.GetOrchestrator(this.sidePlayer, this.boardControl, this.currentParticipant.Id, sensorContainer);
            orchestrator.Finished += (sender, result) => { currentParticipant.Tests.Add(result); this.ShowTest(int.MaxValue); };
            orchestrator.Start();

            return new TestContainer(orchestrator, sensorContainer);
        }

        private string GetQuestionCategory(int level)
        {
            switch (level)
            {
                case 1: return "Pattern recognition (Volke)";
                case 2: return "Rule retrieval (Volke)";
                case 3: return "Simple checkmate judgement (Volke)";
                case 4: return "Checkmate in one move (Volke)";
                case 5: return "Rule Retrieval (Nichelli)";
                default: return "-";
            }
        }

        private void ShowTest(int index = 0)
        {
            if (currentParticipant == null)
                return;

            if (index == 0)
                currentParticipant.Tests.Clear();

            if (this.tests.Count <= index)
            {
                if (index == 0)
                    return;

                String completedMessage = "Muito obrigado pela sua participação!\n\n" +
                                "RESEARCH TEAM: Carlos Eduardo Thomaz, Fábio Henrique " +
                                "Gonçalves Cesar, Fábio Theoto Rocha, Roberto G. de " +
                                "Magalhães and Thomás Oliveira Horta\n\n" +
                                "REFERENCES: (1) H.-J. Volke et al. On-coupling and " +
                                "off-coupling of neocortical areas in chess experts " +
                                "and novices, Journal of Psychophysiology, 16:23-36, " +
                                "2002; (2) P. Nichelli et al. Brain activity in chess " +
                                "playing, Nature, 369:191, 1994; (3) W. da Silva. " +
                                "Xadrez para todos: A Ginástica da Mente, Editora: " +
                                "Bolsa do Livro, 2011; (4) Ludek Pachman. Modern Chess " +
                                "Strategy, Editora: Dover Publications, 1971; " +
                                "(5) Lázló Polgár. Chess: 5334 Problems, Combinations, " +
                                "and Games, Editora: Black Dog & Leventhal Publishers, " +
                                "1995.";

                Participant.Update(currentParticipant);
                this.boardControl.Clear();
                this.boardControl.ShowMessage(completedMessage, null, null, 800, 600);
                this.Close();
                return;
            }
            /*
            var test = this.tests[index];
            var orchestrator = test.GetOrchestrator(this.sidePlayer, this.boardControl, this.currentParticipant.Id);
            orchestrator.Finished += (sender, result) => { currentParticipant.Tests.Add(result); this.ShowTest(index + 1); };
            orchestrator.Start();*/
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
            //this.menuStrip1.Visible = e.Y < 30;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var config = NewGameForm.Show(this);
            if (config == null)
                return;

            if (spectatorServer != null)
                ((IDisposable)spectatorServer).Dispose();

            var board = this.boardControl.StartNew(config.White, config.Black);

            var mouseSensor = new Sensors.MouseSensor(this.boardControl);
            var eyeTrackerSensor = new Sensors.TobiiEyeTracker.EyeTrackerSensor(this.boardControl);
            var sensorContainer = new Sensors.SensorContainer(mouseSensor, eyeTrackerSensor);

            var output = new ProxiedMemoryStream(File.Create(DateTime.Now.ToString("yyyMMddmmss") + ".chess"));
            spectatorServer = new SpectatorServer(board, sensorContainer);
            var writer = new IO.ChessStreamWriter(board, sensorContainer, output);


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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.sidePlayer.Invoke(sidePlayer.Close);
            base.OnClosing(e);
        }

        public void MaximizeToSecondaryMonitor()
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;
                // If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
                // if (window.IsLoaded)
                //  window.WindowState = WindowState.Maximized;
            }
        }
    }
}