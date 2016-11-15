using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class RingDetection
    {
        private LogWriter logWriter;

        public RingDetection(LogWriter writer)
        {
            logWriter = writer;
        }

        public int[][] getInclusionLink(LabelStructure label)
        {
            logWriter.write("ラベルの内包関係を調べます");

            if (label == null)
            {
                logWriter.writeError("ラベル情報がありません");
                logWriter.writeError("処理を中止します");
                return null;
            }

            int[][] link = new int[label.Max - label.Min][];

            for (int n = label.Min; n < label.Max; n++)
            {
                link[n - label.Min] = getInclusionNumber(label, n);
            }

            logWriter.write("ラベルの内包関係を調べました");
            return link;
        }

        private int[] getInclusionNumber(LabelStructure label, int n)
        {

            List<int> list = new List<int>();
            HashSet<int> check = new HashSet<int>();

            Func<int, int, bool> inside = (int x, int y) =>
            {
                bool[,] checkTable = new bool[label.Height, label.Width];

                Stack<Point> sta = new Stack<Point>();

                checkTable[y, x] = true;
                sta.Push(new Point(x, y));

                while (sta.Count > 0)
                {
                    var point = sta.Pop();

                    foreach (var dire in Config.Direction)
                    {
                        Point pos = new Point(point.X + dire.X, point.Y + dire.Y);

                        if (0 <= pos.X && pos.X < label.Width && 0 <= pos.Y && pos.Y < label.Height)
                        {
                            if (!checkTable[pos.Y, pos.X])
                            {
                                if (label[y, x] != n)
                                {
                                    checkTable[pos.Y, pos.X] = true;
                                    sta.Push(pos);
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return true;
            };

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    if (check.Add(label[y, x]))
                    {
                        if (inside(x, y))
                        {
                            list.Add(label[y, x]);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public bool isRing(LabelStructure label, int n)
        {
            logWriter.write("内包するものがあるか調べます");

            int[] r = getInclusionNumber(label, n);

            logWriter.write("内包するものがあるか調べました");
            return r.Length > 0;
        }

    }
}
