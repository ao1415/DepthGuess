using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DepthGuess
{
    class K_means
    {
        private LogWriter logWriter;
        private List<Point3D>[] colors;
        private Point3D[] center;

        public K_means(int colorNum, LogWriter writer)
        {
            logWriter = writer;
            colors = new List<Point3D>[colorNum];
        }

        private Point3D[] GetAverage()
        {
            Point3D[] points = new Point3D[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                points[i].X = points[i].Y = points[i].Z = 0;
                for (int j = 0; j < colors[i].Count; j++)
                {
                    points[i].X += colors[i][j].X;
                    points[i].Y += colors[i][j].Y;
                    points[i].Z += colors[i][j].Z;
                }
                if (colors[i].Count > 0)
                {
                    points[i].X = Math.Round(points[i].X / colors[i].Count);
                    points[i].Y = Math.Round(points[i].Y / colors[i].Count);
                    points[i].Z = Math.Round(points[i].Z / colors[i].Count);
                }
            }

            return points;
        }

        private void SetColor(byte[] buf)
        {
            Random rnd = new Random();
            List<Point3D>[] colorLists = new List<Point3D>[colors.Length];
            for (int i = 0; i < colorLists.Length; i++) colorLists[i] = new List<Point3D>();

            HashSet<Point3D> hash = new HashSet<Point3D>();

            for (int i = 0; i < buf.Length; i += 4)
            {
                int n = rnd.Next(0, colors.Length);

                LAB lab = LAB.FromRGB(buf[i + 0], buf[i + 1], buf[i + 2]);
                Point3D point = new Point3D(lab.L, lab.A, lab.B);
                if (hash.Add(point))
                {
                    colorLists[n].Add(point);
                }
            }

            for (int i = 0; i < colors.Length; i++)
                colors[i] = colorLists[i];

        }

        private void SetColor(Point3D[] centroids)
        {
            List<Point3D>[] colorLists = new List<Point3D>[colors.Length];
            for (int i = 0; i < colorLists.Length; i++) colorLists[i] = new List<Point3D>();

            for (int i = 0; i < colors.Length; i++)
            {
                for (int j = 0; j < colors[i].Count; j++)
                {

                    int min = 0;
                    double minR = double.MaxValue;
                    for (int k = 0; k < centroids.Length; k++)
                    {
                        double r = Range(colors[i][j], centroids[k]);
                        if (minR > r)
                        {
                            min = k;
                            minR = r;
                        }
                    }
                    colorLists[min].Add(colors[i][j]);
                }
            }

            for (int i = 0; i < colors.Length; i++)
                colors[i] = colorLists[i];

        }

        private double Range(Point3D p1, Point3D p2)
        {
            //*
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            double z = p1.Z - p2.Z;
            /*/
            HSV h1 = HSV.FromRGB((byte)p1.X, (byte)p1.Y, (byte)p1.Z);
            HSV h2 = HSV.FromRGB((byte)p2.X, (byte)p2.Y, (byte)p2.Z);
            double dx = Math.Abs(h1.H - h2.H);
            double x = Math.Min(dx, 360 - dx);
            double y = (h1.S - h2.S) * 180;
            double z = (h1.V - h2.V) * 180;
            //*/
            return x * x + y * y + z * z;
            //return Math.Sqrt(x * x + y * y + z * z);
        }

        private bool Diff(Point3D[] p1, Point3D[] p2)
        {
            double max = 0;
            for (int i = 0; i < p1.Length; i++)
                max = Math.Max(max, Range(p1[i], p2[i]));
            return max < 1;
        }

        private void Replace(byte[] buf)
        {

            for (int i = 0; i < buf.Length; i += 4)
            {
                LAB lab = LAB.FromRGB(buf[i + 0], buf[i + 1], buf[i + 2]);
                Point3D c = new Point3D(lab.L, lab.A, lab.B);

                int min = 0;
                double minR = double.MaxValue;
                for (int k = 0; k < center.Length; k++)
                {
                    double r = Range(c, center[k]);
                    if (minR > r)
                    {
                        min = k;
                        minR = r;
                    }
                }

                Color color = LAB.ToRGB(LAB.FromLAB(center[min].X, center[min].Y, center[min].Z));

                buf[i + 0] = color.R;
                buf[i + 1] = color.G;
                buf[i + 2] = color.B;
            }

        }

        public Bitmap GetImage(Bitmap bmp)
        {
            logWriter.Write("k-means法を開始します");

            center = new Point3D[colors.Length];

            Bitmap bitmap = new Bitmap(bmp);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            SetColor(buf);

            for (int i = 0; i < 100; i++)
            {
                var centroids = GetAverage();
                SetColor(centroids);

                if (Diff(center, centroids))
                {
                    center = centroids;
                    break;
                }
                center = centroids;
            }

            Replace(buf);

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);

            logWriter.Write("k-means法が完了しました");

            return bitmap;
        }

        private Bitmap GetImage(Bitmap bmp, CancellationTokenSource token)
        {
            logWriter.Write("k-means法を開始します");

            center = new Point3D[colors.Length];

            Bitmap bitmap = new Bitmap(bmp);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            SetColor(buf);

            for (int i = 0; i < 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    bitmap.UnlockBits(data);
                    return null;
                }

                var centroids = GetAverage();
                SetColor(centroids);

                if (Diff(center, centroids))
                {
                    center = centroids;
                    break;
                }
                center = centroids;
            }


            Replace(buf);

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);

            logWriter.Write("k-means法が完了しました");

            return bitmap;
        }
        public async Task<Bitmap> GetImageAsync(Bitmap bmp, CancellationTokenSource token) { return await Task.Run(() => GetImage(bmp, token)); }

    }
}
