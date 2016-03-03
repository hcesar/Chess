using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public class RoundMask : Mask
    {
        public RoundMask(int radius, params Color[] colors)
            : base(GetMask(radius), colors)
        {

        }

        static float[,] GetMask(int radius)
        {
            float[,] mask = new float[radius * 2, radius * 2];
            int halfSize = (int)Math.Floor(radius / 2f);

            for (int x = -radius; x < radius; x++)
            {
                for (int y = -radius; y < radius; y++)
                {

                    if (x * x + y * y <= radius * radius)
                        mask[x + radius, y + radius] = 1;
                }
            }

            return mask;
        }
    }
}
