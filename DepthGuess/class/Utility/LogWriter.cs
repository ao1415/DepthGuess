using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/*
 * ログを書き出すクラスが定義されています。
 * Writeを呼び出すことでログが一行追加されます。
 * WriteErrorでエラーログが書き出せます。
 */

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
        /// <summary>通常のログを書き出す</summary>
        /// <param name="obj">オブジェクト</param>
        public void Write(object obj) { Write(obj.ToString()); }

        /// <summary>エラーログを書き出す</summary>
        /// <param name="text">メッセージ</param>
        public void WriteError(string text = "")
        {
            string date = GetNow();

            form.BeginInvoke(new Action<string, Color>(Write), new object[] { date + text, Color.Red });
            Console.Error.WriteLine(date + text);
        }
        /// <summary>エラーログを書き出す</summary>
        /// <param name="obj">オブジェクト</param>
        public void WriteError(object obj) { WriteError(obj.ToString()); }

        private void Write(string text, Color color)
        {
            logTextBox.Focus();
            logTextBox.SelectionLength = 0;
            logTextBox.SelectionColor = color;
            logTextBox.AppendText(text + "\n");
            logTextBox.Refresh();
        }

        /// <summary>最後の一行を削除する</summary>
        public void RemoveLine()
        {
            form.BeginInvoke(new Action(() =>
            {
                List<string> lines = new List<string>(logTextBox.Lines);
                var line = lines.Count - 2;
                lines.RemoveAt(line);
                lines.RemoveAt(line);
                logTextBox.Text = string.Join("\n", lines);
                logTextBox.AppendText("\n");
            }));
        }

        /// <summary>現在の時間を取得</summary>
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

        public RichTextBox TextBox { get { return logTextBox; } }
        public Form MainForm { get { return form; } }

    }

    class ProgressWriter : LogWriter
    {
        object o = new object();

        int count = 0;
        int max = 0;

        public ProgressWriter(LogWriter writer, int n) : base(writer.MainForm, writer.TextBox)
        {
            max = n;
        }

        public void Add()
        {
            lock (o)
            {
                count++;
                //if (count % 100 == 0)
                {
                    RemoveLine();
                    Write(count.ToString() + "/" + max.ToString());
                }
            }
        }

    }


}
