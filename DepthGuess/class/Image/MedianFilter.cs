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
        /// <param name="bmp">入力画像</param>
        /// <returns>出力画像</returns>
        private Bitmap GetImage(Bitmap bmp)
        {
            Mat src = BitmapConverter.ToMat(bmp);
            Mat dst = src.Clone();
            Cv2.MedianBlur(src, dst, 3);

            Bitmap bitmap = dst.ToBitmap();

            return bitmap;
        }
        /// <summary>メディアンフィルタを適用する(非同期)</summary>
        /// <param name="bmp">入力画像</param>
        /// <returns>出力画像</returns>
        public async Task<Bitmap> GetImageAsync(Bitmap bmp) { return await Task.Run(() => GetImage(bmp)); }
        
    }
}
