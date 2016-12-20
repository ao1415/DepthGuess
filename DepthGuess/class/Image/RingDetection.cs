using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * ラベル領域の内包関係を調べるクラスが定義されています。
 * GetInclusionLinkでn番目の領域が内包している領域の情報を得られます。
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

            int[][] link = new int[label.Max - label.Min + 1][];

            using (PrograssWindow pw = new PrograssWindow("内包検査", label.Max - label.Min + 1))
            {
                Parallel.For(label.Min, label.Max + 1, (n, state) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        pw.Close();
                        state.Break();
                    }
                    link[n - label.Min] = GetInclusionNumber(label, n);
                    pw.Add();
                });

                pw.Join();
            }

            logWriter.Write("ラベルの内包関係を調べました");
            return link;
        }

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

        private bool IsRing(LabelStructure label, int n)
        {
            logWriter.Write("内包するものがあるか調べます");

            int[] r = GetInclusionNumber(label, n);

            logWriter.Write("内包するものがあるか調べました");
            return r.Length > 0;
        }

    }
}
