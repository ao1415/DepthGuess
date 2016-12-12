using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 色をL*a*b*で表現するためのクラスが定義されています。
 * ColorとL*a*b*を相互に変換できます。
 */

namespace DepthGuess
{
    /// <summary>
    /// L*a*b*(明度、色度、色度)カラーを表します。
    /// </summary>
    class LAB
    {
        private double l;
        private double a;
        private double b;

        public double L { get { return l; } }
        public double A { get { return a; } }
        public double B { get { return b; } }

        private LAB(double _l, double _a, double _b)
        {

        }

        public static LAB FromLAB(double _l, double _a, double _b)
        {
            return new LAB(_l, _a, _b);
        }
        public static LAB FromRGB(Color c)
        {
            return FromRGB(c.R, c.G, c.B);
        }
        public static LAB FromRGB(byte _r, byte _g, byte _b)
        {
            double r = _r / 255.0;
            double g = _g / 255.0;
            double b = _b / 255.0;

            double[] srgb = new double[3];
            srgb[0] = Math.Pow(r, 2.2);
            srgb[1] = Math.Pow(g, 2.2);
            srgb[2] = Math.Pow(b, 2.2);

            double[] xyz = new double[3];
            xyz[0] = 0.3933 * srgb[0] + 0.3651 * srgb[1] + 0.1903 * srgb[2];
            xyz[1] = 0.2123 * srgb[0] + 0.7010 * srgb[1] + 0.0858 * srgb[2];
            xyz[2] = 0.0182 * srgb[0] + 0.1117 * srgb[1] + 0.9570 * srgb[2];

            xyz[0] *= 100;
            xyz[1] *= 100;
            xyz[2] *= 100;

            double Xn = 98.072;
            double Yn = 100.000;
            double Zn = 118.225;
            double x = Math.Pow(xyz[0] / Xn, 1.0 / 3);
            double y = Math.Pow(xyz[1] / Yn, 1.0 / 3);
            double z = Math.Pow(xyz[2] / Zn, 1.0 / 3);

            double[] lab = new double[3];
            lab[0] = 116 * y - 16;
            lab[1] = 500 * (x - y);
            lab[2] = 200 * (y - z);

            return new LAB(lab[0], lab[1], lab[2]);
        }
        public static Color ToRGB(LAB c)
        {
            double y = (c.l + 16) / 116;
            double x = c.a / 500 + y;
            double z = y - c.b / 200;

            double Xn = 98.072;
            double Yn = 100.000;
            double Zn = 118.225;
            double[] xyz = new double[3];
            xyz[0] = Math.Pow(x, 3) * Xn;
            xyz[1] = Math.Pow(y, 3) * Yn;
            xyz[2] = Math.Pow(z, 3) * Zn;

            xyz[0] /= 100;
            xyz[1] /= 100;
            xyz[2] /= 100;

            double[] srgb = new double[3];
            srgb[0] = 3.5064 * xyz[0] - 1.7400 * xyz[1] - 0.5441 * xyz[2];
            srgb[1] = -1.0690 * xyz[0] + 1.9777 * xyz[1] + 0.0352 * xyz[2];
            srgb[2] = 0.0563 * xyz[0] - 0.1970 * xyz[1] + 1.0511 * xyz[2];

            double[] rgb = new double[3];
            rgb[0] = Math.Pow(srgb[0], 1 / 2.2);
            rgb[1] = Math.Pow(srgb[1], 1 / 2.2);
            rgb[2] = Math.Pow(srgb[2], 1 / 2.2);

            byte r = (byte)(rgb[0] * 255);
            byte g = (byte)(rgb[1] * 255);
            byte b = (byte)(rgb[2] * 255);

            return Color.FromArgb(r, g, b);
        }

    }
}
