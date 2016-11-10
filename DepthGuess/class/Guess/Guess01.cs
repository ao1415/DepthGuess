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

        public int[,] getDepth(int[,] label)
        {
            int[,] depth = label;



            return depth;
        }

        private bool hasHole(int[,] label, int number)
        {
            for (int y = 0; y < label.GetLength(0); y++)
            {
                for (int x = 0; x < label.GetLength(1); x++)
                {

                }
            }

            return false;
        }

    }
}
