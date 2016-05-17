using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    internal class ProcessingSettings
    {
        public static string OutputDirectory
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["OutputFolder"];
            }
        }
    }
}
