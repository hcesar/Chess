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
        public abstract IEnumerable<VolkeEyeSensorMessage> ApplyFilter(IList<VolkeEyeSensorMessage> eyeMovements);
    }
}
