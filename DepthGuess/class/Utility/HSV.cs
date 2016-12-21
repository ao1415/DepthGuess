using System;
using System.Drawing;

/*
 * 色をHSVで表現するためのクラスが定義されています
 * ColorとHSVを相互に変換できます
 */

namespace DepthGuess
{
    /// <summary>
    /// HSV(色相、彩度、明度)カラーを表します
    /// </summary>
    class HSV
    {
        /// <summary>
        /// 色相
        /// </summary>
        public double H { get; }
        /// <summary>
        /// 彩度
        /// </summary>
        public double S { get; }
        /// <summary>
        /// 明度
        /// </summary>
        public double V { get; }

        /// <summary>コンストラクタ</summary>
        /// <param name="h">色相</param>
        /// <param name="s">彩度</param>
        /// <param name="v">明度</param>
        private HSV(double h, double s, double v)
        {
            H = RoundDegree(h);
            S = Math.Max(0, Math.Min(1, s));
            V = Math.Max(0, Math.Min(1, v));
        }

        /// <summary>文字列に変換します</summary>
        /// <returns>文字列<see cref="string"/></returns>
        public override string ToString()
        {
            return "HSV[H=" + H.ToString() + ", S=" + S.ToString() + ", V=" + V.ToString() + "]";
        }

        /// <summary>色相を一定値におさめる</summary>
        /// <param name="x">色相</param>
        /// <returns>0～360に収まった値<see cref="double"/></returns>
        private double RoundDegree(double x)
        {
            if (x >= 0)
                return x % 360;
            else
                return 360 - (-x % 360);
        }

        /// <summary>HSVクラスを作成する</summary>
        /// <param name="h">色相</param>
        /// <param name="s">彩度</param>
        /// <param name="v">明度</param>
        /// <returns>HSVの色<see cref="HSV"/></returns>
        public static HSV FromHSV(double h, double s, double v)
        {
            return new HSV(h, s, v);
        }

        /// <summary>HSVクラスを作成する</summary>
        /// <param name="c">色</param>
        /// <returns>HSVの色<see cref="HSV"/></returns>
        public static HSV FromRGB(Color c)
        {
            return FromRGB(c.R, c.G, c.B);
        }

        /// <summary>HSVクラスを作成する</summary>
        /// <param name="_r">赤色</param>
        /// <param name="_g">緑色</param>
        /// <param name="_b">青色</param>
        /// <returns>HSVの色<see cref="HSV"/></returns>
        public static HSV FromRGB(byte _r, byte _g, byte _b)
        {
            double r = _r / 255.0;
            double g = _g / 255.0;
            double b = _b / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double v = max;
            double h, s;

            if (max == min)
            {
                h = 0;
                s = 0;
            }
            else
            {
                double d = max - min;
                if (max == r)
                    h = (g - b) / d;
                else if (max == g)
                    h = (b - r) / d + 2;
                else
                    h = (r - g) / d + 4;
                h *= 60;

                s = d / max;
            }

            return new HSV(h, s, v);
        }

        /// <summary>Colorクラスを作成する</summary>
        /// <param name="c">色</param>
        /// <returns>RGBの色<see cref="Color"/></returns>
        public static Color ToRGB(HSV c)
        {
            double v = c.V;
            double s = c.S;
            double r, g, b;
            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                double h = c.H / 60;
                int i = (int)Math.Floor(h);
                double f = h - i;
                double p = v * (1 - s);
                double q;
                if (i % 2 == 0)
                    q = v * (1 - (1 - f) * s);
                else
                    q = v * (1 - f * s);

                switch (i)
                {
                    case 0:
                        r = v;
                        g = q;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = q;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = q;
                        g = p;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                    default:
                        throw new ArgumentException("色相の値が不正です", "HSV");
                }
            }
            return Color.FromArgb((int)Math.Round(r * 255f), (int)Math.Round(g * 255f), (int)Math.Round(b * 255f));
        }

    }
}
