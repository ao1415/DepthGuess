using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * ラベルの情報を管理するクラスが定義されています。
 * 基本的にラベルはこのクラスで管理します。
 * ラベルにアクセスしたい場合は
 * LabelStructure[y,x]
 * でアクセスすることができます。
 */

namespace DepthGuess
{
    /// <summary>ラベル情報を管理するクラス</summary>
    class LabelStructure
    {

        private int[,] label;
        private int max;
        private int min;

        public LabelStructure(int[,] _label)
        {
            label = _label;

            Height = label.GetLength(0);
            Width = label.GetLength(1);

            setMinMax();
        }
        public LabelStructure(int w,int h)
        {
            label = new int[h, w];
            max = min = 0;
            Width = w;
            Height = h;
        }

        public void setMinMax()
        {
            max = int.MinValue;
            min = int.MaxValue;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    max = Math.Max(max, label[y, x]);
                    min = Math.Min(min, label[y, x]);
                }
            }
        }

        public int this[int y, int x]
        {
            set { label[y, x] = value; }
            get { return label[y, x]; }
        }

        public int Max { get { return max; } }
        public int Min { get { return min; } }

        public int Width { get; }
        public int Height { get; }

        /// <summary>ラベルの2次配列を取得します</summary>
        public int[,] Label { get { return label; } }

    }
}
