using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public static class Common
    {
        public static string GetExecutable()
        {
            return Path.GetFileName(Environment.GetCommandLineArgs().First());
        }
    }
}
