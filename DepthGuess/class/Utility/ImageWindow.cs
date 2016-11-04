using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>画像を表示するウインドウ</summary>
    class ImageWindow
    {
        private Form form;
        private Image img;
        private PictureBox pictureBox;
        private ContextMenuStrip menu;
        private ToolStripMenuItem item1;
        private ToolStripMenuItem item2;
        private SaveFileDialog dialog;
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">タイトル</param>
        /// <param name="image">画像</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public ImageWindow(string text, Image image, LogWriter writer)
        {
            InitializeComponent(text, image);
            logWriter = writer;

            if (image == null)
            {
                logWriter.writeError(text + "が存在しません");
                logWriter.writeError("ダミーデータを表示します");
            }
        }

        private void InitializeComponent(string text, Image image)
        {
            form = new Form();
            pictureBox = new PictureBox();
            menu = new ContextMenuStrip();
            item1 = new ToolStripMenuItem();
            item2 = new ToolStripMenuItem();
            dialog = new SaveFileDialog();
            img = image;

            #region フォームの初期設定
            menu.Items.AddRange(new ToolStripMenuItem[] { item1, item2 });
            menu.Name = "contextMenu";
            menu.Size = new Size(109, 70);

            item1.Name = "saveImageMenuItem";
            item1.Size = new Size(108, 22);
            item1.Text = "名前を付けて保存...";
            item1.Click += new EventHandler(saveImage_Click);

            item2.Name = "resizeImageMenuItem";
            item2.Size = new Size(108, 22);
            item2.Text = "元の大きさに戻す";
            item2.Click += new EventHandler(resizeImage_Click);

            form.SuspendLayout();

            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            if (img == null)
                pictureBox.Size = new Size(200, 200);
            else
                pictureBox.Size = img.Size;
            pictureBox.Image = img;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            dialog.Filter = "画像ファイル|*.png";

            form.AutoScaleDimensions = new SizeF(6F, 12F);
            form.AutoScaleMode = AutoScaleMode.Font;
            form.ClientSize = pictureBox.Size;
            form.Controls.Add(pictureBox);
            form.ShowIcon = false;
            form.ContextMenuStrip = menu;
            form.Name = "ImageForm";
            form.Text = text;

#if BLACK_STYLE
            form.BackColor = Color.FromArgb(30, 30, 30);
#endif

            ((ISupportInitialize)(pictureBox)).EndInit();
            menu.ResumeLayout(false);
            form.ResumeLayout(false);
            form.PerformLayout();
#endregion

        }

        /// <summary>ウインドウを表示する</summary>
        public void show()
        {
            logWriter.write(form.Text + "を表示しました");
            form.Show();
            //無理やり表示させる
            form.Refresh();
        }

        private void saveImage_Click(object sender, EventArgs e)
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                img.Save(path);
            }
        }

        private void resizeImage_Click(object sender, EventArgs e)
        {
            form.ClientSize = img.Size;
        }

    }
}
