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
    [Command("heatmap", "Export heatmaps", "This command exports heat maps for all tests.")]
    public class ExportHeatMapsCommand : Command<ExportHeatMapsCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            List<string> inputFiles = new List<string>();

            if (args.Participants.Count == 0)
                inputFiles.Add(args.InputFile);
            else
                inputFiles.AddRange(args.Participants.Select(i => Path.Combine(args.InputFile, i + ".volke")));

            Log.Information("Exporting heatmaps...");
            if (!inputFiles.All(File.Exists))
            {
                Log.Error("File not found: " + inputFiles.First(i => !File.Exists(i)));
                System.Environment.Exit(1);
            }

            EyeMovementsFilters.Filter eyeMovementFilter = new EyeMovementsFilters.None();
            if (args.ApplyFilter)
                eyeMovementFilter = new EyeMovementsFilters.WeightedLowPassFilter();

            var outputDir = Path.Combine(ProcessingSettings.OutputDirectory, string.Join("-", inputFiles.Select(Path.GetFileNameWithoutExtension)));
            outputDir = Path.Combine(outputDir, "heatmaps" + (args.RenderSaliencyMap ? ".saliency" : ""));
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summaries = inputFiles.Select(inputFile => new VolkeTestReader(inputFile).Using(i => i.ReadSummary())).ToList();
            if (summaries.Any(summary => summary.TestItems.Count != summaries[0].TestItems.Count))
            {
                Log.Error("All files must have the same number of test items.");
                System.Environment.Exit(1);
            }

            for (int i = 0; i < summaries[0].TestItems.Count; i++)
            {
                if (args.Tests.Count > 0 && !args.Tests.Contains(i + 1))
                    continue;

                Log.Information("Exporting heatmap {0}.", i + 1);

                string outputFile = Path.Combine(outputDir, (i + 1).ToString()) + ".png";

                Color backgroundColor = args.BackgroundColor != null && args.BackgroundColor.StartsWith("#") ? ColorTranslator.FromHtml(args.BackgroundColor) : Color.FromName(args.BackgroundColor);
                var mask = new GaussianRoundMask(backgroundColor, args.FixationRadius, args.Sigma, args.GrayScale ? Mask.GrayscaleColors : Mask.DefaultColors);
                var boardImage = Board.Load(summaries[0].TestItems[i].BoardFEN).Draw(900, 900);

                var allFixations = summaries.Select(summary => Fixation.GetFixations(eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements), args.Eye, args.MinFixationThreshold, args.MaxFixationDistance));
                var fixations = allFixations.SelectMany(f => Normalize(f));

                mask.ApplyMask(boardImage, args.WeightOnDuration, args.RenderSaliencyMap, fixations.ToArray());
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                boardImage.Save(outputFile, ImageFormat.Png);
            }

            Log.Information("Done!");
        }

        private IEnumerable<Fixation> Normalize(IEnumerable<Fixation> fixations)
        {
            if (!fixations.Any())
                return fixations;

            float max = fixations.Max(i => i.Duration);
            return fixations.Select(i => new Fixation(0, (int)(100 * (i.Duration / max)), i.X, i.Y));
        }

        #region Arguments
        public class Arguments
        {
            [PositionalArgument(0, "input")]
            public string InputFile { get; set; }

            [Argument("fixationthreshold,ft")]
            public int MinFixationThreshold { get; set; }

            [Argument("maxfixationdistance,mfd")]
            public int MaxFixationDistance { get; set; }

            [Argument("lpf"), ArgumentDescription("Applies low-pass filter")]
            public bool ApplyFilter { get; set; }


            [Argument("eye"), ArgumentDescription("Determines which eyes are going to be used. \r\nValues: Right, Left, Average")]
            public Eye Eye { get; set; }

            [Argument("w"), ArgumentDescription("Weight on fixation duration. Default Value: false")]
            public bool WeightOnDuration { get; set; }

            [Argument("sigma"), ArgumentDescription("Sigma. Default Value: 5")]
            public float Sigma { get; set; }

            [Argument("saliency"), ArgumentDescription("Show as saliency map. Default Value: false")]
            public bool RenderSaliencyMap { get; set; }

            [Argument("g"), ArgumentDescription("Render heatmap in grayscale. Default Value: false")]
            public bool GrayScale { get; set; }

            [Argument("bg"), ArgumentDescription("Background color. Default Value: Transparent")]
            public string BackgroundColor { get; set; }

            [Argument("r"), ArgumentDescription("Fixation Radius. Default Value: 80")]
            public int FixationRadius { get; set; }

            [Argument("tests,t"), ArgumentDescription("Specify which tests are going to be exported.\r\nUsage: 1,2,3")]
            public IList<int> Tests { get; set; }

            [Argument("p"), ArgumentDescription("Specify which participants are going to be merged. In this case 'Input File' parameter must point to folder with chesslab test files.\r\nUsage: 1,2,3")]
            public IList<int> Participants { get; set; }

            public bool Verbose { get; set; }

            public Arguments()
            {
                this.Eye = Eye.Average;
                this.MinFixationThreshold = 120;
                this.MaxFixationDistance = 120;
                this.FixationRadius = 120;
                this.Sigma = 5f;
                this.Tests = new int[0];
                this.Participants = new int[0];
                this.BackgroundColor = "Transparent";
            }
        }
        #endregion
    }
}
