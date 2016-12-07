using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
 * 画像を明るさに変換するクラスが定義されています。
 * GetBrightnessで明るさのラベルが得られます。
 * 未実装
 */

namespace DepthGuess
{
    class BrightnessConversion
    {
        LogWriter logWriter;

        public BrightnessConversion(LogWriter writer)
        {
            logWriter = writer;
        }

        public LabelStructure GetBrightness(Bitmap bmp)
        {
            LabelStructure label = new LabelStructure(bmp.Width, bmp.Height);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] buf = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            bmp.UnlockBits(data);

            for (int i = 0; i < buf.Length; i += 4)
            {
                HSV hsv = HSV.FromRGB(buf[i + 0], buf[i + 1], buf[i + 2]);
                int x = i / 4 % bmp.Width;
                int y = i / 4 / bmp.Width;

                label[y, x] = (int)(hsv.V * 255);
            }

            return label;
        }

    }
}
