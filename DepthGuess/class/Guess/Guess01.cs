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

        private bool hasHole(int[,] label,int number)
        {

            return false;
        }

    }
}
