﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    /// <summary>二値化処理を行う</summary>
    class ImageThreshold
    {
        LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public ImageThreshold(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>二値化処理を行う</summary>
        /// <param name="bmp">処理を行いたい画像</param>
        /// <param name="threshold">閾値</param>
        /// <returns>二値化された<see cref="Bitmap"/></returns>
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