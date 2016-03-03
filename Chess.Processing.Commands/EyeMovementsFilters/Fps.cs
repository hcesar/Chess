using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.EyeMovementsFilters
{
    public class Fps : Filter
    {
        private int fps;
        public Fps(int fps)
        {
            if (fps <= 0)
                throw new ArgumentException("Fps must be greater than zero.", "fps");

            this.fps = fps;
        }

        public override IEnumerable<VolkeEyeSensorMessage> ApplyFilter(IEnumerable<VolkeEyeSensorMessage> eyeMovements)
        {
            int frameMS = (int)(1000 / this.fps);

            long nextFrame = 0;
            VolkeEyeSensorMessage lastFrame = null;
            foreach (var movement in eyeMovements)
            {
                if (movement.Timestamp == nextFrame)
                {
                    yield return new VolkeEyeSensorMessage(nextFrame, movement.LeftPosition, movement.RightPosition);
                    nextFrame += frameMS;
                }

                while (movement.Timestamp > nextFrame)
                {
                    yield return new VolkeEyeSensorMessage(nextFrame, lastFrame.LeftPosition, lastFrame.RightPosition);
                    nextFrame += frameMS;
                }

                lastFrame = movement;
            }
        }
    }
}
