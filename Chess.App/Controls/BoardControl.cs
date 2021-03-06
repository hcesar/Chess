﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App
{
    public class BoardControl : System.Windows.Forms.PictureBox
    {
        public Piece SelectedPiece { get; private set; }

        public Board Board { get; private set; }

        private TrackingControl eyeTracking;
        private TrackingControl mouseTracking;

       

        public event Action<Board> GameStarted;
        public event Action<string> MessageShowed;

        public BoardControl()
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            eyeTracking = new TrackingControl();
            this.Controls.Add(eyeTracking);
            eyeTracking.Size = new System.Drawing.Size(100, 100);
            eyeTracking.BackColor = Color.Transparent;
            eyeTracking.BringToFront();
            eyeTracking.Color = Color.Red;
            eyeTracking.Visible = false;

            mouseTracking = new TrackingControl();
            this.Controls.Add(mouseTracking);
            mouseTracking.Size = new System.Drawing.Size(100, 100);
            mouseTracking.BackColor = Color.Transparent;
            mouseTracking.BringToFront();
            mouseTracking.Color = Color.Green;
            mouseTracking.Visible = false;

            var imgEyeTracking = new Bitmap(eyeTracking.Width, eyeTracking.Height);
            using (var g = Graphics.FromImage(imgEyeTracking))
                g.DrawEllipse(new Pen(Brushes.Red, 3), 5, 5, 90, 90);
            eyeTracking.Image = imgEyeTracking;

            var imgMouseTracking = new Bitmap(mouseTracking.Width, mouseTracking.Height);
            using (var g = Graphics.FromImage(imgMouseTracking))
                g.DrawEllipse(new Pen(Brushes.Blue, 3), 5, 5, 90, 90);
            mouseTracking.Image = imgMouseTracking;

            this.Visible = false;

            this.Image = new Bitmap(this.Width, this.Height);
            base.InitLayout();
        }

        public Board StartNew(Player whitePlayer, Player blackPlayer, string fenString = null)
        {
            this.CloseDialog();


            this.Width = 850;
            this.Height = 850;

            this.Clear();
            //this.Focus();
            this.Visible = true;

            if (fenString == null)
                this.Board = new Board(whitePlayer, blackPlayer);
            else
                this.Board = new Board(whitePlayer, blackPlayer, fenString);

            this.Board.SquareChanged += square => DrawSquare(square);
            this.Board.Check += (player) => { if (this.Board != null && this.Board.CurrentPlayer is HumanPlayer) MessageBox.Show("Check!"); };
            this.Board.Checkmate += (player) => { MessageBox.Show("Checkmate!"); this.Cursor = Cursors.Arrow; };
            this.Board.Stalemate += (reason) => { MessageBox.Show("Stalemate: " + reason.GetDescription()); this.Cursor = Cursors.Arrow; };

            whitePlayer.Turn += () => this.Invoke((Action)(() => this.Cursor = whitePlayer is HumanPlayer ? Cursors.Arrow : Cursors.WaitCursor));
            blackPlayer.Turn += () => this.Invoke((Action)(() => this.Cursor = blackPlayer is HumanPlayer ? Cursors.Arrow : Cursors.WaitCursor));

            foreach (var square in Enum.GetValues(typeof(Square)).Cast<Square>())
                DrawSquare(square);

            if (this.GameStarted != null)
                this.GameStarted(this.Board);

            return this.Board;
        }

        public void ShowMessage(string message, Action closeAction = null, Func<bool> closePredicate = null, int dgSizeX = 800, int dgSizeY = 200)
        {
            this.CloseDialog();

            var dialog = new MessageDialog(dgSizeX, dgSizeY);
            dialog.Closing += (sender, e) => this.Controls.Remove(dialog);
            this.Controls.Add(dialog);
            dialog.ShowMessage(message, closePredicate, closeAction);

            if (this.MessageShowed != null)
                this.MessageShowed(message);
        }

        public void CloseDialog()
        {
            var dialog = this.Controls.OfType<MessageDialog>().FirstOrDefault();
            if (dialog != null)
            {
                this.Controls.Remove(dialog);
                dialog.Close();                
            }

        }
        private void DrawSquare(Square square, bool selected = false)
        {
            using (var lg = this.Image.GetGraphics())
            {
                var g = lg.Graphics;
                var rect = square.GetRectangle();
                g.DrawImageUnscaled(Images.GetSquareImage(square, this.Board[square]), rect);

                if (square.GetColumn() == 1)
                {
                    var loc = rect.Location;
                    loc.Offset(-15, 40);
                    //g.DrawString(square.GetRank().ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, loc);
                }

                if (square.GetRank() == 1)
                {
                    var loc = rect.Location;
                    loc.Offset(40, 105);
                    //g.DrawString(((char)('a' + (square.GetColumn() - 1))).ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, loc);
                }

                if (selected)
                {
                    rect.Width -= 3;
                    rect.Height -= 3;
                    g.DrawRectangle(new Pen(Brushes.Green, 3), rect);
                }
            }

            this.Invalidate(square.GetRectangle());
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.Board == null || !this.Board.IsActive)
                return;

            var square = new Point(e.X, e.Y).ToSquare();
            if (e.Button == System.Windows.Forms.MouseButtons.Left && square >= Square.A1)
            {
                if (this.Board.CurrentPlayer is HumanPlayer)
                    this.ClickSquare(square);
            }
        }

        private void ClickSquare(Square square)
        {
            if (this.Board == null || !this.Board.IsActive)
                return;

            var currentPiece = this.SelectedPiece;
            var targetPiece = this.Board[square];

            if (currentPiece == targetPiece && targetPiece != null)
            {
                DrawSquare(SelectedPiece.Square, false);
                this.SelectedPiece = null;
            }
            else if (currentPiece != null)
            {
                DrawSquare(SelectedPiece.Square, false);
                this.SelectedPiece = null;
                if (this.Board.Move(currentPiece.Square, square, square.IsPromotionSquare() ? typeof(Pieces.Queen) : null))
                    this.Cursor = Cursors.Arrow;
            }
            else if (targetPiece != null && targetPiece.Player == this.Board.Turn)
            {
                this.SelectedPiece = targetPiece;
                DrawSquare(square, true);
            }
        }

        internal void SetEyePosition(Point newPosition)
        {
            newPosition.Offset(-eyeTracking.Width / 2, -eyeTracking.Height / 2);
            this.Invalidate(new Rectangle(eyeTracking.Location, new Size(100, 100)));

            if (!eyeTracking.Visible)
                eyeTracking.Visible = true;
            eyeTracking.Location = newPosition;
        }

        internal void SetMousePosition(Point newPosition)
        {
            newPosition.Offset(-mouseTracking.Width / 2, -mouseTracking.Height / 2);
            this.Invalidate(new Rectangle(mouseTracking.Location, new Size(100, 100)));

            if (!mouseTracking.Visible)
                mouseTracking.Visible = true;
            mouseTracking.Location = newPosition;
        }


        internal void Clear()
        {
            if (this.Board != null)
                ((IDisposable)this.Board).Dispose();

            this.Board = null;
            this.Image = new Bitmap(this.Width, this.Height);
        }

        public static Bitmap GetBoardImage(string fen, bool withCellIndentity = true)
        {
            var ctrl = new BoardControl();
            ctrl.StartNew(new Player(), new Player(), fen);
            ctrl.BackColor = Color.Black;

            var bmp = new Bitmap(850, 850);
            ctrl.DrawToBitmap(bmp, new Rectangle(Point.Empty, Size.Add(bmp.Size, new Size(-15, -15))));
            return bmp;
        }
    }
}