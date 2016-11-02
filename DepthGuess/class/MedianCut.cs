using System;
using System.Collections.Generic;
using System.Drawing;

namespace DepthGuess
{
    /// <summary>
    /// <para>メディアンカット</para>
    /// <para>https://www.snip2code.com/Snippet/405733/</para>
    /// </summary>
    class MedianCut
    {

        private static readonly uint ColorNumber = 8;
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public MedianCut(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>減色処理を行う</summary>
        /// <param name="image">入力画像</param>
        /// <param name="selectColors">カラーパレット</param>
        /// <returns>出力画像</returns>
        public Bitmap getImage(Bitmap image, out Color[] selectColors)
        {
            Bitmap img = new Bitmap(image.Size.Width, image.Size.Height);

            int[,,] colorCube = new int[64, 64, 64];
            List<Color> colors = new List<Color>();

            byte maxR, minR, maxG, minG, maxB, minB;
            maxR = maxG = maxB = byte.MinValue;
            minR = minG = minB = byte.MaxValue;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color c = image.GetPixel(x, y);
                    int r = (c.R & 0xfc) >> 2;
                    int g = (c.G & 0xfc) >> 2;
                    int b = (c.B & 0xfc) >> 2;
                    if (colorCube[r, g, b] == 0)
                    {
                        colorCube[r, g, b] = 1;
                        colors.Add(c);

                        maxR = Math.Max(maxR, c.R);
                        minR = Math.Min(minR, c.R);
                        maxG = Math.Max(maxG, c.G);
                        minG = Math.Min(minG, c.G);
                        maxB = Math.Max(maxB, c.B);
                        minB = Math.Min(minB, c.B);
                    }
                }
            }

            List<Color> pallete = new List<Color>();
            pallete.Add(getColorAverage(colors));

            for (int i = 1; i < ColorNumber; i++)
            {
                int large = maxR - minR;
                int middle = (maxR + minR) / 2;
                char colorIndex = 'R';

                if (large < (maxG - minG) * 0.8)
                {
                    large = maxG - minG;
                    middle = (maxG + minG) / 2;
                    colorIndex = 'G';
                }
                if (large < (maxB - minB) * 0.5)
                {
                    large = maxB - minB;
                    middle = (maxB + minB) / 2;
                    colorIndex = 'B';
                }

                List<Color> tmp1, tmp2;
                tmp1 = new List<Color>();
                tmp2 = new List<Color>();
                foreach (var c in colors)
                {
                    int num = 0;
                    switch (colorIndex)
                    {
                        case 'R': num = c.R; break;
                        case 'G': num = c.G; break;
                        case 'B': num = c.B; break;
                    }
                    if (num > middle) tmp1.Add(c);
                    else tmp2.Add(c);
                }

                colors.Clear();

                if (tmp1.Count < tmp2.Count)
                    Swap(ref tmp1, ref tmp2);

                foreach (var c in tmp1)
                    colors.Add(c);

                pallete.Add(getColorAverage(tmp1));
                pallete.Add(getColorAverage(tmp2));

                switch (colorIndex)
                {
                    case 'R': minR = (byte)middle; break;
                    case 'G': minG = (byte)middle; break;
                    case 'B': minB = (byte)middle; break;
                }

            }

            selectColors = new Color[pallete.Count];

            for (int i = 0; i < pallete.Count; i++)
                selectColors[i] = pallete[i];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);

                    double minRange = double.MaxValue;
                    Color nearColor = Color.Black;
                    foreach (var c in pallete)
                    {
                        double r = getColorRange(color, c);
                        if (minRange > r)
                        {
                            minRange = r;
                            nearColor = c;
                        }
                    }

                    img.SetPixel(x, y, nearColor);
                }
            }

            logWriter.write("減色処理が完了しました");
            logWriter.write("パレット数=" + selectColors.Length);
            for (int i = 0; i < selectColors.Length; i++)
                logWriter.write((i + 1) + "番目のパレット=" + selectColors[i]);

            return img;
        }

        private double getColorRange(Color c1, Color c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            return (int)Math.Sqrt(dr * dr + dg * dg + db * db);
        }
        private Color getColorAverage(List<Color> colors)
        {
            if (colors.Count == 0)
                return Color.FromArgb(0, 0, 0);

            int r, g, b;
            r = g = b = 0;
            foreach (var c in colors)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }

            r = r / colors.Count;
            g = g / colors.Count;
            b = b / colors.Count;

            return Color.FromArgb((byte)r, (byte)g, (byte)b);
        }

        private void Swap<T>(ref T LeftParam, ref T RightParam)
        {
            T temp;
            temp = LeftParam;
            LeftParam = RightParam;
            RightParam = temp;
        }
    }
}
