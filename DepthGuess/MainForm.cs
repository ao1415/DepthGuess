using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthGuess
{
    public partial class MainForm : Form
    {
        private LogWriter logWriter;

        public MainForm()
        {
            InitializeComponent();

            logWriter = new LogWriter(ref logTextBox);
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            logTextBox.Clear();
            LoadImage loadImage = new LoadImage(logWriter);
            Bitmap bmp = loadImage.load(fileTextBox.Text);

            ImageWindow originalImage = new ImageWindow("元画像", bmp, logWriter);
            originalImage.show();

            logWriter.write("処理が完了しました");
        }



    }
}
