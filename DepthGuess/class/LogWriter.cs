using System.Drawing;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>ログを書き出すクラス</summary>
    class LogWriter
    {
        private RichTextBox logTextBox;

        /// <summary>コンストラクタ</summary>
        /// <param name="textBox">テキストボックス</param>
        public LogWriter(ref RichTextBox textBox)
        {
            logTextBox = textBox;
        }

        /// <summary>通常のログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void write(string text)
        {
            write(text, Color.Green);
        }

        /// <summary>エラーログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void writeError(string text)
        {
            write("エラー：" + text, Color.Red);
        }

        private void write(string text, Color color)
        {
            logTextBox.Focus();
            logTextBox.SelectionLength = 0;
            logTextBox.SelectionColor = color;
            logTextBox.AppendText(text + "\n");
            logTextBox.Refresh();
        }

    }
}
