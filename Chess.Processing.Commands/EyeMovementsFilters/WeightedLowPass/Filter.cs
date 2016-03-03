using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;

namespace LookPoint.Filters
{
    /// <remarks>
    /// The class that represents a gaze point via its timestamp, x and y
    /// </remarks>
    public class GazePoint
    {
        private int iX;
        private int iY;
        private int iTimestamp;

        /// <summary>
        /// Gaze X
        /// </summary>
        public int X { get { return iX; } set { iX = value; } }
        /// <summary>
        /// Gaze Y
        /// </summary>
        public int Y { get { return iY; } set { iY = value; } }
        /// <summary>
        /// Timestamp (ms)
        /// </summary>
        public int Timestamp { get { return iTimestamp; } set { iTimestamp = value; } }
        /// <summary>
        /// Gaze location
        /// </summary>
        public Point Location { get { return new Point(iX, iY); } }

        /// <summary>
        /// Creates a new gaze point
        /// </summary>
        public GazePoint()
        {
        }

        /// <summary>
        /// Creates a new gaze point
        /// </summary>
        /// <param name="aPoint">Gaze point</param>
        /// <param name="aTimestamp">Timestamp (ms)</param>
        public GazePoint(Point aPoint, int aTimestamp)
        {
            iX = aPoint.X;
            iY = aPoint.Y;
            iTimestamp = aTimestamp;
        }

        /// <summary>
        /// Creates a new gaze point
        /// </summary>
        /// <param name="aX">Gaze X</param>
        /// <param name="aY">Gaze Y</param>
        /// <param name="aTimestamp">Timestamp (ms)</param>
        public GazePoint(int aX, int aY, int aTimestamp)
        {
            iX = aX;
            iY = aY;
            iTimestamp = aTimestamp;
        }
    }

    public class Buffer<T>
    {
        protected List<T> iBuffer = new List<T>();

        public T[] Items
        {
            get { return iBuffer.ToArray(); }
        }

        public T Last
        {
            get { return iBuffer.Count > 0 ? iBuffer[iBuffer.Count - 1] : default(T); }
        }

        public Buffer()
        {
        }

        protected virtual void VerifyRange()
        {
        }

        public virtual void push(T aObject)
        {
            iBuffer.Add(aObject);
            VerifyRange();
        }

        public void clear()
        {
            iBuffer.Clear();
        }
    }

    public class GazeBuffer : Buffer<GazePoint>,  IEnumerable<GazePoint>
    {
        public class GazeBufferEnumerator : IEnumerator<GazePoint>
        {
            private int iIndex = -1;
            private List<GazePoint> iBuffer;

            public GazePoint Current
            {
                get { return iIndex < 0 || iIndex >= iBuffer.Count ? null : iBuffer[iIndex]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public GazeBufferEnumerator(List<GazePoint> aBuffer)
            {
                iBuffer = aBuffer;
            }

            public bool MoveNext()
            {
                iIndex++;
                return iIndex < iBuffer.Count;
            }

            public void Reset()
            {
                iIndex = -1;
            }

            void IDisposable.Dispose() { }
        }

        private int iTimeWindow = 500;

        public int TimeWindow
        {
            get { return iTimeWindow; }
            set
            {
                iTimeWindow = value;
                VerifyRange();
            }
        }

        public Point Average
        {
            get
            {
                double x = 0.0;
                double y = 0.0;
                foreach (GazePoint gt in iBuffer)
                {
                    x += gt.X;
                    y += gt.Y;
                }
                if (iBuffer.Count > 0)
                {
                    x /= iBuffer.Count;
                    y /= iBuffer.Count;
                }
                return new Point((int)x, (int)y);
            }
        }

        protected override void VerifyRange()
        {
            if (iBuffer.Count > 1)
            {
                int lastTimestamp = iBuffer[iBuffer.Count - 1].Timestamp;
                int toRemove = 0;
                foreach (GazePoint gp in iBuffer)
                {
                    if (lastTimestamp - gp.Timestamp > iTimeWindow)
                        toRemove++;
                    else
                        break;
                }
                if (toRemove > 0)
                    iBuffer.RemoveRange(0, toRemove);
            }
        }

        public IEnumerator<GazePoint> GetEnumerator()
        {
            return new GazeBufferEnumerator(iBuffer);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class Filter
    {
        #region Members

        protected GazeBuffer iOrgGazePoints = new GazeBuffer();
        protected GazeBuffer iFltGazePoints = new GazeBuffer();

        protected int iStartTime;
        protected int iLastTimestamp;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets gaze point living interval in the filter's buffer
        /// </summary>
        public int TimeWindow 
        { 
            get { return iOrgGazePoints.TimeWindow; }
            set { iOrgGazePoints.TimeWindow = value; iFltGazePoints.TimeWindow = value; } 
        }

        #endregion

        /// <summary>
        /// Creates a new filter
        /// </summary>
        public Filter()
        {
            
        }

        protected double Dist(GazePoint aPoint1, GazePoint aPoint2)
        {
            double dx = aPoint1.X - aPoint2.X;
            double dy = aPoint1.Y - aPoint2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        protected double Dist(Point aPoint1, Point aPoint2)
        {
            double dx = aPoint1.X - aPoint2.X;
            double dy = aPoint1.Y - aPoint2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Takes a gaze poing and apply a filtering method
        /// </summary>
        /// <param name="aTimestamp">Timestamp</param>
        /// <param name="aGazePoint">X, Y</param>
        /// <return>Filtered point</return>
        protected abstract GazePoint Feed(GazePoint aGazePoint);

        /// <summary>
        /// Called at the beginning of each trial
        /// </summary>
        public virtual void reset()
        {
            iOrgGazePoints.clear();
            iFltGazePoints.clear();

            iStartTime = -1;
            iLastTimestamp = 0;
        }

        /// <summary>
        /// Takes a gaze poing and apply a filtering method
        /// </summary>
        /// <param name="aTimestamp">Timestamp</param>
        /// <param name="aGazePoint">X, Y</param>
        /// <return>Gaze point</return>
        public Point Feed(int aTimestamp, Point aGazePoint)
        {
            if (iStartTime < 0)
            {
                iStartTime = aTimestamp;
                iLastTimestamp = aTimestamp;
            }

            GazePoint orgGazePoint = new GazePoint(aGazePoint, aTimestamp);
            GazePoint newGazePoint = Feed(orgGazePoint);

            iOrgGazePoints.push(orgGazePoint);
            iFltGazePoints.push(newGazePoint);

            iLastTimestamp = aTimestamp;

            return newGazePoint.Location;
        }
    }
}
