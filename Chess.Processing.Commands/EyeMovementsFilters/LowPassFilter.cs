using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.EyeMovementsFilters
{
    public class LowPassFilter : Filter
    {
        public override IEnumerable<IO.VolkeTest.VolkeEyeSensorMessage> ApplyFilter(IEnumerable<IO.VolkeTest.VolkeEyeSensorMessage> eyeMovements)
        {
            throw new NotImplementedException();
        }

        protected double Dist(Point aPoint1, Point aPoint2)
        {
            double dx = aPoint1.X - aPoint2.X;
            double dy = aPoint1.Y - aPoint2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        protected double Dist(PointF aPoint1, PointF aPoint2)
        {
            double dx = aPoint1.X - aPoint2.X;
            double dy = aPoint1.Y - aPoint2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }


}
