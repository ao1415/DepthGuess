using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class EdgeExtraction
    {
        LogWriter logWriter;

        public EdgeExtraction(LogWriter writer)
        {
            logWriter = writer;
        }

        public Bitmap getImage(Bitmap bmp)
        {
            logWriter.write("エッジ抽出を行います");

            if (bmp == null)
            {
                logWriter.writeError("画像が存在しません");
                logWriter.writeError("エッジ抽出を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height);
            Color[,] table = new Color[bmp.Height, bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    table[y, x] = bmp.GetPixel(x, y);
                }
            }

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (x + 1 < bmp.Width && table[y, x + 1] != table[y, x]) bitmap.SetPixel(x, y, Color.White);
                    else if (x - 1 >= 0 && table[y, x - 1] != table[y, x]) bitmap.SetPixel(x, y, Color.White);
                    else if (y + 1 < bmp.Height && table[y + 1, x] != table[y, x]) bitmap.SetPixel(x, y, Color.White);
                    else if (y - 1 >= 0 && table[y - 1, x] != table[y, x]) bitmap.SetPixel(x, y, Color.White);
                    else bitmap.SetPixel(x, y, Color.Black);
                }
            }

            logWriter.write("エッジ抽出が完了しました");
            return bitmap;
        }

    }
}
