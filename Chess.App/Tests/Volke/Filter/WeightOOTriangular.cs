using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LookPoint.Filters
{
    public class WeightOOTriangular : WeightOOBase
    {
        public WeightOOTriangular()
        {
            iProcessor = new WeightProcessors.Triangular();
        }
    }
}
