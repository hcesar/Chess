using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.EyeMovementsFilters
{
    public abstract class Filter
    {
        public abstract IEnumerable<VolkeEyeSensorMessage> ApplyFilter(IEnumerable<VolkeEyeSensorMessage> eyeMovements);

        internal static Filter Combine(Filter first, Filter second)
        {
            if (first is None)
                return second;
            else if (second is None)
                return first;
            else
                return new CombinedFilter(first, second);
        }

        private class CombinedFilter : Filter
        {
            private Filter first;
            private Filter second;

            public CombinedFilter(Filter first, Filter second)
            {
                this.first = first;
                this.second = second;
            }

            public override IEnumerable<VolkeEyeSensorMessage> ApplyFilter(IEnumerable<VolkeEyeSensorMessage> eyeMovements)
            {
                eyeMovements = first.ApplyFilter(eyeMovements);
                return second.ApplyFilter(eyeMovements);
            }
        }
    }
}
