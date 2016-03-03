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
    [Command("gaze", "Export gaze data", "This command exports the eye positions through time for all tests.")]
    public class ExportGazDataCommand : Command<ExportGazDataCommand.Arguments>
    {
        protected override void ExecuteCore(ExportGazDataCommand.Arguments args)
        {
            Log.Information("Exporting eye positions...");
            if (!File.Exists(args.InputFile))
            {
                Log.Error("File not found: " + args.InputFile);
                System.Environment.Exit(1);
            }

            EyeMovementsFilters.Filter eyeMovementFilter = new EyeMovementsFilters.None();
            if (args.Fps > 0)
                eyeMovementFilter = new EyeMovementsFilters.Fps(args.Fps);

            var outputDir = Path.Combine(ProcessingSettings.OutputDirectory, Path.GetFileNameWithoutExtension(args.InputFile));
            outputDir = Path.Combine(outputDir, "gaze");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            switch (args.Format)
            {
                case ExportFormat.Csv:
                    ExportCsv(summary, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Image:
                    ExportImage(summary, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Movie:
                    ExportMovie(summary, eyeMovementFilter, outputDir);
                    break;
            }

            Log.Information("Done!");
        }

        #region ExportCsv
        private void ExportCsv(VolkeTestSummary summary, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            for (int i = 0; i < summary.TestItems.Count; i++)
            {
                string outputFile = Path.Combine(outputDir, (i + 1).ToString()) + ".csv";
                var eyePositions = eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements)
                                                    .Select(movement => new
                                                    {
                                                        movement.TimeFrame,
                                                        RightX = movement.RightPosition.X,
                                                        RightY = movement.RightPosition.Y,
                                                        LeftX = movement.LeftPosition.X,
                                                        LeftY = movement.LeftPosition.Y,
                                                    });

                using (var outputStream = File.OpenWrite(outputFile))
                    CsvWriter.Write(outputStream, eyePositions);
            }
        }
        #endregion ExportCsv

        #region ExportImage
        private void ExportImage(VolkeTestSummary summary, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            throw new NotImplementedException();
        }
        #endregion ExportImage

        #region ExportMovie
        private void ExportMovie(VolkeTestSummary summary, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            throw new NotImplementedException();
        }
        #endregion ExportMovie

        public override string GetUsageMessage(string executable)
        {
            return executable + " eyepositions inputfile.chess outputfile.csv";
        }

        #region Arguments
        public class Arguments
        {
            [PositionalArgument(0)]
            public string InputFile { get; set; }

            [NamedArgument("format,f")]
            public ExportFormat Format { get; set; }

            [NamedArgument("fps")]
            public int Fps { get; set; }

            public bool Verbose { get; set; }

        }
        #endregion
    }
}
