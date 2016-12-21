using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/*
 * ログを書き出すクラスが定義されています
 * Writeを呼び出すことでログが一行追加されます
 * WriteErrorでエラーログが書き出せます
 */

namespace DepthGuess
{
    /// <summary>
    /// ログを書き出すクラス
    /// </summary>
    public class LogWriter
    {
        public RichTextBox LogTextBox { get; }
        public Form MainForm { get; }

        /// <summary>コンストラクタ</summary>
        /// <param name="textBox">テキストボックス</param>
        public LogWriter(Form mainForm, RichTextBox textBox)
        {
            LogTextBox = textBox;
            MainForm = mainForm;
        }

        /// <summary>通常のログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void Write(string text = "")
        {
            string date = GetNow();

            MainForm.BeginInvoke(new Action<string, Color>(Write), new object[] { date + text, Color.Green });
            Console.Out.WriteLine(date + text);
        }
        /// <summary>通常のログを書き出す</summary>
        /// <param name="obj">オブジェクト</param>
        public void Write(object obj) { Write(obj.ToString()); }

        /// <summary>エラーログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void WriteError(string text = "")
        {
            string date = GetNow();

            MainForm.BeginInvoke(new Action<string, Color>(Write), new object[] { date + text, Color.Red });
            Console.Error.WriteLine(date + text);
        }
        /// <summary>エラーログを書き出す</summary>
        /// <param name="obj">オブジェクト</param>
        public void WriteError(object obj) { WriteError(obj.ToString()); }

        private void Write(string text, Color color)
        {
            LogTextBox.Focus();
            LogTextBox.SelectionLength = 0;
            LogTextBox.SelectionColor = color;
            LogTextBox.AppendText(text + "\n");
            LogTextBox.Refresh();
        }

        /// <summary>
        /// 最後の一行を削除する
        /// </summary>
        public void RemoveLine()
        {
            MainForm.BeginInvoke(new Action(() =>
            {
                List<string> lines = new List<string>(LogTextBox.Lines);
                var line = lines.Count - 2;
                lines.RemoveAt(line);
                lines.RemoveAt(line);
                LogTextBox.Text = string.Join("\n", lines);
                LogTextBox.AppendText("\n");
            }));
        }

        /// <summary>
        /// 現在の時間を取得
        /// </summary>
        private string GetNow()
        {
            string now = DateTime.Now.ToString("[HH:mm:ss.");
            int ms = DateTime.Now.Millisecond;
            now += string.Format("{0:000}]", ms);
            return now;
        }

        /// <summary>
        /// テキストボックスの文字列を全削除する
        /// </summary>
        public void Clear()
        {
            MainForm.BeginInvoke(new Action(() => { LogTextBox.Clear(); }));
        }

        /// <summary>
        /// テキストボックスを強制的に再描写する
        /// </summary>
        public void Refresh()
        {
            MainForm.BeginInvoke(new Action(() => { LogTextBox.Refresh(); }));
        }
        
    }

}
