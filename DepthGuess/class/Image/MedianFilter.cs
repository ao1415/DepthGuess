using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * 画像にメディアンフィルタをかけるクラスが定義されています。
 * GetImageでフィルタをかけた画像を得られます。
 */

namespace DepthGuess
{
    /// <summary>メディアンフィルタを適用するクラス</summary>
    class MedianFilter
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public MedianFilter(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>メディアンフィルタを適用する</summary>
        /// <param name="bmp">入力画像</param>
        /// <returns>出力画像</returns>
        public Bitmap GetImage(Bitmap bmp)
        {
            Mat src = BitmapConverter.ToMat(bmp);
            Mat dst = src.Clone();
            Cv2.MedianBlur(src, dst, 3);

            Bitmap bitmap = dst.ToBitmap();

            return bitmap;
        }

        //未実装
        public LabelStructure GetLabel(LabelStructure label)
        {
            LabelStructure l = new LabelStructure(label.Width, label.Height);

            return l;
        }


    }
}
