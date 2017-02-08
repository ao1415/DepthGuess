using System;
using System.Threading;
using System.Windows.Forms;

/*
 * 処理の経過を表示するウィンドウを作成するクラスが定義されています
 * Addでカウントアップできます
 */

namespace DepthGuess
{
    /// <summary>
    /// 処理の経過を表示する
    /// </summary>
    class PrograssWindow : IDisposable
    {
        private Thread thread;
        private PrograssForm form;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">ウィンドウのタイトル</param>
        /// <param name="max">カウントの最大値</param>
        public PrograssWindow(string text, int max)
        {
            form = new PrograssForm(text, max);

            //PrograssFormを別スレッドで立ち上げる
            thread = new Thread(new ParameterizedThreadStart((object data) =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)data);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(form);

        }

        /// <summary>
        /// カウンタを+1する
        /// </summary>
        public void Add()
        {
            if (!form.IsDisposed)
                form.BeginInvoke(new Action(() =>{form.Add();}));
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void Close()
        {
            if (!form.IsDisposed)
                form.BeginInvoke(new Action(() => { form.Close(); }));
        }

        /// <summary>
        /// ウィンドウの終了を待つ
        /// </summary>
        public void Join()
        {
            thread.Join();
        }

        private bool disposed = false;
        ~PrograssWindow()
        {
            Dispose(false);
        }

        /// <summary>
        /// このクラスを破棄する
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このクラスを破棄する
        /// </summary>
        protected void Dispose(bool isDisposing)
        {
            if (!disposed)
            {
                if (isDisposing)
                {
                    if (thread.IsAlive)
                    {
                        Close();
                        Join();
                    }
                }
                disposed = true;
            }
        }

    }
}
