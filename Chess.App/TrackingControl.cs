using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App
{
    class TrackingControl : PictureBox
    {

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Parent != null)
            {
                float
                    tx = -Left,
                    ty = -Top;

                // make adjustments to tx and ty here if your control
                // has a non-client area, borders or similar

                e.Graphics.TranslateTransform(tx, ty);

                using (PaintEventArgs pea = new PaintEventArgs(e.Graphics, e.ClipRectangle))
                {
                    InvokePaintBackground(Parent, pea);
                    InvokePaint(Parent, pea);
                }

                e.Graphics.TranslateTransform(-tx, -ty);

                // loop through children of parent which are under ourselves
                int start = Parent.Controls.GetChildIndex(this);
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                for (int i = Parent.Controls.Count - 1; i > start; i--)
                {
                    Control c = Parent.Controls[i];

                    // skip ...
                    // ... invisible controls
                    // ... or controls that have zero width/height (Autosize Labels without content!)
                    // ... or controls that don't intersect with ourselves
                    if (!c.Visible || c.Width == 0 || c.Height == 0 || !rect.IntersectsWith(new Rectangle(c.Left, c.Top, c.Width, c.Height)))
                        continue;

                    using (Bitmap b = new Bitmap(c.Width, c.Height, e.Graphics))
                    {
                        c.DrawToBitmap(b, new Rectangle(0, 0, c.Width, c.Height));

                        tx = c.Left - Left;
                        ty = c.Top - Top;

                        // make adjustments to tx and ty here if your control
                        // has a non-client area, borders or similar

                        e.Graphics.TranslateTransform(tx, ty);
                        e.Graphics.DrawImageUnscaled(b, new Point(0, 0));
                        e.Graphics.TranslateTransform(-tx, -ty);
                    }
                }
            }
            for (int i = 5; i < 90; i += 10)
                e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2), (this.Width / 2) - i / 2, (this.Width / 2) - i / 2, i, i);
            //e.Graphics.DrawEllipse(new Pen(new SolidBrush(this.Color), 5), 5, 5, 90, 90);
        }

        public Color Color { get; set; }
    }

}
