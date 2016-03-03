using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.EyeMovementsFilters
{
    public class None : Filter
    {
        public override IEnumerable<VolkeEyeSensorMessage> ApplyFilter(IEnumerable<VolkeEyeSensorMessage> eyeMovements)
        {
            return eyeMovements;
        }
    }
}
