using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * MainFormの動作を作成する場所です。
 * ここで、ボタンやテキストボックスの動作を指定しています。
 * デバッグモードで実行する場合はなるべく、デバッグありで実行してください。
 * (上に表示された開始ボタン or デバッグ->デバッグの開始 or F5)
 */

//コメントアウトのテクニック

//*
namespace DepthGuess { }
/*/
namespace DepthGuess { }
//*/

/*
 * このようなコメントは、「//*」を「/*」に変更することでコメント切り替えることができます。
 * このソースコード上にいくつか存在するので、切り替えたい場合は上記の方法で切り替えてください。
 */

namespace DepthGuess
{
    /// <summary>
    /// すべての処理の実行・管理を行うクラス
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// ログ表示用のクラス
        /// </summary>
        private LogWriter logWriter;

        /// <summary>
        /// 画像処理用のスレッド
        /// </summary>
        private Thread processThread;

        /// <summary>
        /// クラスの初期化を行います。
        /// </summary>
        public MainForm()
        {
            //Visual Studioが自動生成したコードの初期化
            //編集しないこと
            InitializeComponent();

            //BLACK_STYLEが指定されている場合StyleSetup()が呼び出されます
#if BLACK_STYLE
            StyleSetup();
#endif

            logWriter = new LogWriter(this, logTextBox);

        }

