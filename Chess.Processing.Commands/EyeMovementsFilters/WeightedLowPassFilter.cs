using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.EyeMovementsFilters
{
    public class WeightedLowPassFilter : LowPassFilter
    {
        public override IEnumerable<IO.VolkeTest.VolkeEyeSensorMessage> ApplyFilter(IEnumerable<IO.VolkeTest.VolkeEyeSensorMessage> eyeMovements)
        {
            LookPoint.Filters.WeightOOTriangular filterR = new LookPoint.Filters.WeightOOTriangular();
            LookPoint.Filters.WeightOOTriangular filterL = new LookPoint.Filters.WeightOOTriangular();

            var buffer = new Queue<BufferPoint>();
            BufferPoint lastPoint = BufferPoint.Empty;
            foreach (var eyeTracking in eyeMovements)
            {
                var point = new BufferPoint(eyeTracking.LeftPosition, eyeTracking.RightPosition, (int)eyeTracking.Timestamp);

                //if (point.HasEmpty())
                //{
                //    buffer.Enqueue(point);
                //    continue;
                //}


                if (buffer.Count > 0)
                {
                    int intervalSize = buffer.Count;
                    var start = lastPoint == BufferPoint.Empty ? point : lastPoint;
                    var next = lastPoint == BufferPoint.Empty ? point : lastPoint;

                    int idx = 0;
                    while (buffer.Count > 0)
                    {
                        var midPoint = buffer.Dequeue();
                        Correct(midPoint, start, next, intervalSize, ++idx);
                        yield return point.ToSensorMessage(filterL, filterR);
                    }
                }

                yield return point.ToSensorMessage(filterL, filterR);
                lastPoint = point;
            }
        }

        private void Correct(BufferPoint point, BufferPoint last, BufferPoint next, int intervalSize, int index)
        {
            point.Left = Correct(point.Left, last.Left, next.Left, intervalSize, index);
            point.Right = Correct(point.Right, last.Right, next.Right, intervalSize, index);
        }

        private Point Correct(Point point, Point last, Point next, int intervalSize, int index)
        {
            if (point.X != 0 || point.Y != 0)
                return point;

            int stepX = (next.X - last.X) / (intervalSize + 1);
            int stepY = (next.Y - last.Y) / (intervalSize + 1);

            return new Point(last.X + (stepX * index), last.Y + (stepY * index));
        }


        private struct BufferPoint
        {
            public static readonly BufferPoint Empty = new BufferPoint();

            public BufferPoint(Point left, Point right, int timestamp)
            {
                this.Left = left;
                this.Right = right;
                this.Timestamp = timestamp;
            }

            public Point Left;
            public Point Right;
            public int Timestamp;

            public bool HasEmpty()
            {
                return this.Left == Point.Empty || this.Right == Point.Empty;
            }

            public IO.VolkeTest.VolkeEyeSensorMessage ToSensorMessage(LookPoint.Filters.WeightOOTriangular leftFilter, LookPoint.Filters.WeightOOTriangular rightFilter)
            {
                var pointWL = leftFilter.Feed(this.Timestamp, this.Left);
                var pointWR = rightFilter.Feed(this.Timestamp, this.Right);

                return new IO.VolkeTest.VolkeEyeSensorMessage(this.Timestamp, pointWL, pointWR);
            }

            public static bool operator ==(BufferPoint p1, BufferPoint p2)
            {
                return p1.Left == p2.Left && p1.Right == p2.Right;
            }

            public static bool operator !=(BufferPoint p1, BufferPoint p2)
            {
                return p1.Left != p2.Left || p1.Right != p2.Right;
            }

            public override int GetHashCode()
            {
                return this.Left.GetHashCode() + this.Right.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return this == (BufferPoint)obj;
            }
        }
    }
}
