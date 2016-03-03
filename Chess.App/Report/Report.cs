using Chess.App.Tests.Volke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Report
{
    public class Report
    {
        public IList<Board> Boards { get; private set; }

        public static Report Get(string chessFile)
        {
            var ret = new Report();
            ret.Boards = new List<Board>();
            Board currentBoard = null;

            using (var reader = new BinaryReader(File.OpenRead(chessFile)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int waitTime = (int)reader.ReadInt64();
                    var messageType = (VolkeMessageType)reader.ReadByte();


                    switch (messageType)
                    {
                        case VolkeMessageType.TestStart:
                            currentBoard = new Board(reader.ReadString());
                            ret.Boards.Add(currentBoard);
                            break;

                        case VolkeMessageType.MouseSensorData:
                            reader.ReadInt16(); //X
                            reader.ReadInt16(); //Y
                            break;

                        case VolkeMessageType.EyeSensorData:
                            var point1 = new Point(reader.ReadInt16(), reader.ReadInt16());
                            var point2 = new Point(reader.ReadInt16(), reader.ReadInt16());

                            if (currentBoard != null)
                            {
                                var eyeTrackingInfo = new EyeTrackingInfo(point1, point2, waitTime);
                                currentBoard.EyeTrackingInfo.Add(eyeTrackingInfo);
                            }
                            break;

                        case VolkeMessageType.Question:
                            reader.ReadInt16(); //X
                            reader.ReadInt16(); //Y
                            var imageBytes = reader.ReadBytes(reader.ReadInt32());
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return ret;
        }
    }

    public class Board
    {
        public Board(string fen)
        {
            this.FEN = fen;
            this.EyeTrackingInfo = new List<EyeTrackingInfo>();
            this.BoardImage = BoardControl.GetBoardImage(this.FEN);
        }

        public string FEN { get; private set; }
        public Bitmap BoardImage { get; private set; }

        public IList<EyeTrackingInfo> EyeTrackingInfo { get; private set; }

        public IEnumerable<EyeTrackingInfo> GetFilteredPoints()
        {
            LookPoint.Filters.WeightOOTriangular filter = new LookPoint.Filters.WeightOOTriangular();
            int filterCount = 0;

            foreach (var eyeTracking in this.EyeTrackingInfo)
            {
                var x = (eyeTracking.LeftEye.X + eyeTracking.RightEye.X) / 2;
                var y = (eyeTracking.LeftEye.Y + eyeTracking.RightEye.Y) / 2;
                var fp = filter.feed(++filterCount * 20, new PointF((float)x, (float)y));
                var point = new Point((int)fp.X, (int)fp.Y);
                yield return new EyeTrackingInfo(point, point, eyeTracking.Timestamp);
            }
        }

        public IEnumerable<IEnumerable<EyeTrackingInfo>> GetFixations()
        {
            Point lastPoint = Point.Empty;
            var fixation = new List<EyeTrackingInfo>();

            foreach (var point in this.GetFilteredPoints())
            {
                if (Distance(point.LeftEye, lastPoint) > 501 && fixation.Count > 0)
                {
                    if (fixation.Last().Timestamp - fixation.First().Timestamp > 500)
                        yield return fixation.ToArray();
                    fixation.Clear();
                }

                fixation.Add(point);
                lastPoint = point.LeftEye;
            }
        }

        public static int Distance(Point p1, Point p2)
        {
            return (int)(Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y));
        }


        public VolkeTestItem Test { get; set; }

        public void GenerateFixationMap(string subject)
        {
            var path = "maps\\" + subject.Replace(".volke", "");
            if (!Directory.Exists("maps")) Directory.CreateDirectory("maps");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);


            int idx = 0;
            foreach (var fixation in this.GetFixations())
            {
                var x = (float)fixation.Average(i => i.LeftEye.X);
                var y = (float)fixation.Average(i => i.LeftEye.Y);
                using (var g = this.BoardImage.GetGraphics())
                {
                    g.Graphics.DrawEllipse(Pens.Red, x, y, 100f, 100f);
                    g.Graphics.DrawString((++idx).ToString(), new Font("Arial", 12), Brushes.Red, x, y);
                }
            }

            var bmp = new Bitmap(850, 900);

            using (var g = bmp.GetGraphics())
            {
                g.Graphics.DrawString(this.Test.Question, new Font("Calibri", 12), Brushes.Black, 5, 5);
                g.Graphics.DrawString(this.Test.IsCorrect ? "SIM" : "NÃO", new Font("Calibri", 12, FontStyle.Bold), Brushes.Black, 5, 22);
                g.Graphics.DrawImageUnscaled(this.BoardImage, 0, 80);
            }

            bmp.Save(path + "\\" + this.Test.Id + ".png");
        }

        public void GenerateHeatMap(string subject)
        {
            var path = "maps\\" + subject.Replace(".volke", "");
            if (!Directory.Exists("maps")) Directory.CreateDirectory("maps");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            int maskSize = 150;
            var mask = GetMask(maskSize);
            int[,] exposure = new int[850, 850];

            LookPoint.Filters.WeightOOTriangular filter = new LookPoint.Filters.WeightOOTriangular();
            int filterCount = 0;

            foreach (var eyeTracking in this.GetFixations().SelectMany(i => i))
            {
                var mx = (eyeTracking.LeftEye.X + eyeTracking.RightEye.X) / 2;
                var my = (eyeTracking.LeftEye.Y + eyeTracking.RightEye.Y) / 2;
                var fp = filter.feed(++filterCount * 20, new PointF((float)mx, (float)my));
                var point = new Point((int)fp.X, (int)fp.Y);

                if (point.X < 0 || point.Y < 0 || point.X >= 850 || point.Y >= 850)
                    continue;

                int exposureValue = 10;
                for (int x = 0; x < maskSize; x++)
                {
                    int targetX = point.X - (maskSize / 2) + x;
                    if (targetX < 0 || targetX >= 850) continue;

                    for (int y = 0; y < maskSize; y++)
                    {
                        int targetY = point.Y - (maskSize / 2) + y;
                        if (targetY < 0 || targetY >= 850 || mask[x, y] == 0) continue;

                        exposure[targetX, targetY] += exposureValue * mask[x, y];
                    }
                }
            }

            float max = exposure.Cast<int>().Max();

            for (int x = 0; x < 850; x++)
                for (int y = 0; y < 850; y++)
                    this.BoardImage.SetPixel(x, y, GetColor(this.BoardImage.GetPixel(x, y), exposure[x, y], max));

            var bmp = new Bitmap(850, 900);


            using (var g = bmp.GetGraphics())
            {
                g.Graphics.DrawString(this.Test.Question, new Font("Calibri", 12), Brushes.Black, 5, 5);
                g.Graphics.DrawString(this.Test.IsCorrect ? "SIM" : "NÃO", new Font("Calibri", 12, FontStyle.Bold), Brushes.Black, 5, 22);
                g.Graphics.DrawImageUnscaled(this.BoardImage, 0, 80);
            }

            bmp.Save(path + "\\" + this.Test.Id + ".png");
        }

        private Color GetColor(Color color, int exposure, float max)
        {
            var heatMapColor = GetHeatMapColor(Math.Min(1, exposure / 500f));

            int r = (int)(heatMapColor.R * 0.5 + color.R * 0.5);
            int g = (int)(heatMapColor.G * 0.5 + color.G * 0.5);
            int b = (int)(heatMapColor.B * 0.5 + color.B * 0.5);

            return Color.FromArgb(255, r, g, b);
        }

        private Color GetHeatMapColor(float value)
        {
            const int NUM_COLORS = 4;
            float[][] color = new[] { new[] { 0f, 0, 1 }, new[] { 0f, 1, 0 }, new[] { 1f, 1, 0 }, new[] { 1f, 0, 0 } };

            // A static array of 4 colors:  (blue,   green,  yellow,  red) using {r,g,b} for each.

            int idx1;        // |-- Our desired color will be between these two indexes in "color".
            int idx2;        // |
            float fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

            if (value <= 0) { idx1 = idx2 = 0; }    // accounts for an input <=0
            else if (value >= 1) { idx1 = idx2 = NUM_COLORS - 1; }    // accounts for an input >=0
            else
            {
                value = value * (NUM_COLORS - 1);        // Will multiply value by 3.
                idx1 = (int)Math.Floor(value);                  // Our desired color will be after this index.
                idx2 = idx1 + 1;                        // ... and before this index (inclusive).
                fractBetween = value - (float)idx1;    // Distance between the two indexes (0-1).
            }

            try
            {
                float red = 255 * ((color[idx2][0] - color[idx1][0]) * fractBetween + color[idx1][0]);
                float green = 255 * ((color[idx2][1] - color[idx1][1]) * fractBetween + color[idx1][1]);
                float blue = 255 * ((color[idx2][2] - color[idx1][2]) * fractBetween + color[idx1][2]);

                return Color.FromArgb(255, (int)red, (int)green, (int)blue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Color.White;
            }
        }

        static int[,] GetMask(int size)
        {
            int[,] mask = new int[size, size];
            int radius = size / 2;
            int xCenter = size / 2;
            int yCenter = size / 2;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (Math.Pow((x - xCenter), 2) + Math.Pow((y - yCenter), 2) <= (Math.Pow(radius / 2.75, 2)))
                        mask[x, y] = 3;
                    else if (Math.Pow((x - xCenter), 2) + Math.Pow((y - yCenter), 2) <= (Math.Pow(radius / 1.45, 2)))
                        mask[x, y] = 2;
                    else if (Math.Pow((x - xCenter), 2) + Math.Pow((y - yCenter), 2) <= (radius * radius))
                        mask[x, y] = 1;
                }
            }
            return mask;
        }
    }

    public struct EyeTrackingInfo
    {
        public EyeTrackingInfo(Point leftEye, Point rightEye, int timestamp)
            : this()
        {
            LeftEye = leftEye;
            RightEye = rightEye;
            Timestamp = timestamp;
        }

        public Point LeftEye { get; private set; }
        public Point RightEye { get; private set; }
        public int Timestamp { get; private set; }
    }
}