        /// <summary>
        /// フォームのデザインを黒基調に変更します。
        /// </summary>
        private void StyleSetup()
        {
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = SystemColors.Window;

            //MainFormのすべてのコントロールを列挙する
            foreach (var control in Controls)
            {
                //コントロールのタイプを取得
                //タイプで、どのコントロールか判別する
                var type = control.GetType();

                //各種設定
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

        /// <summary>
        /// ファイルの選択のボタンを押されたときに、実行される関数です。
        /// </summary>
        private void FileSelectButton_Click(object sender, EventArgs e)
        {
            //ファイル選択のダイアログを表示する
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                //選択されたファイルをテキストボックスに表示する
                fileTextBox.Text = openImageDialog.FileName;

                SaveSettings();
            }
        }

        /// <summary>
        /// StartButton_Clickから呼び出される関数です。
        /// </summary>
        private void Process()
        {
            logWriter.Write("処理を開始します");


            ImageThreshold threshold = new ImageThreshold(logWriter);

            Bitmap originalImage = new LoadImage(logWriter).Load(fileTextBox.Text);
            new ImageWindow("元画像", originalImage, logWriter);
            if (originalImage == null) return;

            //Color[] pallet;
            //Bitmap mediancutImage = new MedianCut(8, logWriter).GetImage(originalImage, out pallet);
            //new ImageWindow("減色画像(メディアン)", mediancutImage, logWriter);

            new ImageWindow("減色画像(k-means 32)", new K_means(16, logWriter).GetImage(originalImage), logWriter);
            //new ImageWindow("減色画像(k-means 16)", new K_means(16, logWriter).GetImage(originalImage), logWriter);
            //new ImageWindow("減色画像(k-means 8)", new K_means(8, logWriter).GetImage(originalImage), logWriter);
            //new ImageWindow("減色画像(k-means 4)", new K_means(4, logWriter).GetImage(originalImage), logWriter);
            //new ImageWindow("減色画像(k-means 2)", new K_means(2, logWriter).GetImage(originalImage), logWriter);

            //LabelStructure brightnessLabel = new BrightnessConversion(logWriter).GetBrightness(originalImage);
            //Bitmap brightnessImage = new Labeling(logWriter).GetLabelImage(brightnessLabel);
            //new ImageWindow("元画像の輝度画像", brightnessImage, logWriter);

            //LabelStructure brightnessLabel2 = new BrightnessConversion(logWriter).GetBrightness(mediancutImage);
            //Bitmap brightnessImage2 = new Labeling(logWriter).GetLabelImage(brightnessLabel2);
            //new ImageWindow("減色画像の輝度画像", brightnessImage2, logWriter);

            //Bitmap mediancutImage2 = new MedianCut(8, logWriter).GetImage(brightnessImage, out pallet);
            //new ImageWindow("元画像の輝度画像の減色画像", mediancutImage2, logWriter);

            //Bitmap thresholdImage = threshold.GetImage(mediancutImage, 128);
            //new ImageWindow("二値化画像", thresholdImage, logWriter);

            //ノイズ除去がしたい

            //LabelStructure label = new Labeling(logWriter).GetLabelTable(mediancutImage);
            //Bitmap labelImage = new Labeling(logWriter).GetLabelImage(label);
            //new ImageWindow("ラベリング画像", labelImage, logWriter);

            //LabelStructure depth = new Guess01(logWriter).GetDepth(label);

            //Bitmap depthImage = new Labeling(logWriter).GetLabelImage(depth);
            //new ImageWindow("深さ情報", depthImage, logWriter);

            logWriter.Write("処理が完了しました");
            /*
            Action save = () =>
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
                            SaveImage saveImage = new SaveImage(logWriter);
                            saveImage.Save(originalImage, depth, path + ".txt");
                            saveImage.SaveBinary(originalImage, depth, path + ".rgbad");
                        }
                    }
                }), new object[] { (Image)originalImage.Clone(), (int[,])depth.Label.Clone() });
            };

            save();
            */
            BeginInvoke(new Action(() =>
            {
                stopButton.Enabled = false;
                startButton.Enabled = true;
            }));
        }

        /// <summary>
        /// もう一つのProcessです。
        /// 呼び出したいときには上記のProcessに1を付けて、下記のプロセスの2を消してください。
        /// </summary>
        private void Process2()
        {

            Bitmap originalImage = new LoadImage(logWriter).Load(fileTextBox.Text);
            new ImageWindow("元画像", originalImage, logWriter);

            if (originalImage == null) return;

            Mat src = BitmapConverter.ToMat(originalImage);
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat binary = gray.Threshold(0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);

            ConnectedComponents cc = Cv2.ConnectedComponentsEx(binary);

            Mat dst = new Mat();
            cc.RenderBlobs(dst);

            Image grayImage = BitmapConverter.ToBitmap(dst);
            new ImageWindow("Gray", grayImage, logWriter);
        }

        /// <summary>
        /// 開始のボタンを押されたときに、実行される関数です。。
        /// </summary>
        private void StartButton_Click(object sender, EventArgs e)
        {
            //停止ボタンを有効にします。
            stopButton.Enabled = true;
            //開始ボタンを無効にします。
            startButton.Enabled = false;

            logWriter.Clear();

            //ここでProcess関数を呼び出します。
            //並列処理がわからない場合は変更しないでください。
            //また、デバッグモードとリリースモードで例外処理を変更しています。
            #region プロセス実行
#if DEBUG
            //プロセスが実行中か確認
            if (processThread != null && processThread.IsAlive)
            {
                //実行中だった
                MessageBox.Show("処理が終了していません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //新しいプロセスの発行
                processThread = new Thread(new ThreadStart(Process));
                processThread.IsBackground = true;
                processThread.Start();
            }
#else
            //例外処理を行う
            try
            {
                //プロセスが実行中か確認
                if (processThread != null && processThread.IsAlive)
                {
                    //実行中だった
                    MessageBox.Show("処理が終了していません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    //新しいプロセスの発行
                    processThread = new Thread(new ThreadStart(Process));
                    processThread.IsBackground = true;
                    processThread.Start();
                }
            }
            catch (Exception ex)
            {
                //例外の内容をログで表示
                logWriter.WriteError("エラーが発生しました");
                logWriter.WriteError(ex.ToString());
                logWriter.WriteError("処理を停止します");
            }
#endif
            #endregion

        }
        /// <summary>
        /// 停止のボタンを押されたときに、実行される関数です
        /// </summary>
        private void StopButton_Click(object sender, EventArgs e)
        {
            //プロセスが実行中ならば終了させる。
            if (processThread != null && processThread.IsAlive)
                processThread.Abort();

            logWriter.WriteError("処理を中止しました");

            //停止ボタンを無効にします。
            stopButton.Enabled = false;
            //開始ボタンを有効にします。
            startButton.Enabled = true;
        }

        /// <summary>
        /// フォームが読み込まれるときに、実行される関数です。
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //以前のフォームの状態に戻します。
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;

            fileTextBox.Text = Properties.Settings.Default.FilePath;

            logTextBox.Focus();
        }

        /// <summary>
        /// 2重確認防止のための変数。
        /// </summary>
        bool closeFlag = false;
        /// <summary>
        /// フォームが終了する前に、実行される関数です。
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //プロセスの状態を確認します。
            if (processThread != null && processThread.IsAlive)
            {
                //確認を取ります。
                var com = MessageBox.Show("処理が終了していません\n本当に終了しますか?", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (com == DialogResult.Cancel)
                {
                    //フォームが終了するのをキャンセルします。
                    e.Cancel = true;
                    return;
                }
                else
                {
                    //プロセスを終了します。
                    processThread.Abort();
                    closeFlag = true;
                }
            }
            else if (!closeFlag)
            {
                //確認を取ります。
                var com = MessageBox.Show("終了しますか?", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (com == DialogResult.Cancel)
                {
                    //フォームが終了するのをキャンセルします。
                    e.Cancel = true;
                    return;
                }
                else
                {
                    closeFlag = true;
                }
            }

            SaveSettings();

            //アプリケーションを終了させます。
            Application.Exit();
        }

        /// <summary>
        /// フォームの状態を保存します。
        /// </summary>
        private void SaveSettings()
        {
            //ウインドウの大きさ・場所
            if (WindowState == FormWindowState.Normal)
                Properties.Settings.Default.Bounds = Bounds;
            else
                Properties.Settings.Default.Bounds = RestoreBounds;

            //ウインドウの状態
            Properties.Settings.Default.WindowState = WindowState;

            //ファイルのパス
            Properties.Settings.Default.FilePath = fileTextBox.Text;

            //保存
            Properties.Settings.Default.Save();

        }

    }

    /// <summary>
    /// 処理系統の変更を一括して行うためのクラス
    /// </summary>
    static class Config
    {
        //*
        //八近傍
        private static System.Drawing.Point[] direction =
            new System.Drawing.Point[] {
                new System.Drawing.Point(-1, -1),
                new System.Drawing.Point(0, -1),
                new System.Drawing.Point(1, -1),
                new System.Drawing.Point(-1, 0),
                new System.Drawing.Point(1, 0),
                new System.Drawing.Point(-1, 1),
                new System.Drawing.Point(0, 1),
                new System.Drawing.Point(1, 1)
            };
        /*/
        //四近傍
        private static System.Drawing.Point[] direction =
            new System.Drawing.Point[] {
                new System.Drawing.Point(0, -1),
                new System.Drawing.Point(-1, 0),
                new System.Drawing.Point(1, 0),
                new System.Drawing.Point(0, 1)
            };
        //*/

        /// <summary>
        /// 探査の際の近傍を取得
        /// </summary>
        public static System.Drawing.Point[] Direction { get { return direction; } }

    }

}
