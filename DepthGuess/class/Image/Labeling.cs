using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class Labeling
    {
        LogWriter logWriter;

        public Labeling(LogWriter writer)
        {
            logWriter = writer;
        }

        public int[,] getLabelTable(Bitmap bmp)
        {
            logWriter.write("ラベリング処理を行います");

            if (bmp == null)
            {
                logWriter.writeError("画像が存在しません");
                logWriter.writeError("ラベリング処理を中止します");
                return null;
            }

            int[,] labelTable = new int[bmp.Height, bmp.Width];
            Color[,] colorTable = new Color[bmp.Height, bmp.Width];

            Point[] direction;

            //*
            //八近傍
            direction = new Point[] { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1) };
            /*/
            //四近傍
            direction = new Point[] { new Point(0, -1), new Point(-1, 0), new Point(1, 0), new Point(0, 1) };
            //*/

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    colorTable[y, x] = bmp.GetPixel(x, y);
                    labelTable[y, x] = -1;
                }
            }

            int label = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (labelTable[y, x] == -1)
                    {
                        Stack<Point> sta = new Stack<Point>();

                        labelTable[y, x] = label;
                        sta.Push(new Point(x, y));

                        while (sta.Count > 0)
                        {
                            Point p = sta.Pop();
                            foreach (var dire in direction)
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
                }
            }

            logWriter.write("ラベリング処理が完了しました");
            return labelTable;
        }

        public Bitmap getLabelImage(Bitmap bmp)
        {
            int[,] label = getLabelTable(bmp);

            logWriter.write("ラベルデータの画像作成を行います");

            if (label == null)
            {
                logWriter.writeError("ラベルデータが存在しません");
                logWriter.writeError("画像作成を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height);
            int val = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    val = Math.Max(val, label[y, x]);
                }
            }
            val++;
            int c = Math.Max(byte.MaxValue / val, 16);

            logWriter.write("分割数　　=" + val);
            logWriter.write("色の変化量=" + c);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int color = (label[y, x] * c) & byte.MaxValue;
                    bitmap.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }
            logWriter.write("ラベルデータの画像作成が完了しました");

            return bitmap;
        }
        public Bitmap getLabelImage(int[,] label)
        {
            logWriter.write("ラベルデータの画像作成を行います");

            if (label == null)
            {
                logWriter.writeError("ラベルデータが存在しません");
                logWriter.writeError("画像作成を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(label.GetLength(1), label.GetLength(0));
            int val = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    val = Math.Max(val, label[y, x]);
                }
            }
            val++;
            int c = Math.Max(byte.MaxValue / val, 16);

            logWriter.write("分割数　　=" + val);
            logWriter.write("色の変化量=" + c);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int color = (label[y, x] * c) & byte.MaxValue;
                    bitmap.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }
            logWriter.write("ラベルデータの画像作成が完了しました");

            return bitmap;
        }

    }
}
