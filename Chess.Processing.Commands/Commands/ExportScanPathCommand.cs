using AForge.Video.FFMPEG;
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
    [Command("scanpath", "Export fixations", "This command exports the scanpath for all tests.")]
    public class ExportScanpathCommand : Command<ExportScanpathCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            Log.Information("Exporting scanpaths...");
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
            outputDir = Path.Combine(outputDir, "scanpath");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            switch (args.Format)
            {
                case ExportFormat.Csv:
                    throw new NotSupportedException();
                case ExportFormat.Image:
                    ExportImage(args, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Video:
                    ExportVideo(args, eyeMovementFilter, outputDir);
                    break;
            }

            Log.Information("Done!");
        }

        #region ExportVideo
        private void ExportVideo(Arguments args, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {
            if (!Directory.Exists(Path.Combine(outputDir, "Video")))
                Directory.CreateDirectory(Path.Combine(outputDir, "Video"));

            var fps = args.FrameRate == 0 ? 23 : args.FrameRate;
            eyeMovementFilter = EyeMovementsFilters.Filter.Combine(eyeMovementFilter, new EyeMovementsFilters.Fps(fps));

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            for (int i = 0; i < summary.TestItems.Count; i++)
            {
                if (args.Tests.Count > 0 && !args.Tests.Contains(i + 1))
                    continue;

                Log.Information("Exporting video for test '{0}'", i + 1);
                var test = summary.TestItems[i];
                string outputFile = Path.Combine(outputDir, "video", args.OutputFile != null ? args.OutputFile : (i + 1).ToString()) + ".avi";

                var questionBitmap = new Bitmap(900, 900);
                using (var g = Graphics.FromImage(questionBitmap))
                    g.DrawImageUnscaled(test.Question.Image, test.Question.Point);

                var board = new Board(new Player(), new Player(), test.BoardFEN);
                var boardImage = board.Draw(900, 900);
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                using (VideoFileWriter writer = new VideoFileWriter())
                {
                    int bitrate = (args.Bitrate > 0 ? args.Bitrate : 3) * 1000 * 1000;
                    writer.Open(outputFile, boardImage.Width, boardImage.Height, fps, VideoCodec.MPEG4, bitrate);
                    if (args.WithQuestion)
                        WriteFrames(writer, args.Eye, questionBitmap, 80, eyeMovementFilter.ApplyFilter(test.QuestionEyeMovements));

                    var fixations = Fixation.GetFixations(eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements), args.Eye, args.MinFixationThreshold, args.MaxFixationDistance);
                    WriteFrames(writer, args.Eye, boardImage, 80, eyeMovementFilter.ApplyFilter(test.AnswerEyeMovements), fixations);
                    writer.Close();
                }

            }
        }
        static void WriteFrames(VideoFileWriter writer, Eye eye, Bitmap baseImage, int vspan, IEnumerable<VolkeEyeSensorMessage> eyeMovements, IList<Fixation> fixations = null)
        {
            fixations = fixations ?? new Fixation[0];

            long maxDuration = fixations.Max(i => i.Duration);
            foreach (var eyeMovement in eyeMovements)
            {
                var frame = (Bitmap)baseImage.Clone();

                if (fixations.Any(i => i.End < eyeMovement.Timestamp))
                    DrawScanPath(frame, fixations.Where(fix => fix.End < eyeMovement.Timestamp), maxDuration);

                using (var g = Graphics.FromImage(frame))
                {
                    var left = eyeMovement.LeftPosition;
                    left.Offset(-vspan / 2, -vspan / 2);
                    var right = eyeMovement.RightPosition;
                    right.Offset(-vspan / 2, -vspan / 2);
                    var avg = new Point(NonZeroAverage(left.X, right.X), NonZeroAverage(left.Y, right.Y));

                    if (eye.HasFlag(Eye.Left) && IsPositive(left))
                        g.DrawEllipse(new Pen(Brushes.Red, 3), new Rectangle(left, new Size(vspan, vspan)));
                    if (eye.HasFlag(Eye.Right) && IsPositive(right))
                        g.DrawEllipse(new Pen(Brushes.Blue, 3), new Rectangle(right, new Size(vspan, vspan)));
                    if (eye.HasFlag(Eye.Average) && IsPositive(avg))
                        g.DrawEllipse(new Pen(Brushes.Green, 3), new Rectangle(avg, new Size(vspan, vspan)));
                }
                GC.Collect();
                writer.WriteVideoFrame(frame);
            }
        }

        private static int NonZeroAverage(int v1, int v2)
        {
            return ((v1 <= 0 ? v2 : v1) + (v2 <= 0 ? v1 : v2)) / 2;
        }

        private static bool IsPositive(Point p)
        {
            return p.X > 0 && p.Y > 0;
        }
        #endregion ExportVideo

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

                var board = Board.Load(summary.TestItems[i].BoardFEN);
                var boardImage = board.Draw(900, 900);
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                DrawScanPath(boardImage, fixations, fixations.Max(f => f.Duration));
                boardImage.Save(outputFile, ImageFormat.Png);
            }
        }

        private static void DrawScanPath(Bitmap board, IEnumerable<Fixation> fixations, long maxDuration){

            var colorCarousel = NewColorCarousel(Color.Red, Color.Blue, Color.YellowGreen);

            Fixation lastFixation = fixations.First();
            foreach (var fixation in fixations.Skip(1))
                board.DrawLine(colorCarousel(), lastFixation.Point, (lastFixation = fixation).Point);

            colorCarousel = NewColorCarousel(Color.Red, Color.Blue, Color.YellowGreen);
            int fixationNumber = 1;
            foreach (var fixation in fixations)
            {
                int circleSize = (int)Math.Max(80, (120 * (fixation.Duration / maxDuration)));
                int halfsize = circleSize / 2;

                using (var graphics = Graphics.FromImage(board))
                {
                    var fillColor = Color.FromArgb(127, colorCarousel());
                    graphics.FillCircle(fillColor, new Point(fixation.X - halfsize, fixation.Y - halfsize), new Size(circleSize, circleSize));
                    graphics.DrawCircle(Color.WhiteSmoke, new Point(fixation.X - halfsize, fixation.Y - halfsize), new Size(circleSize, circleSize));
                    var text = string.Format("{0}", fixationNumber++, fixation.Duration);

                    var textSize = graphics.MeasureString(text, 18);
                    graphics.DrawText(new Point((int)(fixation.X - textSize.Width / 2), (int)(fixation.Y - textSize.Height / 2)), text, 18, Color.Black);
                }
                lastFixation = fixation;
            }
        }


        private static Func<Color> NewColorCarousel(params Color[] colors)
        {
            int idx = 0;

            return () =>
            {
                var ret = colors[idx++];
                if (idx >= colors.Length)
                    idx = 0;

                return ret;
            };
        }

        #endregion ExportImage

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

            [Argument("question,wq"), ArgumentDescription("Allow quetion gaze data to be exported")]
            public bool WithQuestion { get; set; }

            [Argument("bitrate,b"), ArgumentDescription("Defines the bitrate of final video")]
            public int Bitrate { get; set; }

            [Argument("lpf"), ArgumentDescription("Applies low-pass filter")]
            public bool ApplyFilter { get; set; }

            [Argument("eye"), ArgumentDescription("Determines which eyes are going to be used. \r\nValues: Right, Left, Average")]
            public Eye Eye { get; set; }

            [Argument("tests,t"), ArgumentDescription("Specify which tests are going to be exported.\r\nUsage: 1,2,3")]
            public IList<int> Tests { get; set; }

            [Argument("fixationtime,ft"), ArgumentDescription("Defines the minimum time threshold (in miliseconds) for determining a fixation. Default value: 120"), DefaultValue("120")]
            public int MinFixationThreshold { get; set; }

            [Argument("fixationdist,mfd"), ArgumentDescription("Defines the maximum distance threshold (in pixel) for determining a fixation. Default value: 120"), DefaultValue("120")]
            public int MaxFixationDistance { get; set; }

            public Arguments()
            {
                this.Tests = new List<int>();
                this.Eye = Eye.Average;
                this.MinFixationThreshold = 120;
                this.MaxFixationDistance = 120;
            }
        }
        #endregion
    }
}
