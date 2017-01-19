using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/*
 * エッジを抽出するクラスが定義されています。
 * GetImageでエッジが抽出された画像を得られます。
 */

namespace DepthGuess
{
    /// <summary>
    /// エッジを抽出する
    /// </summary>
    class EdgeExtraction
    {
        LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public EdgeExtraction(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>エッジを抽出する</summary>
        /// <param name="bmp">エッジ抽出を行いたい画像</param>
        /// <returns>抽出された<see cref="Bitmap"/> </returns>
        public Bitmap GetImage(Bitmap bmp)
        {
            logWriter.Write("エッジ抽出を行います");

            if (bmp == null)
            {
                logWriter.WriteError("画像が存在しません");
                logWriter.WriteError("エッジ抽出を中止します");
                return null;
            }

            Bitmap bitmap = new Bitmap(bmp);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            Color[,] table = new Color[bitmap.Height, bitmap.Width];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int index = y * bitmap.Width * 4 + x * 4;
                    table[y, x] = Color.FromArgb(buf[index + 0], buf[index + 1], buf[index + 2]);
                }
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int index = y * bitmap.Width * 4 + x * 4;

                    if (x + 1 < bitmap.Width && table[y, x + 1] != table[y, x]) buf[index + 0] = buf[index + 1] = buf[index + 2] = byte.MaxValue;
                    else if (x - 1 >= 0 && table[y, x - 1] != table[y, x]) buf[index + 0] = buf[index + 1] = buf[index + 2] = byte.MaxValue;
                    else if (y + 1 < bitmap.Height && table[y + 1, x] != table[y, x]) buf[index + 0] = buf[index + 1] = buf[index + 2] = byte.MaxValue;
                    else if (y - 1 >= 0 && table[y - 1, x] != table[y, x]) buf[index + 0] = buf[index + 1] = buf[index + 2] = byte.MaxValue;
                    else buf[index + 0] = buf[index + 1] = buf[index + 2] = byte.MinValue;
                }
            }

            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);

            logWriter.Write("エッジ抽出が完了しました");
            return bitmap;
        }

    }
}
