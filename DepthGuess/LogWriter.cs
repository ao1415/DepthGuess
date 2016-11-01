using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthGuess
{
    class LogWriter
    {
        private RichTextBox logTextBox;

        public LogWriter(ref RichTextBox textBox)
        {
            logTextBox = textBox;
        }

        public void write(string text)
        {
            write(text, Color.Green);
        }

        public void writeError(string text)
        {
            write("エラー：" + text, Color.Red);
        }

        private void write(string text, Color color)
        {
            logTextBox.SelectionLength = 0;
            logTextBox.SelectionColor = color;
            logTextBox.SelectedText = text + "\n";
        }

    }
}
