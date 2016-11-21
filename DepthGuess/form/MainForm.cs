using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthGuess
{
    public partial class MainForm : Form
    {
        private LogWriter logWriter;

        private Thread processThread;

        public MainForm()
        {
            InitializeComponent();

#if BLACK_STYLE
            StyleSetup();
#endif

            logWriter = new LogWriter(this, logTextBox);

        }

        private void StyleSetup()
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

        private void FileSelectButton_Click(object sender, EventArgs e)
        {
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                fileTextBox.Text = openImageDialog.FileName;
                SaveSettings();
            }
        }
        
        private void StartButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = true;
            startButton.Enabled = false;

            logWriter.Clear();

            Action process = () =>
            {
                logWriter.Write("処理を開始します");

                LoadImage loadImage = new LoadImage(logWriter);
                SaveImage saveImage = new SaveImage(logWriter);

                MedianCut medianCut = new MedianCut(logWriter);
                SobelFilter sobelFilter = new SobelFilter(logWriter);
                ImageThreshold threshold = new ImageThreshold(logWriter);
                EdgeExtraction edge = new EdgeExtraction(logWriter);
                Labeling labeling = new Labeling(logWriter);

                Guess01 guess1 = new Guess01(logWriter);

                Bitmap originalImage = loadImage.Load(fileTextBox.Text);
                new ImageWindow("元画像", originalImage, logWriter);

                Color[] pallet;
                Bitmap mediancutImage = medianCut.GetImage(originalImage, out pallet);
                new ImageWindow("減色画像", mediancutImage, logWriter);

                Bitmap thresholdImage = threshold.GetImage(mediancutImage, 128);
                new ImageWindow("二値化画像", thresholdImage, logWriter);

                LabelStructure label = labeling.GetLabelTable(thresholdImage);
                Bitmap labelImage = labeling.GetLabelImage(label);
                new ImageWindow("ラベリング画像", labelImage, logWriter);

                int[,] depth = guess1.GetDepth(label);

                new ImageWindow("深さ情報", labeling.GetLabelImage(new LabelStructure(depth)), logWriter);

                logWriter.Write("処理が完了しました");

                Action save=() =>
                {
                    BeginInvoke(new Action<Image, int[,]>((Image img, int[,] dth) =>
                    {
                        DialogResult com = MessageBox.Show("3次元画像を保存しますか?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (com == DialogResult.Yes)
                        {
                            SaveFileDialog saveDialog = new SaveFileDialog();
                            saveDialog.Filter = "3次元画像|*.rgbad;*.txt";
                            saveDialog.Title = "保存";
                            saveDialog.DefaultExt = "rgbad";
                            if (saveDialog.ShowDialog() == DialogResult.OK)
                            {
                                string path = Path.GetDirectoryName(saveDialog.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveDialog.FileName);
                                saveImage.Save(originalImage, depth, path + ".txt");
                                saveImage.SaveBinary(originalImage, depth, path + ".rgbad");
                            }
                        }
                    }), new object[] { (Image)originalImage.Clone(), (int[,])depth.Clone() });
                };

                //save();

                BeginInvoke(new Action(() =>
                {
                    stopButton.Enabled = false;
                    startButton.Enabled = true;
                }));
            };

            #region プロセス実行
#if DEBUG
            if (processThread != null && processThread.IsAlive)
            {
                MessageBox.Show("処理が終了していません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                processThread = new Thread(new ThreadStart(process));
                processThread.IsBackground = true;
                processThread.Start();
            }
            //process();
#else
            try
            {
                if (processThread != null && processThread.IsAlive)
                {
                    MessageBox.Show("処理が終了していません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    processThread = new Thread(new ThreadStart(process));
                    processThread.IsBackground = true;
                    processThread.Start();
                }
            }
            catch (Exception ex)
            {
                logWriter.WriteError("エラーが発生しました");
                logWriter.WriteError(ex.ToString());
                logWriter.WriteError("処理を停止します");
            }
#endif
            #endregion

        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (processThread != null && processThread.IsAlive)
                processThread.Abort();

            logWriter.WriteError("処理を中止しました");

            stopButton.Enabled = false;
            startButton.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;

            fileTextBox.Text = Properties.Settings.Default.FilePath;

            logTextBox.Focus();
        }

        bool closeFlag = false;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processThread != null && processThread.IsAlive)
            {
                var com = MessageBox.Show("処理が終了していません\n本当に終了しますか?", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (com == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                processThread.Abort();
                closeFlag = true;
            }
            else if (!closeFlag)
            {
                var com = MessageBox.Show("終了しますか?", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (com == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                closeFlag = true;
            }

            SaveSettings();

            Application.Exit();
        }

        private void SaveSettings()
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

    static class Config
    {
        //*
        //八近傍
        private static Point[] direction = new Point[] { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1) };
        /*/
        //四近傍
        private static Point[] direction = new Point[] { new Point(0, -1), new Point(-1, 0), new Point(1, 0), new Point(0, 1) };
        //*/

        public static Point[] Direction { get { return direction; } }

    }

}
