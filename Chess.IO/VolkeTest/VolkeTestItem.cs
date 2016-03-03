using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO.VolkeTest
{
    public class VolkeTestItem
    {
        public IList<VolkeEyeSensorMessage> QuestionEyeMovements { get; set; }
        public IList<VolkeEyeSensorMessage> AnswerEyeMovements { get; set; }

        public VolkeQuestionMessage Question { get; set; }

        public string BoardFEN { get; set; }
    }
}
