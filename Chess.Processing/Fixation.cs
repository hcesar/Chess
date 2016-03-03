using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public class Fixation
    {
        public long Start { get; set; }
        public long End { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public long Duration { get { return this.End - this.Start; } }

        public Point Point { get; private set; }

        public Fixation(long start, long end, int x, int y)
        {
            this.Start = start;
            this.End = end;
            this.X = x;
            this.Y = y;
            this.Point = new Point(x, y);
        }

        public static IList<Fixation> GetFixations(IEnumerable<VolkeEyeSensorMessage> gazeData, Eye eye = Eye.Average, int minFixationThreshold = 120, int maxDistance = 120)
        {
            Func<VolkeEyeSensorMessage, Point> getPoint;

            switch (eye)
            {
                case Eye.Left: getPoint = (g) => new Point(g.LeftPosition.X, g.LeftPosition.Y); break;
                case Eye.Right: getPoint = (g) => new Point(g.RightPosition.X, g.RightPosition.Y); break;
                case Eye.Average: getPoint = (g) => new Point(g.X, g.Y); break;
                default: throw new NotSupportedException();

            }

            var gazePoints = gazeData.ToList();

            // empty list to contain data  
            var protoFixations = new List<List<long>>();
            var fixations = new List<Fixation>();

            // loop through all coordinates
            var si = 0;
            var fixstart = false;
            Func<long, List<long>> toList = (l) => new[] { l }.ToList();

            for (int i = 0; i < gazePoints.Count; i++)
            {
                //# calculate Euclidean distance from the current fixation coordinate
                //# to the next coordinate
                var dist = Math.Sqrt(Math.Pow(getPoint(gazePoints[si]).X - getPoint(gazePoints[i]).X, 2) + Math.Pow(getPoint(gazePoints[si]).Y - getPoint(gazePoints[i]).Y, 2));
                // check if the next coordinate is below maximal distance
                if (dist <= maxDistance && !fixstart)
                {
                    // start a new fixation
                    si = 0 + i;
                    fixstart = true;
                    protoFixations.Add(toList(gazePoints[i].Timestamp));
                }
                else if (dist > maxDistance && fixstart)
                {
                    // end the current fixation
                    fixstart = false;
                    // only store the fixation if the duration is ok
                    if (gazePoints[i - 1].Timestamp - protoFixations.Last()[0] >= minFixationThreshold)
                    {
                        var lastFixation = protoFixations.Last();
                        var point = getPoint(gazePoints[si]);
                        if (point.IsPositive())
                            fixations.Add(new Fixation(lastFixation[0], gazePoints[i - 1].Timestamp, point.X, point.Y));
                    }
                    else
                    {
                        // delete the last fixation start if it was too short
                        protoFixations.RemoveAt(protoFixations.Count - 1);
                    }
                    si = 0 + i;
                }
                else if (!fixstart)
                {
                    si += 1;
                }
            }

            return fixations;
        }

        public Rectangle ToReclangle(int fixationRadius)
        {
            return new Rectangle(this.X - fixationRadius, this.Y - fixationRadius, fixationRadius * 2, fixationRadius * 2);
        }
    }
}
