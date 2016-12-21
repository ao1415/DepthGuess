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
    public partial class PictureForm : Form
    {
        private LogWriter logWriter;

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
