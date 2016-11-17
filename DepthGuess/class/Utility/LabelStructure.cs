using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class LabelStructure
    {

        private int[,] label;
        private int max;
        private int min;
        private int width;
        private int height;

        public LabelStructure(int[,] _label)
        {
            label = _label;
            max = int.MinValue;
            min = int.MaxValue;

            foreach (var val in label)
            {
                max = Math.Max(max, val);
                min = Math.Min(min, val);
            }

            height = label.GetLength(0);
            width = label.GetLength(1);

        }

        public int this[int y, int x]
        {
            set { label[y, x] = value; }
            get { return label[y, x]; }
        }

        public int Max { get { return max; } }
        public int Min { get { return min; } }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public int[,] Label { get { return label; } }

    }
}
