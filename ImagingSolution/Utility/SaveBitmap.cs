using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace ImagingSolution
{
    /// <summary>
    /// ImagingDotNetクラス
    /// </summary>
    public static partial class ImagingDotNet
    {
        // 名前を付けて保存
        // https://imagingsolution.net/program/csharp/save_bitmap/

        /// <summary>
        /// 名前を付けて保存ダイアログボックスで画像ファイルを保存します。
        /// </summary>
        /// <param name="bmp">保存するBitmapオブジェクト</param>
        public static void SaveBitmap(this Bitmap bmp)
        {
            //名前を付けて保存ダイアログボックスの作成  
            var sfd = new SaveFileDialog();
            //ファイルフィルタ  
            sfd.Filter = "Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png|CSV(*.csv)|*.csv";
            //ダイアログの表示 （Cancelボタンがクリックされた場合は何もしない）
            if (sfd.ShowDialog() == DialogResult.Cancel) return;

            bmp.SaveBitmap(sfd.FileName);
        }

        /// <summary>
        /// 名前を付けてBitmapクラスオブジェクトをファイルに保存します。
        /// </summary>
        /// <param name="bmp">保存するBitmapオブジェクト</param>
        /// <param name="filename">画像ファイルのファイル名</param>
        public static void SaveBitmap(this Bitmap bmp, string filename)
        {
            // ファイルの拡張子を取得し、小文字にする
            var ext = System.IO.Path.GetExtension(filename).ToLower();

            switch (ext)
            {
                case ".bmp":
                    bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;

                case ".jpg":
                    bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;

                case ".png":
                    bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    break;

                case ".csv":
                    SaveCsvBitmap(bmp, filename);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Bitmapの輝度値をCSVファイルに保存する
        /// 輝度値の並びは画像の右上から B,G,R,B,G,R…の順番
        /// </summary>
        /// <param name="bmp">保存するBitmapオブジェクト</param>
        /// <param name="filename">CSVファイルのファイル名</param>
        public static void SaveCsvBitmap(this Bitmap bmp, string filename)
        {
            using (var lb = new LockBitmap(bmp))
            using (var sw = new System.IO.StreamWriter(filename, false, System.Text.Encoding.GetEncoding("shift_jis")))
            {
                for (int y = 0; y < lb.Height; y++)
                {
                    string lineText = "";

                    for (int x = 0; x < lb.Width; x++)
                    {
                        for (int ch = 0; ch < lb.Channel; ch++)
                        {
                            lineText += $"{lb[y, x, ch]}, ";
                        }
                    }
                    // 最後の２文字(, )を削除
                    lineText = lineText.Remove(lineText.Length - 2);

                    sw.WriteLine(lineText);
                }
            }
        }
    }
}
