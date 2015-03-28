using Chess.App;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

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