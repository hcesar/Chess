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
    [Command("aoi", "Export fixations", "This command exports the eye fixations for all tests.")]
    public class ExportAoiCommand : Command<ExportAoiCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            Log.Information("Exporting AOI from fixations...");
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
            outputDir = Path.Combine(outputDir, "aoi");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());
            ExportImage(args, eyeMovementFilter, outputDir);

            Log.Information("Done!");
        }

        #region ExportImage
        private void ExportImage(Arguments args, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            for (int i = 0; i < summary.TestItems.Count; i++)
            {
                if (args.Tests.Count > 0 && !args.Tests.Contains(i + 1))
                    continue;

                string outputFile = Path.Combine(outputDir, args.OutputFile != null ? args.OutputFile : (i + 1).ToString()) + ".png";
                var fixations = Fixation.GetFixations(eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements), args.Eye, args.MinFixationThreshold, args.MaxFixationDistance);

                var board = Board.Load(summary.TestItems[i].BoardFEN);
                var boardImage = board.Draw(900, 900);
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                int size = 16;
                int halfsize = size / 2;

                foreach (Square square in Enum.GetValues(typeof(Square)))
                {
                    var point = square.GetRectangle();
                    if (board[square] == null || !fixations.Any(f => point.IntersectsWith(f.ToReclangle(args.FixationRadius))))
                    {
                        board.Draw(boardImage, square, true);
                    }
                }


                boardImage.Save(outputFile, ImageFormat.Png);
            }
        }
        #endregion ExportImage

        #region Arguments
        public class Arguments
        {
            [PositionalArgument(0, "input"), Required, ArgumentDescription("Defines the input chess file")]
            public string InputFile { get; set; }

            [PositionalArgument(1, "output"), ArgumentDescription("Defines the output file")]
            public string OutputFile { get; set; }

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

            [Argument("radius"), ArgumentDescription("Defines the fixation radius. Default value: 80")]
            public int FixationRadius { get; set; }

            public Arguments()
            {
                this.Tests = new List<int>();
                this.Eye = Eye.Average;
                this.MinFixationThreshold = 120;
                this.MaxFixationDistance = 120;
                this.FixationRadius = 80;
            }

        }
        #endregion
    }
}
