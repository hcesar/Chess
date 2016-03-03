using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public class GaussianRoundMask : Mask
    {
        public GaussianRoundMask(int radius, float sigma = 5, params Color[] colors)
            : base(GetMask(radius, sigma), colors)
        {

        }

        public GaussianRoundMask(Color backgroundColor, int radius, float sigma = 5, params Color[] colors)
            : base(backgroundColor, GetMask(radius, sigma), colors)
        {

        }

        static float[,] GetMask(int radius, double sigma = 3.3)
        {
            float max = 0.0f;

            float[,] mask = new float[radius * 2, radius * 2];
            double s = radius * sigma * sigma;


            for (int x = -radius; x < radius; x++)
            {
                for (int y = -radius; y < radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        double r = Math.Sqrt(x * x + y * y);
                        float value = (float)((Math.Exp(-(r * r) / s)) / (Math.PI * s));
                        mask[x + radius, y + radius] = value;
                        max = Math.Max(value, max);
                    }
                }
            }

            int normalizeToValue = 100;

            // normalize the Kernel
            int size = (int)Math.Sqrt(mask.Length);
            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    mask[i, j] = (mask[i, j] / max) * normalizeToValue;

            return mask;
        }
    }
}
