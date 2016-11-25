using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess.Class.Image
{
    class MedianFilter
    {
        private LogWriter logWriter;

        public MedianFilter(LogWriter writer)
        {
            logWriter = writer;
        }

        public Bitmap GetImage(Bitmap bmp)
        {
            Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height);



            return bitmap;
        }

        public LabelStructure GetLabel(LabelStructure label)
        {
            LabelStructure l = new LabelStructure(label.Width, label.Height);

            return l;
        }


    }
}
