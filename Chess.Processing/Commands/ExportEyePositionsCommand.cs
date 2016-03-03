using Chess.App.Tests;
using Chess.App.Tests.Volke;
using Chess.IO;
using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("eyepositions", "Export eye positions", "This command exports the eye positions through time for all tests.")]
    public class ExportEyePositionsCommand : Command<ExportEyePositionsCommand.Arguments>
    {
        protected override void ExecuteCore(ExportEyePositionsCommand.Arguments args)
        {
            Log.Information("Exporting eye positions...");
            if (!File.Exists(args.InputFile))
            {
                Log.Error("File not found: " + args.InputFile);
                System.Environment.Exit(1);
            }

            var outputDir = Path.Combine(ProcessingSettings.OutputDirectory, Path.GetFileNameWithoutExtension(args.InputFile));
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var sb = new StringBuilder();

            using (var reader = new VolkeTestReader(args.InputFile))
            {
                var summary = reader.ReadSummary();
                for (int i = 0; i < summary.TestItems.Count; i++)
                {
                    sb.Clear();
                    sb.AppendLine("Time;X1;Y1;X2;Y2");

                    foreach (var entry in summary.TestItems[i].EyeMovements)
                    {
                        sb
                            .AppendFormat("{0};", entry.TimeFrame)
                            .AppendFormat("{0};", entry.LeftPosition.X)
                            .AppendFormat("{0};", entry.LeftPosition.Y)
                            .AppendFormat("{0};", entry.RightPosition.X)
                            .AppendFormat("{0};", entry.RightPosition.Y)
                            .AppendLine();
                    }

                    string outputFile = Path.Combine(outputDir, (i + 1).ToString()) + ".csv";
                    File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
                }
            }

            Log.Information("Done!");
        }

        public override string GetUsageMessage(string executable)
        {
            return executable + " eyepositions inputfile.chess outputfile.csv";
        }

        public class Arguments
        {
            [PositionalArgument(0)]
            public string InputFile { get; set; }

            [NamedArgument("precision")]
            public int PrecisionInMilliseconds { get; set; }

            public bool Verbose { get; set; }

        }
    }
}
