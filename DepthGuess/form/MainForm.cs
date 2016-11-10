using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

#if BLACK_STYLE
            styleSetup();
#endif

            logWriter = new LogWriter(ref logTextBox);

        }

        private void styleSetup()
        {
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = SystemColors.Window;

            foreach (var control in Controls)
            {
                var type = control.GetType();

                if (type == typeof(Button))
                {
                    Button btn = (Button)control;
                    btn.BackColor = Color.FromArgb(62, 62, 64);
                    btn.ForeColor = Color.FromArgb(220, 220, 220);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Font = new Font("ＭＳ ゴシック", 9);
                }
                else if (type == typeof(TextBox))
                {
                    TextBox text = (TextBox)control;
                    text.BackColor = Color.FromArgb(37, 37, 38);
                    text.ForeColor = Color.FromArgb(220, 220, 220);
                    text.BorderStyle = BorderStyle.FixedSingle;
                    text.Font = new Font("ＭＳ ゴシック", 9);
                }
                else if (type == typeof(RichTextBox))
                {
                    RichTextBox text = (RichTextBox)control;
                    text.BackColor = Color.FromArgb(40, 40, 42);
                    text.ForeColor = Color.FromArgb(220, 220, 220);
                    text.BorderStyle = BorderStyle.None;
                    text.Font = new Font("ＭＳ ゴシック", 9);
                }
                else if (type == typeof(Label))
                {
                    Label label = (Label)control;
                    label.BackColor = Color.FromArgb(30, 30, 30);
                    label.ForeColor = Color.FromArgb(220, 220, 220);
                    label.Font = new Font("ＭＳ ゴシック", 9);
                }
            }

        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            logTextBox.Clear();

#if DEBUG
            process();
#else
            try
            {
                process();
            }
            catch (Exception ex)
            {
                logWriter.writeError("エラーが発生しました");
                logWriter.writeError(ex.ToString());
                logWriter.writeError("処理を停止します");
            }
#endif
        }

        private void process()
        {
            logWriter.write("処理を開始します");

            LoadImage loadImage = new LoadImage(logWriter);
            SaveImage saveImage = new SaveImage(logWriter);

            MedianCut medianCut = new MedianCut(logWriter);
            SobelFilter sobelFilter = new SobelFilter(logWriter);
            ImageThreshold threshold = new ImageThreshold(logWriter);
            EdgeExtraction edge = new EdgeExtraction(logWriter);
            Labeling labeling = new Labeling(logWriter);

            Bitmap originalImage = loadImage.load(fileTextBox.Text);
            new ImageWindow("元画像", originalImage, logWriter);

            Color[] pallet;
            Bitmap mediancutImage = medianCut.getImage(originalImage, out pallet);
            new ImageWindow("減色画像", mediancutImage, logWriter);

            Bitmap thresholdImage = threshold.getImage(mediancutImage, 128);
            new ImageWindow("二値化画像", thresholdImage, logWriter);

            int[,] label = labeling.getLabelTable(thresholdImage);
            Bitmap labelImage = labeling.getLabelImage(label);
            new ImageWindow("ラベリング画像", labelImage, logWriter);

            saveImage.saveBinary(originalImage, label, "./depthImage.rgbad");

            /*
            Bitmap edgeImage1 = edge.getImage(mediancutImage);
            new ImageWindow("エッジ画像", edgeImage1, logWriter);
            Bitmap edgeImage2 = edge.getImage(originalImage);
            new ImageWindow("エッジ画像(減色なし)", edgeImage2, logWriter);



            Bitmap sobelImage1 = sobelFilter.getImage(mediancutImage);
            new ImageWindow("sobel画像", sobelImage1, logWriter);
            Bitmap sobelImage2 = sobelFilter.getImage(originalImage);
            new ImageWindow("sobel画像(減色なし)", sobelImage2, logWriter);



            Bitmap thresholdImage1 = threshold.getImage(sobelImage1, 127);
            new ImageWindow("二値化エッジ画像", thresholdImage1, logWriter);
            Bitmap thresholdImage2 = threshold.getImage(sobelImage2, 127);
            new ImageWindow("二値化エッジ画像(減色なし)", thresholdImage2, logWriter);
            */



            logWriter.write("処理が完了しました");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;

            fileTextBox.Text = Properties.Settings.Default.FilePath;

            logTextBox.Focus();
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
