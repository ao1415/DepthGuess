using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthGuess
{
    public partial class PrograssForm : Form
    {
        public PrograssForm(string text, int max)
        {
            InitializeComponent();

#if BLACK_STYLE
            StyleSetup();
#endif

            Text = text;

            progressBar.Minimum = 0;
            progressBar.Maximum = max;

            label.Text = progressBar.Value.ToString() + "/" + progressBar.Maximum.ToString();
        }

        public void Add()
        {
            progressBar.Value++;

            label.Text = progressBar.Value.ToString() + "/" + progressBar.Maximum.ToString();
            label.Update();

            if (progressBar.Value >= progressBar.Maximum)
                Close();
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
                if (type == typeof(Label))
                {
                    Label l = (Label)control;
                    l.BackColor = Color.FromArgb(30, 30, 30);
                    l.ForeColor = Color.FromArgb(220, 220, 220);
                    l.Font = new Font("ＭＳ ゴシック", 9);
                }
                else if (type == typeof(ProgressBar))
                {
                    ProgressBar pbar = (ProgressBar)control;
                    pbar.BackColor = Color.FromArgb(30, 30, 30);
                    pbar.ForeColor = Color.FromArgb(220, 220, 220);
                }

            }

        }

    }
}
