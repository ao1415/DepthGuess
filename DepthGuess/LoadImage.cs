using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthGuess
{
    class LoadImage
    {
        private LogWriter logWriter;

        public LoadImage(LogWriter writer)
        {
            logWriter = writer;
        }

        public Bitmap load(string path)
        {
            Bitmap bmp;
            try
            {
                bmp = (Bitmap)Image.FromFile(path);
                logWriter.write("ファイルを読み込みました");
                return bmp;
            }
            catch (Exception)
            {
                logWriter.writeError("ファイル読み込みに失敗しました");
                return null;
            }
        }

    }
}
