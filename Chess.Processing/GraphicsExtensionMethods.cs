using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ImageExtensionMethods
    {
        public static void DrawCircle(this Graphics graphics, Color color, Point point, Size size)
        {
            DrawCircle(graphics, new Pen(color, 3), point, size);
        }
        public static void DrawCircle(this Graphics graphics, Pen pen, Point point, Size size)
        {
            graphics.DrawEllipse(pen, point.X, point.Y, size.Width, size.Height);
        }

        public static void FillCircle(this Graphics graphics, Color color, Point point, Size size)
        {
            FillCircle(graphics, new SolidBrush(color), point, size);
        }
        public static void FillCircle(this Graphics graphics, Brush brush, Point point, Size size)
        {
            graphics.FillEllipse(brush, point.X, point.Y, size.Width, size.Height);
        }

        public static void DrawLine(this Graphics graphics, Color color, Point p1, Point p2)
        {
            DrawLine(graphics, new Pen(color, 3), p1, p2);
        }

        public static void DrawLine(this Graphics graphics, Pen pen, Point p1, Point p2)
        {
            graphics.DrawLine(pen, p1, p2);
        }

        public static void DrawText(this Graphics graphics, Point point, string text, int fontSize = 12, Color color = new Color())
        {
            graphics.DrawString(text, new Font("Arial", fontSize, FontStyle.Regular), new SolidBrush(color == Color.Empty ? Color.Black : color), point.X, point.Y);
        }

        public static SizeF MeasureString(this Graphics graphics, string text, int fontSize = 12)
        {
            return graphics.MeasureString(text, new Font("Arial", fontSize, FontStyle.Regular), fontSize);
        }
    }
}
