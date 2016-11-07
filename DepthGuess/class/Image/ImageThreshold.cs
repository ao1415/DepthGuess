using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class ImageThreshold
    {
        LogWriter logWriter;

        public ImageThreshold(LogWriter writer)
        {
            logWriter = writer;
        }

        public Bitmap getImage(Bitmap bmp, byte threshold)
        {
            logWriter.write("二値化処理を行います");

            if (bmp == null)
            {
                logWriter.writeError("画像が存在しません");
                logWriter.writeError("二値化処理を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    if (c.R >= threshold) bitmap.SetPixel(x, y, Color.White);
                    else bitmap.SetPixel(x, y, Color.Black);
                }
            }

            logWriter.write("二値化処理が完了しました");
            return bitmap;
        }

    }
}
