using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * MainFormの動作を作成する場所です
 * ここで、ボタンやテキストボックスの動作を指定しています
 * デバッグモードで実行する場合はなるべく、デバッグありで実行してください
 * (上に表示されている開始ボタン or デバッグ->デバッグの開始 or F5)
 */


#region コメントアウトのテクニック
//*
namespace DepthGuess { }
/*/
namespace DepthGuess { }
//*/

/*
 * このようなコメントは、上の「//*」を「/*」に変更することでコメント切り替えることができます
 * このソースコード上にいくつか存在するので、切り替えたい場合は上記の方法で切り替えてください
 */
#endregion

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
        /// クラスの初期化を行います
        /// </summary>
        public MainForm()
        {
            //Visual Studioが自動生成したコードの初期化
            //編集しないこと
            InitializeComponent();

            Config.StyleSetup(this);

            logWriter = new LogWriter(this, logTextBox);

        }

        /// <summary>フ
        /// ァイルの選択のボタンを押されたときに、実行される関数です
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

        /// <summary>画像を保存します</summary>
        /// <param name="image">画像</param>
        /// <param name="depth">深さ</param>
        private void SaveImage(Bitmap image, LabelStructure depth)
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
                    saveImage.Save(image, depth, path + ".txt");
                    saveImage.SaveBinary(image, depth, path + ".rgbad");
                    depth.SetMinMax();
                    saveImage.SaveChip(image, depth, path);
                }
            }
        }

        /// <summary>
        /// タスク中止用の変数
        /// </summary>
        private CancellationTokenSource tokenSource = null;
        /// <summary>
        /// StartButton_Clickから呼び出される関数です
        /// </summary>
        private async Task ProcessAsync()
        {
            logWriter.Write("処理を開始します");
            CancellationToken token = tokenSource.Token;

            Bitmap originalImage = await Task.Run(() => { return new LoadImage(logWriter).Load(fileTextBox.Text); });
            token.ThrowIfCancellationRequested();

            if (originalImage == null) return;

            //new ImageWindow("元画像", originalImage, logWriter);
            
            Bitmap kmeansImage = await new K_means(8, logWriter).GetImageAsync(originalImage, tokenSource);
            token.ThrowIfCancellationRequested();

            //new ImageWindow("減色画像(k-means)", kmeansImage, logWriter);

            Bitmap medianImage = await new MedianFilter(logWriter).GetImageAsync(kmeansImage);
            token.ThrowIfCancellationRequested();

            //new ImageWindow("メディアンフィルタ", medianImage, logWriter);
            token.ThrowIfCancellationRequested();
            
            LabelStructure label = await new Labeling(logWriter).GetLabelTableAsync(medianImage, tokenSource);
            token.ThrowIfCancellationRequested();

            Bitmap labelImage = await new Labeling(logWriter).GetLabelImageAsync(label);

            //new ImageWindow("ラベリング画像", labelImage, logWriter);
            
            LabelStructure depth = await new Guess01(logWriter).GetDepthAsync(label, tokenSource);
            token.ThrowIfCancellationRequested();

            Bitmap depthImage = await new Labeling(logWriter).GetLabelImageAsync(depth);
            new ImageWindow("深さ情報", depthImage, logWriter);

            SaveImage(originalImage, depth);
            logWriter.Write("処理が完了しました");
        }

        /// <summary>
        /// 開始のボタンを押されたときに、実行される関数です
        /// </summary>
        private void StartButton_Click(object sender, EventArgs e)
        {
            //停止ボタンを有効にします
            stopButton.Enabled = true;
            //開始ボタンを無効にします
            startButton.Enabled = false;

            logWriter.Clear();

            //ここでProcess関数を呼び出します
            //並列処理がわからない場合は変更しないでください
            #region 並列処理
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();

                ProcessAsync().ContinueWith(t =>
                {
                    tokenSource.Dispose();
                    tokenSource = null;
                    if (t.IsCanceled)
                        logWriter.WriteError("処理を中止しました");

                    if (t.Exception != null)
                        logWriter.WriteError(t.Exception);

                    BeginInvoke(new Action(() =>
                    {
                        stopButton.Enabled = false;
                        startButton.Enabled = true;
                    }));
                });
            }
            #endregion
        }
        /// <summary>
        /// 停止のボタンを押されたときに、実行される関数です
        /// </summary>
        private void StopButton_Click(object sender, EventArgs e)
        {
            //プロセスが実行中ならば終了させる
            if (tokenSource != null)
                tokenSource.Cancel();
        }

        /// <summary>
        /// フォームが読み込まれるときに、実行される関数です
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //以前のフォームの状態に戻します
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;

            fileTextBox.Text = Properties.Settings.Default.FilePath;

            logTextBox.Focus();
        }

        /// <summary>
        /// <para>2重確認防止のための変数</para>
        /// <para>現状別スレッドのフォームを正常に終了できない</para>
        /// <para>将来的には正常に終了できるようにしたい</para>
        /// </summary>
        bool closeFlag = false;
        /// <summary>
        /// フォームが終了する前に、実行される関数です
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //プロセスの状態を確認します
            if (tokenSource != null)
            {
                //警告を表示
                MessageBox.Show("処理が終了していません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Question);

                //フォームが終了するのをキャンセルします
                e.Cancel = true;
                return;
            }
            else if (!closeFlag)
            {
                //確認を取ります
                var com = MessageBox.Show("終了しますか?", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (com == DialogResult.Cancel)
                {
                    //フォームが終了するのをキャンセルします
                    e.Cancel = true;
                    return;
                }
                else
                {
                    closeFlag = true;
                }
            }

            SaveSettings();

            //アプリケーションを終了させます
            Application.Exit();
        }

        /// <summary>
        /// フォームの状態を保存します
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
        private static Point[] direction =
            new Point[] {
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(-1, 1),
                new Point(0, 1),
                new Point(1, 1)
            };
        /*/
        //四近傍
        private static Point[] direction =
            new Point[] {
                new Point(0, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, 1)
            };
        //*/

        /// <summary>
        /// 探査の際の近傍を取得
        /// </summary>
        public static Point[] Direction { get { return direction; } }

        /// <summary>
        /// フォームのデザインを黒基調に変更します
        /// </summary>
        public static void StyleSetup(Form form)
        {
#if BLACK_STYLE
            form.BackColor = Color.FromArgb(30, 30, 30);
            form.ForeColor = SystemColors.Window;

            //MainFormのすべてのコントロールを列挙する
            foreach (var control in form.Controls)
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
                else if (type == typeof(ProgressBar))
                {
                    ProgressBar pbar = (ProgressBar)control;
                    pbar.BackColor = Color.FromArgb(30, 30, 30);
                    pbar.ForeColor = Color.FromArgb(220, 220, 220);
                }
            }
#endif
        }
    }

}
