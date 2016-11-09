using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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

            Bitmap bitmap = new Bitmap(bmp);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            for (int i = 0; i < buf.Length; i += 4)
            {
                if (buf[i] >= threshold)
                {
                    buf[i + 0] = buf[i + 1] = buf[i + 2] = 255;
                    buf[i + 3] = 255;
                }
                else
                {
                    buf[i + 0] = buf[i + 1] = buf[i + 2] = 0;
                    buf[i + 3] = 255;
                }
            }

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);

            logWriter.write("二値化処理が完了しました");
            return bitmap;
        }

    }
}
