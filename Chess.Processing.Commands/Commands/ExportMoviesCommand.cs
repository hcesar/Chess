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
    [Command("movies", "Export test movies", "This command exports the movies for all tests.")]
    public class ExportMoviesCommand : Command<ExportMoviesCommand.Arguments>
    {
        protected override void ExecuteCore(ExportMoviesCommand.Arguments args)
        {
            Log.Information("Exporting movies...");
            if (!File.Exists(args.InputFile))
            {
                Log.Error("File not found: " + args.InputFile);
                System.Environment.Exit(1);
            }

            int vspan = args.VisualSpanSize == 0 ? 80 : args.VisualSpanSize;
            EyeMovementsFilters.Filter eyeMovementFilter = new EyeMovementsFilters.Fps(23);
            if (args.FrameRate > 0)
                eyeMovementFilter = new EyeMovementsFilters.Fps(args.FrameRate);

            var outputDir = Path.Combine(ProcessingSettings.OutputDirectory, Path.GetFileNameWithoutExtension(args.InputFile), "movies");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            using (var reader = new VolkeTestReader(args.InputFile))
            {
                var summary = reader.ReadSummary();
                for (int i = 0; i < summary.TestItems.Count; i++)
                {
                    if (args.TestToExport.HasValue && i != args.TestToExport.GetValueOrDefault() - 1)
                        continue;

                    Log.Information("Exporting movie for test '{0}'", i + 1);
                    var test = summary.TestItems[i];
                    string outputFile = Path.Combine(outputDir, (i + 1).ToString()) + ".avi";


                    var questionBitmap = new Bitmap(900, 900);
                    using (var g = Graphics.FromImage(questionBitmap))
                        g.DrawImageUnscaled(test.Question.Image, test.Question.Point);

                    var board = new Board(new Player(), new Player(), test.BoardFEN);
                    var boardImage = new Bitmap(900, 900);
                    board.Draw(boardImage);
                    boardImage.DrawText(new Point(10, 850), Question.GetQuestion(i), 12);


                    using (VideoFileWriter writer = new VideoFileWriter())
                    {
                        writer.Open(outputFile, boardImage.Width, boardImage.Height, args.FrameRate == 0 ? 23 : args.FrameRate, VideoCodec.MPEG4, 3000 * 1000);
                        //WriteFrames(writer, questionBitmap, 80, eyeMovementFilter.ApplyFilter(test.QuestionEyeMovements));
                        WriteFrames(writer, boardImage, 80, eyeMovementFilter.ApplyFilter(test.AnswerEyeMovements));
                        writer.Close();
                    }

                }
            }

            Log.Information("Done!");
        }

        static void WriteFrames(VideoFileWriter writer, Bitmap baseImage, int vspan, IEnumerable<VolkeEyeSensorMessage> eyeMovements)
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
                    g.DrawEllipse(new Pen(Brushes.Red, 3), new Rectangle(left, new Size(vspan, vspan)));
                    g.DrawEllipse(new Pen(Brushes.Blue, 3), new Rectangle(right, new Size(vspan, vspan)));
                }
                GC.Collect();
                writer.WriteVideoFrame(frame);
            }
        }

        public override string GetUsageMessage(string executable)
        {
            return executable + " eyepositions inputfile.chess outputfile.csv";
        }

        public class Arguments
        {
            [PositionalArgument(0, "input")]
            public string InputFile { get; set; }

            [Argument("fps")]
            public int FrameRate { get; set; }

            [Argument("vspan")]
            public int VisualSpanSize { get; set; }


            [Argument("test")]
            public int? TestToExport { get; set; }

            public bool Verbose { get; set; }

        }
    }
}
