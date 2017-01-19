using System;
using System.Drawing;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>
    /// 画像を表示するフォーム
    /// </summary>
    public partial class PictureForm : Form
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">タイトル</param>
        /// <param name="image">画像</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public PictureForm(string text, Image image, LogWriter writer)
        {
            InitializeComponent();

            logWriter = writer;

            saveItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                dialog.FileName = text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;
                    SaveImage saveImage = new SaveImage(logWriter);
                    saveImage.Save(pictureBox.Image, path);
                }
            });
            sizeItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ClientSize = pictureBox.Image.Size;
            });

            pictureBox.Image = image;
            ClientSize = image.Size;

            Text = text;

            Config.StyleSetup(this);

        }
    }
}
