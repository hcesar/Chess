using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Chess
{
    internal class WinBoard : IDisposable
    {
        private bool firstMove = true;

        private bool Active { get { return this.process != null; } }

        private StreamReader output;
        private StreamWriter input;
        private Process process;
        private PlayerColor color;
        private string startFen;

        public AILevel AILevel { get; private set; }

        public event Action<Square, Square, Type> OnMove;

        public event Action<StalemateReason> OnStalemate;

        public event Action OnGameEnded;

        public event Action OnResign;

        public WinBoard(PlayerColor color, AILevel aiLevel, string startFen, string engine)
        {
            this.color = color;
            this.AILevel = aiLevel;
            this.startFen = startFen;
            var psi = new ProcessStartInfo { UseShellExecute = false, CreateNoWindow = true, FileName = "engines\\polyglot", Arguments = "engines\\" + engine + ".ini", RedirectStandardInput = true, RedirectStandardOutput = true };
            this.process = Process.Start(psi);
            this.output = this.process.StandardOutput;
            this.input = this.process.StandardInput;
        }

        internal void Start()
        {
            output.ReadLine();
            Send("xboard");
            Send("st " + (int)this.AILevel);
            Send("setboard " + this.startFen);
            Send(color.ToString().ToLower());

            if (this.color == PlayerColor.White)
                Send("go");

            new Thread(Listen) { IsBackground = true }.Start();
        }

        private void Send(string cmd)
        {
            firstMove = firstMove && cmd != "go";

            if (this.input == null)
                return;

            //Console.WriteLine(string.Format("{0} Send: {1}", this.color, cmd));
            this.input.WriteLine(cmd);
        }

        private void Listen()
        {
            while (true)
            {
                if (output == null)
                    break;

                string cmd = output.ReadLine();
                if (cmd == null)
                    break;

                string[] parts = cmd.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                //Console.WriteLine(string.Format("{0} Receive: {1}", this.color, cmd));

                if (!ProcessCommand(parts[0], parts.Length == 2 ? parts[1].Trim() : null))
                    throw new InvalidOperationException(string.Format("Command not supported: {0} ({1})", cmd, this.color));
            }
        }

        private bool ProcessCommand(string cmd, string argument)
        {
            Console.WriteLine("WINBOARD: {0} - {1} {2}", this.color, cmd, argument);
            switch (cmd)
            {
                case "move":
                    var source = argument.Substring(0, 2).ToEnum<Square>();
                    var target = argument.Substring(2, 2).ToEnum<Square>();
                    string promotionType = argument.Substring(4).Trim().Trim('=', '(', ')');
                    var promotion = promotionType.Length == 0 ? null : Piece.GetPieceType(argument.Substring(4).Trim().Trim('=', '(', ')'));

                    if (this.OnMove != null)
                        this.OnMove(source, target, promotion);

                    break;

                case "1/2-1/2":
                    if (this.OnStalemate != null)
                        this.OnStalemate(GetReason(argument));
                    break;

                case "resign":

                    if (this.OnResign != null)
                        this.OnResign();
                    break;

                case "0-1":
                case "1-0":
                    if (this.OnGameEnded != null)
                        this.OnGameEnded();
                    break;

                default:
                    return false;
            }

            return true;
        }

        private StalemateReason GetReason(string reason)
        {
            if (reason.IndexOf("fifty", StringComparison.InvariantCultureIgnoreCase) >= 0 || reason.IndexOf("50") >= 0)
                return StalemateReason.FiftyMoves;
            if (reason.IndexOf("repetition", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return StalemateReason.Repetition;
            if (reason.IndexOf("insufficient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return StalemateReason.InsufficientMaterial;

            return StalemateReason.NoMoveAvailable;
        }

        public void Move(string move)
        {
            if (this.Active)
                this.Send(move);

            if (this.firstMove)
                this.Send("go");
        }

        public void Dispose()
        {
            if (this.process != null)
            {
                this.Send("quit");
                this.process = null;
                this.output = null;
            }
        }
    }
}