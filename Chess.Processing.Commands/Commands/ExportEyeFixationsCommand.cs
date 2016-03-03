using Chess.App.Tests;
using Chess.App.Tests.Volke;
using Chess.IO;
using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("fixations", "Export fixations", "This command exports the eye fixations for all tests.")]
    public class ExportFixationsCommand : Command<ExportFixationsCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            Log.Information("Exporting eye fixations...");
            if (!File.Exists(args.InputFile))
            {
                Log.Error("File not found: " + args.InputFile);
                System.Environment.Exit(1);
            }

            EyeMovementsFilters.Filter eyeMovementFilter = new EyeMovementsFilters.None();
            if (args.ApplyFilter)
                eyeMovementFilter = new EyeMovementsFilters.WeightedLowPassFilter();

            if (args.FrameRate > 0)
                eyeMovementFilter = EyeMovementsFilters.Filter.Combine(eyeMovementFilter, new EyeMovementsFilters.Fps(args.FrameRate));

            var outputDir = Path.Combine(ProcessingSettings.OutputDirectory, Path.GetFileNameWithoutExtension(args.InputFile));
            outputDir = Path.Combine(outputDir, "fixations");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            switch (args.Format)
            {
                case ExportFormat.Csv:
                    ExportCsv(args, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Image:
                    ExportImage(args, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Video:
                    throw new NotSupportedException();
            }

            Log.Information("Done!");
        }

        #region ExportCsv
        private void ExportCsv(Arguments args, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            if (!Directory.Exists(Path.Combine(outputDir, "CSV")))
                Directory.CreateDirectory(Path.Combine(outputDir, "CSV"));

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());


            for (int i = 0; i < summary.TestItems.Count; i++)
            {
                if (args.Tests.Count > 0 && !args.Tests.Contains(i + 1))
                    continue;

                string outputFile = Path.Combine(outputDir, "CSV", (i + 1).ToString()) + ".csv";
                var fixations = Fixation.GetFixations(eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements), args.Eye, args.MinFixationThreshold, args.MaxFixationDistance);

                using (var outputStream = File.OpenWrite(outputFile))
                    CsvWriter.Write(outputStream, fixations);
            }
        }
        #endregion ExportCsv

        #region ExportImage
        private void ExportImage(Arguments args, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            if (!Directory.Exists(Path.Combine(outputDir, "Image")))
                Directory.CreateDirectory(Path.Combine(outputDir, "Image"));

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            for (int i = 0; i < summary.TestItems.Count; i++)
            {
                if (args.Tests.Count > 0 && !args.Tests.Contains(i + 1))
                    continue;

                string outputFile = Path.Combine(outputDir, "Image", args.OutputFile != null ? args.OutputFile : (i + 1).ToString()) + ".png";
                var fixations = Fixation.GetFixations(eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements), args.Eye, args.MinFixationThreshold, args.MaxFixationDistance);

                var boardImage = Board.Load(summary.TestItems[i].BoardFEN).Draw(900, 900);
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                int idx = 1;

                var halfsize = args.MaxFixationDistance / 2;

                foreach (var fixation in fixations)
                {
                    boardImage.DrawCircle(Color.Red, new Point(fixation.X - halfsize, fixation.Y - halfsize), new Size(args.MaxFixationDistance, args.MaxFixationDistance));
                    var text = string.Format("{0}) {1}ms", idx++, fixation.Duration);
                    boardImage.DrawText(new Point(fixation.X - 20, fixation.Y - 10), text, 10, Color.Red);
                }

                boardImage.Save(outputFile, ImageFormat.Png);
            }
        }
        #endregion ExportImage

        #region ExportMovie
        private void ExportMovie(VolkeTestSummary summary, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            throw new NotImplementedException();
        }
        #endregion ExportMovie

        #region Arguments
        public class Arguments
        {
            [PositionalArgument(0, "input"), Required, ArgumentDescription("Defines the input chess file")]
            public string InputFile { get; set; }

            [PositionalArgument(1, "output"), ArgumentDescription("Defines the output file")]
            public string OutputFile { get; set; }

            [Argument("format,f"), Required, ArgumentDescription("Determines the output format: Image, CSV, Video")]
            public ExportFormat Format { get; set; }

            [Argument("fps"), ArgumentDescription("Sets the video frame rate (valid only with Video format)")]
            public int FrameRate { get; set; }

            [Argument("lpf"), ArgumentDescription("Applies low-pass filter")]
            public bool ApplyFilter { get; set; }

            [Argument("eye"), ArgumentDescription("Determines which eyes are going to be used. \r\nValues: Right, Left, Average")]
            public Eye Eye { get; set; }

            [Argument("tests,t"), ArgumentDescription("Specify which tests are going to be exported.\r\nUsage: 1,2,3")]
            public IList<int> Tests { get; set; }

            [Argument("fixationtime,ft"), ArgumentDescription("Defines the minimum time threshold (in miliseconds) for determining a fixation. Default value: 120")]
            public int MinFixationThreshold { get; set; }

            [Argument("fixationdist,mfd"), ArgumentDescription("Defines the maximum distance threshold (in pixel) for determining a fixation. Default value: 120")]
            public int MaxFixationDistance { get; set; }

            public Arguments()
            {
                this.Tests = new List<int>();
                this.MinFixationThreshold = 120;
                this.MaxFixationDistance = 120;
                this.Eye = Processing.Eye.Average;
            }
        }
        #endregion
    }
}
