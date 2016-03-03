using System;
using System.Collections.Generic;
using System.Text;

namespace LookPoint.Filters
{
    public abstract class WeightOOBase : WeightBase
    {
        private GazePoint iBufferedPoint = null;
        private GazePoint iCurrentFixation = null;

        private float iThreshold = 50;

        public float Threshold { get { return iThreshold; } set { iThreshold = value; } }

        protected override GazePoint Feed(GazePoint aGazePoint)
        {
            if (iCurrentFixation == null)
            {
                iCurrentFixation = aGazePoint;
            }
            else if (iCurrentFixation != null && iBufferedPoint != null)
            {
                double distCF = Dist(iCurrentFixation, aGazePoint);
                double distBP = Dist(iBufferedPoint, aGazePoint);
                if (distCF < distBP)
                {
                    if (distCF < iThreshold)
                    {
                        iCurrentFixation = GetFixation(iOrgGazePoints, aGazePoint);
                        iBufferedPoint = null;
                    }
                    else
                    {
                        iBufferedPoint = aGazePoint;
                        iCurrentFixation = GetFixation(iOrgGazePoints, null);
                    }
                }
                else
                {
                    if (distBP < iThreshold)
                    {
                        iOrgGazePoints.clear();
                        iOrgGazePoints.push(iBufferedPoint);
                        iCurrentFixation = GetFixation(iOrgGazePoints, aGazePoint);
                        iBufferedPoint = null;
                    }
                    else
                    {
                        iCurrentFixation = aGazePoint;//iBufferedPoint;
                        iBufferedPoint = aGazePoint;
                    }
                }
            }
            else
            {
                double distCF = Dist(iCurrentFixation, aGazePoint);
                if (distCF > iThreshold)
                {
                    iBufferedPoint = aGazePoint;
                    iCurrentFixation = GetFixation(iOrgGazePoints, null);
                }
                else
                {
                    iCurrentFixation = GetFixation(iOrgGazePoints, aGazePoint);
                }
            }

            iCurrentFixation.Timestamp = aGazePoint.Timestamp;
            return iCurrentFixation;
        }

        public override string ToString()
        {
            return "Weighted OnOff: " + iProcessor.ToString();
        }

        public override void reset()
        {
            base.reset();
            iCurrentFixation = null;
            iBufferedPoint = null;
        }
    }
}
