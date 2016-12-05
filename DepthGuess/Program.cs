using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * ここはMainFormを起動している場所です。
 * ここにコードを記述したり、削除したりしないでください。
 * コードを書き込みたい場合は
 * Form->MainForm.cs->MainForm.Desgner.cs->MainForm
 * に書き込んでください。
 */

namespace DepthGuess
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
