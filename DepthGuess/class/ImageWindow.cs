﻿using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>画像を表示するウインドウ</summary>
    class ImageWindow
    {
        private Form form;
        private PictureBox pictureBox;
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">タイトル</param>
        /// <param name="image">画像</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public ImageWindow(string text, Image image, LogWriter writer)
        {
            InitializeComponent(text, image);
            logWriter = writer;
        }

        private void InitializeComponent(string text, Image image)
        {
            form = new Form();
            pictureBox = new PictureBox();
            form.SuspendLayout();

            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = image.Size;
            pictureBox.Image = image;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            form.AutoScaleDimensions = new SizeF(6F, 12F);
            form.AutoScaleMode = AutoScaleMode.Font;
            form.ClientSize = image.Size;
            form.Controls.Add(pictureBox);
            form.Name = "ImageForm";
            form.Text = text;
            ((ISupportInitialize)(pictureBox)).EndInit();
            form.ResumeLayout(false);
            form.PerformLayout();
        }

        /// <summary>ウインドウを表示する</summary>
        public void show()
        {
            logWriter.write(form.Text + "を表示しました");
            form.Show();
            form.Refresh();
        }

    }
}
