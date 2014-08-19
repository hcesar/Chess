using Chess.App;
using System.Drawing;

namespace System
{
    internal static class ExtensionMethods
    {
        public static LockedGraphics GetGraphics(this Image image)
        {
            return new LockedGraphics(image);
        }
    }
}