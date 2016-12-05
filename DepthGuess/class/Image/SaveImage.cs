using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthGuess
{
    /// <summary>画像をファイルに保存する</summary>
    class SaveImage
    {
        LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public SaveImage(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>画像を保存する</summary>
        /// <param name="image">保存したい画像</param>
        /// <param name="path">保存する場所</param>
        public void Save(Image image, string path)
        {
            logWriter.Write("画像を保存します");

            try
            {
                image.Save(path);
            }
            catch (Exception)
            {
                logWriter.WriteError("画像の保存に失敗しました");
                return;
            }

            logWriter.Write("画像を保存しました");
            logWriter.Write("path=" + path);
        }

        /// <summary>深さ情報を持った画像を保存する</summary>
        /// <param name="image">保存したい画像</param>
        /// <param name="depth">画像の深さデータ</param>
        /// <param name="path">保存したい場所</param>
        public void Save(Bitmap image, LabelStructure depth, string path)
        {
            logWriter.Write("三次元画像を保存します");

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(image.Width + " " + image.Height);
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color c = image.GetPixel(x, y);
                            sw.WriteLine(c.R + " " + c.G + " " + c.B + " " + depth[y, x]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                logWriter.WriteError("三次元画像の保存に失敗しました");
                return;
            }

            logWriter.Write("三次元画像を保存しました");
            logWriter.Write("path=" + path);
        }
        /// <summary>深さ情報を持った画像のバイナリデータを保存する</summary>
        /// <param name="image">保存したい画像</param>
        /// <param name="depth">画像の深さデータ</param>
        /// <param name="path">保存したい場所</param>
        public void SaveBinary(Bitmap image, LabelStructure depth, string path)
        {
            logWriter.Write("三次元画像を保存します");

            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path)))
                {
                    bw.Write(image.Width);
                    bw.Write(image.Height);
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color c = image.GetPixel(x, y);
                            bw.Write(c.R);
                            bw.Write(c.G);
                            bw.Write(c.B);
                            bw.Write(c.A);
                            bw.Write(depth[y, x]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                logWriter.WriteError("三次元画像の保存に失敗しました");
                return;
            }

            logWriter.Write("三次元画像を保存しました");
            logWriter.Write("path=" + path);
        }

    }
}
