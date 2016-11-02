using System;
using System.Drawing;
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

            MedianCut medianCut = new MedianCut(logWriter);
            Color[] pallet;
            Bitmap median = medianCut.getImage(bmp, out pallet);

            ImageWindow medianImage = new ImageWindow("減色画像", median, logWriter);
            medianImage.show();

            logWriter.write("処理が完了しました");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;

            fileTextBox.Text = Properties.Settings.Default.FilePath;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                Properties.Settings.Default.Bounds = Bounds;
            else
                Properties.Settings.Default.Bounds = RestoreBounds;

            Properties.Settings.Default.WindowState = WindowState;

            Properties.Settings.Default.FilePath = fileTextBox.Text;

            Properties.Settings.Default.Save();
        }
    }
}
