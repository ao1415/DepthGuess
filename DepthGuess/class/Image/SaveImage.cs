using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    class SaveImage
    {
        LogWriter logWriter;

        public SaveImage(LogWriter writer)
        {
            logWriter = writer;
        }

        public void save(Image image, string path)
        {
            logWriter.write("画像を保存します");

            try
            {
                image.Save(path);
            }
            catch (Exception)
            {
                logWriter.writeError("画像の保存に失敗しました");
                return;
            }

            logWriter.write("画像を保存しました");
            logWriter.write("path=" + path);
        }

        public void save(Bitmap image, int[,] depth, string path)
        {
            logWriter.write("三次元画像を保存します");

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(image.Width + " " + image.Height);
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color c = image.GetPixel(x, y);
                            sw.WriteLine(c.R + " " + c.G + " " + c.B + " " + depth[y, x]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                logWriter.writeError("三次元画像の保存に失敗しました");
                return;
            }

            logWriter.write("三次元画像を保存しました");
            logWriter.write("path=" + path);
        }

    }
}
