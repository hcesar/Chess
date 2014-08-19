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

        private PictureBox eyeTracking;
        private MessageDialog dlgMessage;

        public BoardControl()
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            //
            // dlgMessage
            //
            this.dlgMessage = new Chess.App.MessageDialog();
            this.dlgMessage.Anchor = System.Windows.Forms.AnchorStyles.Left | AnchorStyles.Top;
            this.dlgMessage.BackColor = System.Drawing.Color.Wheat;
            this.dlgMessage.Location = new System.Drawing.Point(0, 300);
            this.dlgMessage.Margin = new System.Windows.Forms.Padding(0);
            this.dlgMessage.Name = "dlgMessage";
            this.dlgMessage.Size = new System.Drawing.Size(800, 200);
            this.dlgMessage.TabIndex = 1;
            this.dlgMessage.TabStop = false;
            this.dlgMessage.Visible = false;
            this.dlgMessage.Text = "Teste";

            this.Controls.Add(this.dlgMessage);

            eyeTracking = new PictureBox();
            this.Controls.Add(eyeTracking);
            eyeTracking.Size = new System.Drawing.Size(100, 100);
            eyeTracking.BackColor = Color.Transparent;
            eyeTracking.BringToFront();
            eyeTracking.Visible = false;

            var imgEyeTracking = new Bitmap(eyeTracking.Width, eyeTracking.Height);
            using (var g = Graphics.FromImage(imgEyeTracking))
                g.DrawEllipse(new Pen(Brushes.Red, 3), 5, 5, 90, 90);
            eyeTracking.Image = imgEyeTracking;

            var img = new Bitmap(this.Width, this.Height);
            using (var g = Graphics.FromImage(img))
            {
                foreach (var square in Enum.GetValues(typeof(Square)).Cast<Square>())
                    g.DrawImageUnscaled(Images.GetSquareImage(square), square.GetRectangle());
            }

            this.Image = img;
            this.Visible = false;
            base.InitLayout();
        }

        public Board StartNew(Player whitePlayer, Player blackPlayer, string fenString = null)
        {
            this.Visible = true;
            if (this.Board != null)
                ((IDisposable)this.Board).Dispose();

            if (fenString == null)
                this.Board = new Board(whitePlayer, blackPlayer);
            else
                this.Board = new Board(whitePlayer, blackPlayer, fenString);

            this.Board.SquareChanged += square => DrawSquare(square);
            this.Board.Check += (player) => { if (this.Board.CurrentPlayer is HumanPlayer) MessageBox.Show("Check!"); };
            this.Board.Checkmate += (player) => { MessageBox.Show("Checkmate!"); this.Cursor = Cursors.Arrow; };
            this.Board.Stalemate += (reason) => { MessageBox.Show("Stalemate: " + reason.GetDescription()); this.Cursor = Cursors.Arrow; };

            whitePlayer.Turn += () => this.Invoke((Action)(() => this.Cursor = whitePlayer is HumanPlayer ? Cursors.Arrow : Cursors.WaitCursor));
            blackPlayer.Turn += () => this.Invoke((Action)(() => this.Cursor = blackPlayer is HumanPlayer ? Cursors.Arrow : Cursors.WaitCursor));

            foreach (var square in Enum.GetValues(typeof(Square)).Cast<Square>())
                DrawSquare(square);
            return this.Board;
        }

        public void ShowMessage(string message, Action closeAction = null, Func<bool> closePredicate = null)
        {
            this.dlgMessage.ShowMessage(message, closePredicate, closeAction);
        }

        private void DrawSquare(Square square, bool selected = false)
        {
            using (var lg = this.Image.GetGraphics())
            {
                var g = lg.Graphics;
                var rect = square.GetRectangle();
                g.DrawImageUnscaled(Images.GetSquareImage(square, this.Board[square]), rect);

                if (selected)
                {
                    rect.Width -= 3;
                    rect.Height -= 3;
                    rect.Offset(1, 1);
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
    }
}