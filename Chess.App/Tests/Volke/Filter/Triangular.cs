using System;
using System.Collections.Generic;
using System.Text;

namespace LookPoint.Filters.WeightProcessors
{
    public class Triangular : WeightProcessor
    {
        public GazePoint GetFixation(GazeBuffer aHistory, GazePoint aNewPoint, int aWindowSize)
        {
            double x = 0.0;
            double y = 0.0;
            double count = 0.0;

            int w = 1;
            if (aHistory != null)
            {
                foreach (GazePoint pt in aHistory)
                {
                    if (aWindowSize > 0)
                    {
                        GazePoint refPoint = aNewPoint != null ? aNewPoint : aHistory.Last;
                        if (refPoint.Timestamp - pt.Timestamp > aWindowSize)
                            continue;
                    }
                    x += w * pt.X;
                    y += w * pt.Y;
                    count += w;
                    w++;
                }
            }
            if (aNewPoint != null)
            {
                x += w * aNewPoint.X;
                y += w * aNewPoint.Y;
                count += w;
            }

            if (count != 0.0)
            {
                x /= count;
                y /= count;
            }

            return new GazePoint((float)x, (float)y, 0);
        }

        public override string ToString()
        {
            return "Triangular";
        }
    }
}
