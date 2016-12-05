using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class Guess01
    {
        LogWriter logWriter;

        public Guess01(LogWriter writer)
        {
            logWriter = writer;
        }

        public LabelStructure GetDepth(LabelStructure label)
        {
            logWriter.Write("深さ推測を行います");

            if (label == null)
            {
                logWriter.WriteError("ラベルデータがありません");
                logWriter.WriteError("深さ推測を中止します");
                return null;
            }

            int[][] link = new RingDetection(logWriter).GetInclusionLink(label);

            int[] depthTable = new int[link.Length];

            for (int i = 0; i < link.Length; i++)
            {
                for (int j = 0; j < link[i].Length; j++)
                {
                    depthTable[link[i][j]]++;
                }
            }

            LabelStructure depthGrid = new LabelStructure(new int[label.Height, label.Width]);

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    int num = label[y, x];
                    depthGrid[y, x] = depthTable[num];
                }
            }

            logWriter.Write("深さ推測を行いました");
            return depthGrid;
        }

    }
}
