using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Threading.Tasks;

/*
 * 画像にメディアンフィルタをかけるクラスが定義されています
 * GetImageでフィルタをかけた画像を得られます
 */

namespace DepthGuess
{
    /// <summary>
    /// メディアンフィルタを適用するクラス
    /// </summary>
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
        /// <param name="bmp">入力画像</param>C:\Users\i1211408\Documents\Visual Studio 2015\Projects\DepthGuess\DepthGuess\Class\Image\RingDetection.cs
        /// <returns>出力画像</returns>
        private Bitmap GetImage(Bitmap bmp)
        {
            Mat src = BitmapConverter.ToMat(bmp);
            Mat dst = src.Clone();
            Cv2.MedianBlur(src, dst, 3);

            Bitmap bitmap = dst.ToBitmap();

            return bitmap;
        }
        public async Task<Bitmap> GetImageAsync(Bitmap bmp) { return await Task.Run(() => GetImage(bmp)); }


        //未実装
        public LabelStructure GetLabel(LabelStructure label)
        {
            LabelStructure l = new LabelStructure(label.Width, label.Height);

            return l;
        }


    }
}
