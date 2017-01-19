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
    /// <summary>
    /// 画像を減色するクラス
    /// </summary>
    class K_means
    {
        private LogWriter logWriter;

        /// <summary>
        /// 色の属性
        /// </summary>
        private List<Point3D>[] colors;
        /// <summary>
        /// クラスタ
        /// </summary>
        private Point3D[] center;

        /// <summary>コンストラクタ</summary>
        /// <param name="colorNum">減色数</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public K_means(int colorNum, LogWriter writer)
        {
            logWriter = writer;
            colors = new List<Point3D>[colorNum];
        }

        /// <summary>クラスタの重心を求める</summary>
        /// <returns>各クラスタの重心</returns>
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

        /// <summary>色情報を入力する</summary>
        /// <param name="buf">カラー配列</param>
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

        /// <summary>色の属性の再配置</summary>
        /// <param name="centroids">クラスタの重心</param>
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

        /// <summary>色の距離を取得する</summary>
        /// <param name="p1">色</param>
        /// <param name="p2">色</param>
        /// <returns>色の距離</returns>
        private double Range(Point3D p1, Point3D p2)
        {
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            double z = p1.Z - p2.Z;

            return x * x + y * y + z * z;
        }

        /// <summary>クラスタの変化を返す</summary>
        /// <param name="p1">直前のクラスタ</param>
        /// <param name="p2">直後のクラスタ</param>
        /// <returns>変化していなければtrue, それ以外はfalse</returns>
        private bool Diff(Point3D[] p1, Point3D[] p2)
        {
            double max = 0;
            for (int i = 0; i < p1.Length; i++)
                max = Math.Max(max, Range(p1[i], p2[i]));
            return max < 1;
        }

        /// <summary>カラー配列の置き換え</summary>
        /// <param name="buf">カラー配列</param>
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

        /// <summary>減色した画像を得る</summary>
        /// <param name="bmp">減色したい画像</param>
        /// <returns>減色された画像</returns>
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

        /// <summary>減色した画像を得る</summary>
        /// <param name="bmp">減色したい画像</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>減色された画像</returns>
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
        /// <summary>減色した画像を得る</summary>
        /// <param name="bmp">減色したい画像</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>減色された画像</returns>
        public async Task<Bitmap> GetImageAsync(Bitmap bmp, CancellationTokenSource token) { return await Task.Run(() => GetImage(bmp, token)); }

    }
}
