using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DepthGuess
{
    /// <summary>sobelフィルタを画像に適用する</summary>
    class SobelFilter
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public SobelFilter(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>フィルターを適用する</summary>
        /// <param name="image">適用する画像</param>
        /// <returns>適用された<see cref="Bitmap"/></returns>
        public Bitmap getImage(Bitmap image)
        {

            logWriter.write("sobelフィルタを行います");

            if (image == null)
            {
                logWriter.writeError("画像が存在しません");
                logWriter.writeError("sobelフィルタを中止します");
                return null;
            }

            byte[][,] bmp = new byte[3][,];
            for (int i = 0; i < bmp.Length; i++)
                bmp[i] = new byte[image.Height, image.Width];

            Bitmap bitmap = new Bitmap(image);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    //Color c = image.GetPixel(x, y); ;
                    bmp[0][y, x] = buf[y * bitmap.Width * 4 + x * 4 + 0];
                    bmp[1][y, x] = buf[y * bitmap.Width * 4 + x * 4 + 1];
                    bmp[2][y, x] = buf[y * bitmap.Width * 4 + x * 4 + 2];
                }
            }

            double[,] gaussianFilter = new double[3, 3] {
                { 1.0 / 16, 2.0 / 16, 1.0 / 16 },
                { 2.0 / 16, 4.0 / 16, 2.0 / 16 },
                { 1.0 / 16, 2.0 / 16, 1.0 / 16 } };

            byte[,] bmp0 = applyFilter(bmp[0], gaussianFilter);
            byte[,] bmp1 = applyFilter(bmp[1], gaussianFilter);
            byte[,] bmp2 = applyFilter(bmp[2], gaussianFilter);

            byte[,][,] filterBmp = new byte[3, 4][,];

            double[][,] xFilter = new double[2][,];
            double[][,] yFilter = new double[2][,];

            xFilter[0] = new double[3, 3] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 } };
            xFilter[1] = new double[3, 3] {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 } };

            yFilter[0] = new double[3, 3] {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 } };
            yFilter[1] = new double[3, 3] {
                { 1, 2, 1 },
                { 0, 0, 0 },
                { -1, -2, -1 } };

            filterBmp[0, 0] = applyFilter(bmp0, xFilter[0]);
            filterBmp[0, 1] = applyFilter(bmp0, yFilter[0]);
            filterBmp[0, 2] = applyFilter(bmp0, xFilter[1]);
            filterBmp[0, 3] = applyFilter(bmp0, yFilter[1]);

            filterBmp[1, 0] = applyFilter(bmp1, xFilter[0]);
            filterBmp[1, 1] = applyFilter(bmp1, yFilter[0]);
            filterBmp[1, 2] = applyFilter(bmp1, xFilter[1]);
            filterBmp[1, 3] = applyFilter(bmp1, yFilter[1]);

            filterBmp[2, 0] = applyFilter(bmp2, xFilter[0]);
            filterBmp[2, 1] = applyFilter(bmp2, yFilter[0]);
            filterBmp[2, 2] = applyFilter(bmp2, xFilter[1]);
            filterBmp[2, 3] = applyFilter(bmp2, yFilter[1]);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    double r1 = Math.Pow(filterBmp[0, 0][y, x], 2) + Math.Pow(filterBmp[0, 1][y, x], 2);
                    double r2 = Math.Pow(filterBmp[0, 2][y, x], 2) + Math.Pow(filterBmp[0, 3][y, x], 2);

                    double r3 = Math.Pow(filterBmp[1, 0][y, x], 2) + Math.Pow(filterBmp[1, 1][y, x], 2);
                    double r4 = Math.Pow(filterBmp[1, 2][y, x], 2) + Math.Pow(filterBmp[1, 3][y, x], 2);

                    double r5 = Math.Pow(filterBmp[2, 0][y, x], 2) + Math.Pow(filterBmp[2, 1][y, x], 2);
                    double r6 = Math.Pow(filterBmp[2, 2][y, x], 2) + Math.Pow(filterBmp[2, 3][y, x], 2);

                    double r = Math.Sqrt(r1 + r2 + r3 + r4 + r5 + r6);
                    int val = (int)r;
                    val = Math.Min(byte.MaxValue, val);
                    val = Math.Max(byte.MinValue, val);

                    buf[y * bitmap.Width * 4 + x * 4 + 0] = (byte)val;
                    buf[y * bitmap.Width * 4 + x * 4 + 1] = (byte)val;
                    buf[y * bitmap.Width * 4 + x * 4 + 2] = (byte)val;
                }
            }

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);

            logWriter.write("sobelフィルタが完了しました");

            return bitmap;
        }

        private byte[,] applyFilter(byte[,] table, double[,] filter)
        {
            int[,] next = new int[table.GetLength(0), table.GetLength(1)];

            Size filterSize = new Size(filter.GetLength(0) / 2, filter.GetLength(1) / 2);
            
            for (int y = filterSize.Height; y < table.GetLength(0) - filterSize.Height; y++)
            {
                for (int x = filterSize.Width; x < table.GetLength(1) - filterSize.Width; x++)
                {
                    for (int dy = 0; dy < filter.GetLength(0); dy++)
                    {
                        for (int dx = 0; dx < filter.GetLength(1); dx++)
                        {
                            int val = table[y + dy - filterSize.Height, x + dx - filterSize.Width];
                            next[y, x] += (int)(val * filter[dy, dx]);
                        }
                    }
                }
            }

            byte[,] nTable = new byte[table.GetLength(0), table.GetLength(1)];

            for (int y = 0; y < next.GetLength(0); y++)
            {
                for (int x = 0; x < next.GetLength(1); x++)
                {
                    int val = next[y, x];
                    val = Math.Min(byte.MaxValue, val);
                    val = Math.Max(byte.MinValue, val);
                    nTable[y, x] = (byte)val;
                }
            }

            return nTable;
        }

    }
}
