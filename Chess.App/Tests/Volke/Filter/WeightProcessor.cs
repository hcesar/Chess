using System;
using System.Collections.Generic;
using System.Text;

namespace LookPoint.Filters
{
    public enum ProcessorType
    {
        Linear = 0,
        Triangular,
        Gaussian
    }

    public interface WeightProcessor
    {
        GazePoint GetFixation(GazeBuffer aHistory, GazePoint aNewPoint, int aWindowSize);
    }
}
