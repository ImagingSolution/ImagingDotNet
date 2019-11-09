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
        /// <summary>
        /// 画像ファイルを指定してBitmapクラスオブジェクトを作成します。（Bitmapクラスと同じ）
        /// </summary>
        /// <param name="filename">画像ファイルのファイル名</param>
        /// <returns>作成したBitmapクラスオブジェクト</returns>
        public static Bitmap CreateBitmap(string filename)
        {
            if (System.IO.File.Exists(filename) == false) return null;

            Bitmap bmp;

            // ファイルストリームでファイルを開く
            using (var fs = new System.IO.FileStream(
                filename,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read))
            {
                bmp = new Bitmap(fs);
            }
            return bmp;
        }

        /// <summary>
        /// ファイルストリームを指定してBitmapクラスオブジェクトを作成します。（Bitmapクラスと同じ）
        /// </summary>
        /// <param name="stream">画像のファイルストリームを指定します。</param>
        /// <returns>作成したBitmapクラスオブジェクト</returns>
        public static Bitmap CreateBitmap(System.IO.Stream stream)
        {
            return new Bitmap(stream);
        }

        /// <summary>
        /// 画像のサイズとPixelFormatを指定してBitmapクラスオブジェクトを作成します。
        /// formatにFormat8bppIndexedを指定したとき、グレースケールのパレットが設定されます。
        /// </summary>
        /// <param name="width">画像の幅</param>
        /// <param name="height">画像の高さ</param>
        /// <param name="format">画像のピクセルフォーマット</param>
        /// <returns>作成したBitmapクラスオブジェクト</returns>
        public static Bitmap CreateBitmap(int width, int height, System.Drawing.Imaging.PixelFormat format)
        {
            var bmp = new Bitmap(width, height, format);

            // 8bitインデックスドカラーの時はグレースケールを設定する
            SetGrayScalePalette(bmp);

            return bmp;
        }

        /// <summary>
        /// 画像メモリのポインタを指定してBitmapクラスオブジェクトを作成します。
        /// </summary>
        /// <param name="width">画像の幅</param>
        /// <param name="height">画像の高さ</param>
        /// <param name="format">画像のピクセルフォーマット</param>
        /// <param name="scan0">画像データのポインタ</param>
        /// <returns></returns>
        public static Bitmap CreateBitmap(int width, int height, System.Drawing.Imaging.PixelFormat format, IntPtr scan0)
        {
            int stride = (width * Bitmap.GetPixelFormatSize(format) + 31) / 32 * 4;

            var bmp = new Bitmap(width, height, stride, format, scan0);

            // 8bitインデックスドカラーの時はグレースケールを設定する
            SetGrayScalePalette(bmp);

            return bmp;
        }


    }
}
