using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class GraphicsExtensionMethods
    {
        public static void DrawCircle(this Image img, Color color, Point point, Size size)
        {
            DrawCircle(img, new Pen(color, 3), point, size);
        }
        public static void DrawCircle(this Image img, Pen pen, Point point, Size size)
        {
            using (var g = Graphics.FromImage(img))
            {
                g.DrawEllipse(pen, point.X, point.Y, size.Width, size.Height);
            }
        }

        public static void FillCircle(this Image img, Color color, Point point, Size size)
        {
            FillCircle(img, new SolidBrush(color), point, size);
        }
        public static void FillCircle(this Image img, Brush brush, Point point, Size size)
        {
            using (var g = Graphics.FromImage(img))
            {
                g.FillEllipse(brush, point.X, point.Y, size.Width, size.Height);
            }
        }

        public static void DrawLine(this Image img, Color color, Point p1, Point p2)
        {
            DrawLine(img, new Pen(color, 3), p1, p2);
        }

        public static void DrawLine(this Image img, Pen pen, Point p1, Point p2)
        {
            using (var g = Graphics.FromImage(img))
            {
                g.DrawLine(pen, p1, p2);
            }
        }

        public static void DrawText(this Image img, Point point, string text, int fontSize = 12, Color? color = null)
        {
            using (var g = Graphics.FromImage(img))
            {
                g.DrawString(text, new Font("Arial", fontSize, FontStyle.Regular), new SolidBrush(color == null ? Color.White : color.Value), point.X, point.Y);
            }
        }

        public static SizeF MeasureString(this Image img, string text, int fontSize = 12)
        {
            using (var g = Graphics.FromImage(img))
            {
                return g.MeasureString(text, new Font("Arial", fontSize, FontStyle.Regular), fontSize);
            }
        }



    }
}
