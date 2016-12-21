using System.Drawing;
using System.Windows.Forms;

/*
 * PrograssFormの動作を作成する場所です
 * ここで、プログレスバーの動作を指定しています
 */

namespace DepthGuess
{
    /// <summary>
    /// プログレスバーの表示を行うクラス
    /// </summary>
    public partial class PrograssForm : Form
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="text">ウインドウのタイトル</param>
        /// <param name="max">カウンタの最大値</param>
        public PrograssForm(string text, int max)
        {
            //Visual Studioが自動生成したコードの初期化
            //編集しないこと
            InitializeComponent();

            Config.StyleSetup(this);

            Text = text;

            progressBar.Minimum = 0;
            progressBar.Maximum = max;

            label.Text = progressBar.Value.ToString() + "/" + progressBar.Maximum.ToString();
        }

        /// <summary>
        /// カウンタを+1する
        /// </summary>
        public void Add()
        {
            //カウントアップ
            progressBar.Value++;

            //ラベルテキスト更新
            label.Text = progressBar.Value.ToString() + "/" + progressBar.Maximum.ToString();
            //ラベルの表示の更新
            label.Update();

        }
        
    }
}
