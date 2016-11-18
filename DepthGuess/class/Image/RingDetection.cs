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

        public int[][] GetInclusionLink(LabelStructure label)
        {
            logWriter.Write("ラベルの内包関係を調べます");

            if (label == null)
            {
                logWriter.WriteError("ラベル情報がありません");
                logWriter.WriteError("処理を中止します");
                return null;
            }

            int[][] link = new int[label.Max - label.Min + 1][];

            for (int n = label.Min; n <= label.Max; n++)
            {
                link[n - label.Min] = GetInclusionNumber(label, n);
            }

            logWriter.Write("ラベルの内包関係を調べました");
            return link;
        }

        private int[] GetInclusionNumber(LabelStructure label, int n)
        {

            Func<int, int, HashSet<int>> inside = (int x, int y) =>
            {
                bool[,] checkTable = new bool[label.Height, label.Width];

                Stack<Point> sta = new Stack<Point>();

                HashSet<int> labelTable = new HashSet<int>();

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
                                if (label[pos.Y, pos.X] != n)
                                {
                                    checkTable[pos.Y, pos.X] = true;
                                    sta.Push(pos);
                                    labelTable.Add(label[pos.Y, pos.X]);
                                }
                            }
                        }
                        else
                        {
                            return new HashSet<int>();
                        }
                    }
                }

                return labelTable;
            };

            HashSet<int> list = new HashSet<int>();
            HashSet<int> check = new HashSet<int>();
            check.Add(n);

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    if (check.Add(label[y, x]))
                    {
                        var table = inside(x, y);
                        foreach (int v in table)
                        {
                            list.Add(v);
                            check.Add(v);
                        }
                    }
                }
            }
            
            return list.Distinct().ToArray();
        }

        public bool IsRing(LabelStructure label, int n)
        {
            logWriter.Write("内包するものがあるか調べます");

            int[] r = GetInclusionNumber(label, n);

            logWriter.Write("内包するものがあるか調べました");
            return r.Length > 0;
        }

    }
}
