using System;
using System.Drawing;

/*
 * 画像を読み込むクラスが定義されています。
 * Loadにファイルパスを渡すことで、画像が読み込めます。
 */

namespace DepthGuess
{
    /// <summary>ファイルから画像を読み込む</summary>
    class LoadImage
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public LoadImage(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary><see cref="Bitmap"/>形式で画像を読み込む</summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>読み込んだ<see cref="Bitmap"/></returns>
        public Bitmap Load(string path)
        {
            logWriter.Write("画像読み込みを開始します");

            Bitmap bmp;
            try
            {
                bmp = (Bitmap)Image.FromFile(path);
                logWriter.Write("画像を読み込みました");
                return bmp;
            }
            catch (Exception ex)
            {
                logWriter.WriteError("画像読み込みに失敗しました");
                logWriter.WriteError(ex.ToString());
                return null;
            }
        }
        

    }
}
