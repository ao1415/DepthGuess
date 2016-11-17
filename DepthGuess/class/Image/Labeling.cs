using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    /// <summary>画像のラベルを作成する</summary>
    class Labeling
    {
        LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public Labeling(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>画像からラベルを作成する</summary>
        /// <param name="bmp">ラベルを作成したい画像</param>
        /// <returns>ラベル情報<see cref="int[,]"/></returns>
        public LabelStructure GetLabelTable(Bitmap bmp)
        {
            logWriter.Write("ラベリング処理を行います");

            if (bmp == null)
            {
                logWriter.WriteError("画像が存在しません");
                logWriter.WriteError("ラベリング処理を中止します");
                return null;
            }

            int[,] labelTable = new int[bmp.Height, bmp.Width];
            Color[,] colorTable = new Color[bmp.Height, bmp.Width];
            
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buf = new byte[bmp.Width * bmp.Height * 4];
                Marshal.Copy(data.Scan0, buf, 0, buf.Length);

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int index = y * bmp.Width * 4 + x * 4;
                        colorTable[y, x] = Color.FromArgb(buf[index + 0], buf[index + 1], buf[index + 2]);
                        labelTable[y, x] = -1;
                    }
                }
                bmp.UnlockBits(data);
            }

            int label = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    #region 同色探査部分
                    if (labelTable[y, x] == -1)
                    {
                        Stack<Point> sta = new Stack<Point>();

                        labelTable[y, x] = label;
                        sta.Push(new Point(x, y));

                        while (sta.Count > 0)
                        {
                            Point p = sta.Pop();
                            foreach (var dire in Config.Direction)
                            {
                                Point point = new Point(p.X + dire.X, p.Y + dire.Y);
                                if (0 <= point.X && point.X < bmp.Width && 0 <= point.Y && point.Y < bmp.Height)
                                {
                                    if (labelTable[point.Y, point.X] == -1 && colorTable[y, x] == colorTable[point.Y, point.X])
                                    {
                                        labelTable[point.Y, point.X] = label;
                                        sta.Push(point);
                                    }
                                }
                            }
                        }
                        label++;
                    }
                    #endregion
                }
            }

            logWriter.Write("ラベリング処理が完了しました");
            return new LabelStructure(labelTable);
        }

        /// <summary>画像からラベル情報を視覚化した画像を作成する</summary>
        /// <param name="bmp">視覚化したい画像</param>
        /// <returns>視覚化された画像<see cref="Bitmap"></returns>
        public Bitmap GetLabelImage(Bitmap bmp)
        {
            var label = GetLabelTable(bmp);

            logWriter.Write("ラベルデータの画像作成を行います");

            if (label == null)
            {
                logWriter.WriteError("ラベルデータが存在しません");
                logWriter.WriteError("画像作成を中止します");
                return null;
            }

            Bitmap bitmap = GetLabelImage(label);

            return bitmap;
        }
        /// <summary>ラベル情報を視覚化する</summary>
        /// <param name="label">視覚化したいラベルデータ</param>
        /// <returns>視覚化された画像<see cref="Bitmap"></returns>
        public Bitmap GetLabelImage(LabelStructure label)
        {
            logWriter.Write("ラベルデータの画像作成を行います");

            if (label == null)
            {
                logWriter.WriteError("ラベルデータが存在しません");
                logWriter.WriteError("画像作成を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(label.Width, label.Height);
            int val = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    val = Math.Max(val, label[y, x]);
                }
            }
            val += 1;
            int c = Math.Max(byte.MaxValue / val, 16);

            logWriter.Write("分割数　　=" + val);
            logWriter.Write("色の変化量=" + c);

            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
                Marshal.Copy(data.Scan0, buf, 0, buf.Length);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int index = y * bitmap.Width * 4 + x * 4;
                        int color = (label[y, x] * c) & byte.MaxValue;
                        buf[index + 0] = buf[index + 1] = buf[index + 2] = (byte)color;
                        buf[index + 3] = byte.MaxValue;
                    }
                }

                Marshal.Copy(buf, 0, data.Scan0, buf.Length);
                bitmap.UnlockBits(data);
            }

            logWriter.Write("ラベルデータの画像作成が完了しました");

            return bitmap;
        }

    }
}
