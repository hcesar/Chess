using AForge.Video.FFMPEG;
using Chess.App.Tests;
using Chess.App.Tests.Volke;
using Chess.IO;
using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("gaze", "Export gaze data", "Exports gaze for all tests.")]
    public class ExportGazeDataCommand : Command<ExportGazeDataCommand.Arguments>
    {
        protected override void ExecuteCore(ExportGazeDataCommand.Arguments args)
        {
            Log.Information("Exporting gaze data...");
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
            outputDir = Path.Combine(outputDir, "gaze");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var summary = new VolkeTestReader(args.InputFile).Using(i => i.ReadSummary());

            switch (args.Format)
            {
                default:
                    ExportCsv(args, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Image:
                    ExportImage(args, eyeMovementFilter, outputDir);
                    break;
                case ExportFormat.Video:
                    ExportVideo(args, eyeMovementFilter, outputDir);
                    break;
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

                Log.Information("Exporting CSV for test '{0}'", i + 1);

                string outputFile = Path.Combine(outputDir, "CSV", (i + 1).ToString()) + ".csv";
                var eyePositions = eyeMovementFilter.ApplyFilter(summary.TestItems[i].AnswerEyeMovements)
                                                    .Select(movement => new
                                                    {
                                                        TimeFrame = movement.Timestamp,
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
        private void ExportImage(Arguments args, EyeMovementsFilters.Filter eyeMovementFilter, string outputDir)
        {

        }
        #endregion ExportImage

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
                //string outputFile = Path.Combine(outputDir, "video", (i + 1).ToString()) + ".avi";
                string outputFile = Path.Combine(outputDir, "video", args.OutputFile != null ? args.OutputFile : (i + 1).ToString()) + ".avi";


                var questionBitmap = new Bitmap(900, 900);
                using (var g = Graphics.FromImage(questionBitmap))
                    g.DrawImageUnscaled(test.Question.Image, test.Question.Point);

                var board = new Board(new Player(), new Player(), test.BoardFEN);
                var boardImage = new Bitmap(900, 900);
                board.Draw(boardImage);
                boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);

                using (VideoFileWriter writer = new VideoFileWriter())
                {
                    int bitrate = (args.Bitrate > 0 ? args.Bitrate : 3) * 1000 * 1000;
                    writer.Open(outputFile, boardImage.Width, boardImage.Height, fps, VideoCodec.MPEG4, bitrate);
                    if (args.WithQuestion)
                        WriteFrames(writer, args.Eye, questionBitmap, 80, eyeMovementFilter.ApplyFilter(test.QuestionEyeMovements));
                    WriteFrames(writer, args.Eye, boardImage, 80, eyeMovementFilter.ApplyFilter(test.AnswerEyeMovements));
                    writer.Close();
                }

            }
        }

        static void WriteFrames(VideoFileWriter writer, Eye eye, Bitmap baseImage, int vspan, IEnumerable<VolkeEyeSensorMessage> eyeMovements)
        {
            foreach (var eyeMovement in eyeMovements)
            {
                var frame = (Bitmap)baseImage.Clone();
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

            [Argument("sampling"), ArgumentDescription("Resamples the source signal")]
            public int Sampling { get; set; }

            [Argument("question,wq"), ArgumentDescription("Allow quetion gaze data to be exported")]
            public bool WithQuestion { get; set; }

            [Argument("bitrate,b"), ArgumentDescription("Defines the bitrate of final video")]
            public int Bitrate { get; set; }

            [Argument("lpf"), ArgumentDescription("Applies low-pass filter")]
            public bool ApplyFilter { get; set; }

            [Argument("eye"), ArgumentDescription("Determines which eyes are going to be ploted (only supported in Video Format). \r\nValues: Both (default), Right, Left, Average\r\nValues can be combine, e.g.: -eye Right,Left,Average")]
            public Eye Eye { get; set; }

            [Argument("tests,t"), ArgumentDescription("Specify which tests are going to be exported.\r\nUsage: 1,2,3")]
            public IList<int> Tests { get; set; }

            public Arguments()
            {
                this.Tests = new List<int>();
            }
        }
        #endregion
    }
}
