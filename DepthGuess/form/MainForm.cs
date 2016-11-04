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
            styleSetup();

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

            try
            {
                LoadImage loadImage = new LoadImage(logWriter);
                Bitmap bmp = loadImage.load(fileTextBox.Text);

                ImageWindow originalImage = new ImageWindow("元画像", bmp, logWriter);
                originalImage.show();

                MedianCut medianCut = new MedianCut(logWriter);
                Color[] pallet;
                Bitmap median = medianCut.getImage(bmp, out pallet);

                ImageWindow medianImage = new ImageWindow("減色画像", median, logWriter);
                medianImage.show();

                SobelFilter sobelFilter = new SobelFilter(logWriter);
                Bitmap sobel = sobelFilter.getImage(median);

                ImageWindow sobelImage = new ImageWindow("エッジ画像", sobel, logWriter);
                sobelImage.show();

                logWriter.write("処理が完了しました");
            }
            catch (Exception ex)
            {
                logWriter.writeError("エラーが発生しました");
                logWriter.writeError(ex.ToString());
                logWriter.writeError("処理を停止します");
            }
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
