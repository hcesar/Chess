using System;
using System.Collections.Generic;
using System.Text;

namespace LookPoint.Filters
{
    public abstract class WeightBase : Filter
    {
        protected WeightProcessor iProcessor;

        public WeightProcessor Processor { get { return iProcessor; } }

        protected GazePoint GetFixation(GazeBuffer aHistory, GazePoint aNewPoint)
        {
            return iProcessor.GetFixation(aHistory, aNewPoint, 0);
        }

    }
}
