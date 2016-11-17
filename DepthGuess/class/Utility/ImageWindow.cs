using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DepthGuess
{
    /// <summary>画像を表示するウインドウ</summary>
    class ImageWindow
    {
        private LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="text">タイトル</param>
        /// <param name="image">画像</param>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public ImageWindow(string text, Image image, LogWriter writer)
        {

            logWriter = writer;

            if (image == null)
            {
                logWriter.WriteError(text + "が存在しません");
                logWriter.WriteError("ダミーデータを表示します");
            }

            logWriter.Write(text + "を表示しました");

            //Form form = new PictureForm(text, image, logWriter);
            //form.Show();
            //form.Refresh();

            PictureForm form = new PictureForm((string)text.Clone(), (Image)image.Clone(), logWriter);
            
            Thread thread = new Thread(new ParameterizedThreadStart((object data) =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)data);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(form);

        }
        
        private class PictureForm : Form
        {

            private LogWriter logWriter;
            public PictureForm(string text, Image image, LogWriter writer)
            {
                InitializeComponent(text, image);
                logWriter = writer;
            }

            /// <summary>
            /// 必要なデザイナー変数です。
            /// </summary>
            private IContainer components = null;

            /// <summary>
            /// 使用中のリソースをすべてクリーンアップします。
            /// </summary>
            /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            /// <summary>
            /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
            /// コード エディターで変更しないでください。
            /// </summary>
            private void InitializeComponent(string text, Image image)
            {
                pictureBox = new PictureBox();
                menu = new ContextMenuStrip();
                saveItem = new ToolStripMenuItem();
                sizeItem = new ToolStripMenuItem();
                dialog = new SaveFileDialog();
                img = image;

                menu.Items.AddRange(new ToolStripMenuItem[] { saveItem, sizeItem });
                menu.Name = "contextMenu";
                menu.Size = new Size(109, 70);

                saveItem.Name = "saveImageMenuItem";
                saveItem.Size = new Size(108, 22);
                saveItem.Text = "名前を付けて保存...";
                saveItem.Click += new EventHandler((object sender, EventArgs e) =>
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.FileName;
                        SaveImage saveImage = new SaveImage(logWriter);
                        saveImage.Save(img, path);
                    }
                });

                sizeItem.Name = "resizeImageMenuItem";
                sizeItem.Size = new Size(108, 22);
                sizeItem.Text = "元の大きさに戻す";
                sizeItem.Click += new EventHandler((object sender, EventArgs e) =>
                {
                    ClientSize = img.Size;
                });


                SuspendLayout();

                pictureBox.Location = new Point(0, 0);
                pictureBox.Name = "pictureBox";
                if (img == null)
                    pictureBox.Size = new Size(200, 200);
                else
                    pictureBox.Size = img.Size;
                pictureBox.Image = img;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

                dialog.FileName = text;
                dialog.Filter = "画像ファイル|*.png";

                AutoScaleDimensions = new SizeF(6F, 12F);
                AutoScaleMode = AutoScaleMode.Font;
                ClientSize = pictureBox.Size;
                Controls.Add(pictureBox);
                ShowIcon = false;
                ContextMenuStrip = menu;
                Name = "ImageForm";
                Text = text;

#if BLACK_STYLE
                BackColor = Color.FromArgb(30, 30, 30);
#endif

                ((ISupportInitialize)(pictureBox)).EndInit();
                menu.ResumeLayout(false);
                ResumeLayout(false);
                PerformLayout();
            }

            private Image img;
            private PictureBox pictureBox;
            private ContextMenuStrip menu;
            private ToolStripMenuItem saveItem;
            private ToolStripMenuItem sizeItem;
            private SaveFileDialog dialog;
        }

    }
}
