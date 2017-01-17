using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * ラベル領域の内包関係を調べるクラスが定義されています
 * GetInclusionLinkでn番目の領域が内包している領域の情報を得られます
 * [0][1]=3ならば、0番の領域が3の領域を内包している
 */

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

            using (PrograssWindow pw = new PrograssWindow("内包検査", label.Max - label.Min + 1))
            {
                Parallel.For(label.Min, label.Max + 1, (n, state) =>
                {
                    link[n - label.Min] = GetInclusionNumber(label, n);
                    pw.Add();
                });

                pw.Join();
            }

            logWriter.Write("ラベルの内包関係を調べました");
            return link;
        }

        public int[][] GetInclusionLink(LabelStructure label, CancellationTokenSource token)
        {
            logWriter.Write("ラベルの内包関係を調べます");

            if (label == null)
            {
                logWriter.WriteError("ラベル情報がありません");
                logWriter.WriteError("処理を中止します");
                return null;
            }

            label.SetMinMax();
            int[][] link = new int[label.Max - label.Min + 1][];

            using (PrograssWindow pw = new PrograssWindow("内包検査", label.Max - label.Min + 1))
            {
                Parallel.For(label.Min, label.Max + 1, (n, state) =>
                {
                    if (token.IsCancellationRequested) state.Break();
                    link[n - label.Min] = GetInclusionNumber3(label, n);
                    pw.Add();
                });
            }

            logWriter.Write("ラベルの内包関係を調べました");
            return link;
        }

        //遅い・不正確
        private int[] GetInclusionNumber(LabelStructure label, int n)
        {
            int[] dx = new int[] { -1, 0, 1, -1 };
            int[] dy = new int[] { -1, -1, -1, 0 };

            Dictionary<int, bool> table = new Dictionary<int, bool>();

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    if (!table.ContainsKey(label[y, x]))
                        table[label[y, x]] = false;

                    if (x - 1 < 0 || label.Width <= x + 1 || y - 1 < 0 || label.Height <= y + 1)
                    {
                        table[label[y, x]] = true;

                        for (int i = 0; i < 4; i++)
                        {
                            if (0 <= x + dx[i] && x + dx[i] < label.Width && 0 <= y + dy[i])
                            {
                                if (label[y + dy[i], x + dx[i]] != n)
                                {
                                    table[label[y + dy[i], x + dx[i]]] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (0 <= x + dx[i] && x + dx[i] < label.Width && 0 <= y + dy[i])
                            {
                                if (label[y + dy[i], x + dx[i]] != n)
                                {
                                    int l = label[y + dy[i], x + dx[i]];
                                    if (table[l])
                                        table[label[y, x]] = true;
                                }
                            }
                        }
                    }
                }
            }

            HashSet<int> inclusion = new HashSet<int>();
            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    if (label[y, x] != n && !table[label[y, x]])
                    {
                        inclusion.Add(label[y, x]);
                    }
                }
            }

            return inclusion.ToArray();
        }
        //早い・不正確
        private int[] GetInclusionNumber2(LabelStructure label, int n)
        {
            Dictionary<int, bool> table = new Dictionary<int, bool>();

            for (int i = label.Min; i <= label.Max; i++)
                table[i] = false;

            for (int x = 0; x < label.Width; x++)
            {
                if (label[0, x] != n)
                    table[label[0, x]] = true;
                if (label[label.Height - 1, x] != n)
                    table[label[label.Height - 1, x]] = true;
            }
            for (int y = 0; y < label.Height; y++)
            {
                if (label[y, 0] != n)
                    table[label[y, 0]] = true;
                if (label[y, label.Width - 1] != n)
                    table[label[y, label.Width - 1]] = true;
            }

            for (int y = 1; y < label.Height - 1; y++)
            {
                for (int x = 1; x < label.Width - 1; x++)
                {
                    if (label[y, x] != n && !table[label[y, x]])
                    {
                        foreach (var d in Config.Direction)
                        {
                            int num = label[y + d.Y, x + d.X];

                            if (table[num])
                            {
                                table[label[y, x]] = true;
                                break;
                            }
                        }
                    }
                }
            }

            List<int> list = new List<int>();
            table[n] = true;

            foreach (var val in table)
            {
                if (!val.Value)
                    list.Add(val.Key);
            }

            return list.ToArray();
        }
        //遅い・正確
        private int[] GetInclusionNumber3(LabelStructure label, int n)
        {
            int[] table = new int[label.Max - label.Min + 1];
            for (int i = 0; i < table.Length; i++) table[i] = i;

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    if (label[y, x] != n)
                    {
                        int min = int.MaxValue;

                        foreach (var d in Config.Direction)
                        {
                            if (0 <= x + d.X && x + d.X < label.Width && 0 <= y + d.Y && y + d.Y < label.Height)
                            {
                                if (label[y + d.Y, x + d.X] != n)
                                    min = Math.Min(min, table[label[y + d.Y, x + d.X]]);
                            }
                        }

                        if (table[label[y, x]] > min)
                        {
                            for (int i = 0; i < table.Length; i++)
                                if (table[i] == table[label[y, x]])
                                    table[i] = min;
                            for (int i = 0; i < table.Length; i++)
                                while (table[i] != table[table[i]])
                                    table[i] = table[table[i]];
                        }
                    }
                }
            }
            
            for (int x = 0; x < label.Width; x++)
            {
                table[label[0, x]] = 0;
                table[label[label.Height - 1, x]] = 0;
            }
            for (int y = 0; y < label.Height; y++)
            {
                table[label[y, 0]] = 0;
                table[label[y, label.Width - 1]] = 0;
            }
            for (int i = 0; i < table.Length; i++)
                while (table[i] != table[table[i]])
                    table[i] = table[table[i]];
            
            List<int> list = new List<int>();
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] != 0 && i != n)
                    list.Add(i);
            }

            return list.ToArray();
        }

        private bool IsRing(LabelStructure label, int n)
        {
            logWriter.Write("内包するものがあるか調べます");

            int[] r = GetInclusionNumber(label, n);

            logWriter.Write("内包するものがあるか調べました");
            return r.Length > 0;
        }

    }
}
