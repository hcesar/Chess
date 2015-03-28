using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App
{
    public class BoardControl : System.Windows.Forms.PictureBox
    {
        public Piece SelectedPiece { get; private set; }

        public Board Board { get; private set; }

        public event Action<Square> PiecePicked;

        public BoardControl()
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            this.Visible = false;

            this.Width = 850;
            this.Height = 850;
            this.Image = new Bitmap(this.Width, this.Height);

            foreach (var square in Enum.GetValues(typeof(Square)).Cast<Square>())
                DrawSquare(square);

            base.InitLayout();
        }

        public void ShowMessage(string message, Action closeAction = null, Func<bool> closePredicate = null)
        {
            var dialog = new MessageDialog();
            dialog.Closing += (sender, e) => this.Controls.Remove(dialog);
            this.Controls.Add(dialog);
            dialog.ShowMessage(message, closePredicate, closeAction);
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
                    g.DrawString(square.GetRank().ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, loc);
                }

                if (square.GetRank() == 1)
                {
                    var loc = rect.Location;
                    loc.Offset(40, 105);
                    g.DrawString(((char)('A' + (square.GetColumn() - 1))).ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, loc);
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

        public void ClickSquare(Square square)
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
                if (this.PiecePicked != null)
                    PiecePicked(square);

                this.SelectedPiece = targetPiece;
                DrawSquare(square, true);
            }
        }

        public void Clear()
        {
            if (this.Board != null)
                ((IDisposable)this.Board).Dispose();

            this.Board = null;
            this.Image = new Bitmap(this.Width, this.Height);
        }
    }
}