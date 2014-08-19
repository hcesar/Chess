using System;
using System.Drawing;
using System.Threading;

namespace Chess.App
{
    internal class LockedGraphics : IDisposable
    {
        private Image image;

        public Graphics Graphics { get; private set; }

        public LockedGraphics(System.Drawing.Image image)
        {
            this.image = image;
            Monitor.Enter(this.image);

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    this.Graphics = Graphics.FromImage(this.image);
                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

        void IDisposable.Dispose()
        {
            Monitor.Exit(this.image);
        }
    }
}