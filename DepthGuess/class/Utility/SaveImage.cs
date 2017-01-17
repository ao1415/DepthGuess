using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
 * 画像を保存するクラスが定義されています。
 * Saveに画像と、パスを渡すことで画像が保存されます。
 */

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
            catch (Exception ex)
            {
                logWriter.WriteError("三次元画像の保存に失敗しました");
                logWriter.WriteError(ex.ToString());
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
            /*
             * バイナリデータの保存形式
             * 0_3byte  画像の横幅(int)
             * 4_7byte  画像の縦幅(int)
             * 8_byte   色・深さの情報
             * 8n   byte  赤色情報(byte)
             * 8n+1 byte  緑色情報(byte)
             * 8n+2 byte  青色情報(byte)
             * 8n+3 byte  アルファ情報(byte)
             * 8n+4_8n+7byte  深さ情報(int)
             * 以下画像の大きさだけループ
             */

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
            catch (Exception ex)
            {
                logWriter.WriteError("三次元画像の保存に失敗しました");
                logWriter.WriteError(ex.ToString());
                return;
            }

            logWriter.Write("三次元画像を保存しました");
            logWriter.Write("path=" + path);
        }

        public void SaveChip(Bitmap image, LabelStructure depth, string path)
        {
            logWriter.Write("画像を保存します");

            try
            {
                for (int i = depth.Min; i <= depth.Max; i++)
                {
                    Bitmap bitmap = new Bitmap(image);
                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    byte[] buf = new byte[bitmap.Width * bitmap.Height * 4];
                    Marshal.Copy(data.Scan0, buf, 0, buf.Length);

                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            if (depth[y, x] != i)
                            {
                                int index = y * bitmap.Width * 4 + x * 4;
                                buf[index + 0] = 0;
                                buf[index + 1] = 0;
                                buf[index + 2] = 0;
                                buf[index + 3] = 0;
                            }
                        }
                    }

                    Marshal.Copy(buf, 0, data.Scan0, buf.Length);
                    bitmap.UnlockBits(data);

                    string name = path + "_" + i.ToString() + ".png";
                    bitmap.Save(name);
                    logWriter.Write("path=" + name);
                }
            }
            catch (Exception ex)
            {
                logWriter.WriteError("三次元画像の保存に失敗しました");
                logWriter.WriteError(ex.ToString());
                return;
            }

            logWriter.Write("画像を保存しました");
        }

    }
}
