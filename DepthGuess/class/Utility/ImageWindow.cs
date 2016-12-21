using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

/*
 * 画像を表示するウインドウを作成するクラスが定義されています
 * コンストラクタを呼び出すことで、新しくウインドウを作成します
 * ウインドウを右クリックすることで、
 * ウインドウサイズを画像に合わせる
 * 画像を保存する
 * が選択できます
 */

namespace DepthGuess
{
    /// <summary>
    /// 画像を表示するウインドウ
    /// </summary>
    class ImageWindow
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">タイトル</param>
        /// <param name="image">画像</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public ImageWindow(string text, Image image, LogWriter writer)
        {

            logWriter = writer;

            if (image == null)
            {
                logWriter.WriteError(text + "が存在しません");
                logWriter.WriteError("ダミーデータを表示します");
                image = new Bitmap(100, 100);
            }

            logWriter.Write(text + "を表示しました");

            PictureForm form = new PictureForm((string)text.Clone(), (Image)image.Clone(), logWriter);

            //別スレッドで表示を行う
            Thread thread = new Thread(new ParameterizedThreadStart((object data) =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)data);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(form);

        }
        
    }
}
