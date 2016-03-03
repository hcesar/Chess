using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public class Mask
    {
        public static readonly Color[] DefaultColors = new[] { Color.Green, Color.Yellow, Color.Red };
        public static readonly Color[] GrayscaleColors = new[] { Color.Black, Color.White };

        private Color backgroundColor;
        private float[,] mask;
        private Color[] colors;

        public Mask(Color backgroundColor, float[,] mask, params Color[] colors)
        {
            if (colors.Length == 0)
                this.colors = new[] { Color.Black, Color.White };
            else
                this.colors = colors;

            this.backgroundColor = backgroundColor;
            this.mask = mask;
        }
        public Mask(float[,] mask, params Color[] colors)
            : this(Color.Transparent, mask, colors)
        {
        }

        public void ApplyMask(Bitmap img, bool weightOnDuration, bool renderSaliencyMap, params Point[] points)
        {
            var fixations = points.Select(p => new Fixation(0L, 1L, p.X, p.Y)).ToArray();
            ApplyMask(img, weightOnDuration, renderSaliencyMap, fixations);
        }

        public void ApplyMask(Bitmap img, bool weightOnDuration, bool renderSaliencyMap, params Fixation[] points)
        {
            var maxDuration = points.Max(f => f.Duration);
            var size = mask.GetUpperBound(0) + 1; //Assume square mask
            float max = mask.Cast<float>().Max();

            float[,] exposures = new float[img.Width, img.Height];
            foreach (var point in points)
            {
                var startX = point.X - (size / 2);
                var startY = point.Y - (size / 2);
                var endX = point.X + (size / 2) + (size % 2 == 0 ? 0 : 1);
                var endY = point.Y + (size / 2) + (size % 2 == 0 ? 0 : 1);

                float weight = weightOnDuration ? Math.Max(0.6f, point.Duration / maxDuration) : 1;
                int imgWidth = img.Width;
                int imgHeight = img.Height;
                for (int x = Math.Max(0, startX); x < Math.Min(imgWidth - 1, endX); x++)
                    for (int y = Math.Max(0, startY); y < Math.Min(imgHeight - 1, endY); y++)
                        exposures[x, y] += (mask[x - startX, y - startY] * weight);
            }

            if (renderSaliencyMap)
            {
                Parallel.For(15, 815, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (x) =>
                {
                    for (int y = 0; y < 800; y++)
                    {
                        float exposure = exposures[x, y];
                        if (exposure / max > 0.02f)
                            continue ;

                        lock (img)
                            img.SetPixel(x, y, Color.Black);
                    }
                });

                return;
            }


            int height = img.Height;
            Parallel.For(15, 815, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (x) =>
            {
                for (int y = 0; y < 800; y++)
                {
                    float exposure = exposures[x, y];


                    // if (exposure / max <= 0.05f)
                    //    exposure = 0;

                    // if (exposure > 0 && exposure / max <= 0.12f)
                    //    exposure = (0.05f * max);

                    Color pixel;
                    lock (img)
                        pixel = img.GetPixel(x, y);

                    Color color;
                    if (!renderSaliencyMap && exposure / max < 0.02f)
                    {
                        if (this.backgroundColor == Color.Transparent)
                            continue;

                        color = MergeColor(pixel, this.backgroundColor);
                    }
                    else
                    {
                        color = GetColor(pixel, exposure, max, renderSaliencyMap);
                    }

                    lock (img)
                        img.SetPixel(x, y, color);
                }
            });
        }

        private Color GetColor(Color color, double exposure, float max, bool renderSaliencyMap)
        {
            if (renderSaliencyMap && exposure / max < 0.02f)
                return Color.Black;

            var heatMapColor = GetHeatMapColor((float)Math.Min(1, exposure / max));
            return MergeColor(color, heatMapColor);
        }

        private Color MergeColor(Color color1, Color color2)
        {
            int r = (int)(color1.R * 0.5 + color2.R * 0.5);
            int g = (int)(color1.G * 0.5 + color2.G * 0.5);
            int b = (int)(color1.B * 0.5 + color2.B * 0.5);

            return Color.FromArgb(255, r, g, b);
        }

        private Color GetHeatMapColor(float value)
        {
            //float[][] colors = new[] { new[] { 0f, 0, 0 }, new[] { 1f, 1f, 1 } };
            int NUM_COLORS = this.colors.Length;


            // A static array of 4 colors:  (blue,   green,  yellow,  red) using {r,g,b} for each.

            int idx1;        // |-- Our desired color will be between these two indexes in "color".
            int idx2;        // |
            float fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

            if (value <= 0) { idx1 = idx2 = 0; }    // accounts for an input <=0
            else if (value >= 1) { idx1 = idx2 = NUM_COLORS - 1; }    // accounts for an input >=0
            else
            {
                value = value * (NUM_COLORS - 1);        // Will multiply value by 3.
                idx1 = (int)Math.Floor(value);                  // Our desired color will be after this index.
                idx2 = idx1 + 1;                        // ... and before this index (inclusive).
                fractBetween = value - (float)idx1;    // Distance between the two indexes (0-1).
            }

            try
            {
                float red = ((colors[idx2].R - colors[idx1].R) * fractBetween + colors[idx1].R);
                float green = ((colors[idx2].G - colors[idx1].G) * fractBetween + colors[idx1].G);
                float blue = ((colors[idx2].B - colors[idx1].B) * fractBetween + colors[idx1].B);

                return Color.FromArgb(255, (int)red, (int)green, (int)blue);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
