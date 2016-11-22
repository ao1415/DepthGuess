using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>ログを書き出すクラス</summary>
    class LogWriter
    {
        private RichTextBox logTextBox;
        private Form form;

        /// <summary>コンストラクタ</summary>
        /// <param name="textBox">テキストボックス</param>
        public LogWriter(Form mainForm, RichTextBox textBox)
        {
            logTextBox = textBox;
            form = mainForm;
        }

        /// <summary>通常のログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void Write(string text = "")
        {
            string date = GetNow();

            form.BeginInvoke(new Action<string, Color>(Write), new object[] { date + text, Color.Green });
            Console.Out.WriteLine(date + text);
        }
        public void Write(object obj) { Write(obj.ToString()); }

        /// <summary>エラーログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void WriteError(string text = "")
        {
            string date = GetNow();

            form.BeginInvoke(new Action<string, Color>(Write), new object[] { date + text, Color.Red });
            Console.Error.WriteLine(date + text);
        }
        public void WriteError(object obj) { WriteError(obj.ToString()); }

        private void Write(string text, Color color)
        {
            logTextBox.Focus();
            logTextBox.SelectionLength = 0;
            logTextBox.SelectionColor = color;
            logTextBox.AppendText(text + "\n");
            logTextBox.Refresh();
        }

        public void RemoveLine()
        {
            form.BeginInvoke(new Action(() =>
            {
                List<string> lines = new List<string>(logTextBox.Lines);
                var line = lines.Count - 2;
                lines.RemoveAt(line);
                logTextBox.Text = string.Join("\n", lines);
            }));
        }

        private string GetNow()
        {
            string now = DateTime.Now.ToString("[HH:mm:ss.");
            int ms = DateTime.Now.Millisecond;
            now += string.Format("{0:000}]", ms);
            return now;
        }

        /// <summary>テキストボックスの文字列を全削除する</summary>
        public void Clear()
        {
            form.BeginInvoke(new Action(() => { logTextBox.Clear(); }));
        }

        /// <summary>テキストボックスを強制的に再描写する</summary>
        public void Refresh()
        {
            form.BeginInvoke(new Action(() => { logTextBox.Refresh(); }));
        }

        public Form MainForm { get { return form; } }

    }
}
