using Chess.App.Tests;
using Chess.App.Tests.Volke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("eyefixations", "Export eye fixations", "This command exports the eye fixations for all tests.")]
    public class ExportEyeFixationsCommand : Command<ExportEyeFixationsCommand.Arguments>
    {
        protected override void ExecuteCore(ExportEyeFixationsCommand.Arguments args)
        {
            Log.Information("Exporting eye fixations...");

            var sb = new StringBuilder();
            sb.AppendLine("T;X;Y");


            //File.WriteAllText(args[1], sb.ToString(), Encoding.UTF8);
            Log.Information("Done!");
        }

        public override string GetUsageMessage(string executable)
        {
            return executable + " eyefixations inputfile.chess outputfile.csv";
        }

        public class Arguments
        {
            [PositionalArgument(0)]
            public string InputFile { get; set; }
            [PositionalArgument(1)]
            public string OutputFile { get; set; }

            [NamedArgument("precision")]
            public int PrecisionInMilliseconds { get; set; }

            public bool Verbose { get; set; }

        }
    }
}
