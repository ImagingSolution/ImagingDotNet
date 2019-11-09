using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace ImagingSolution
{
    public static partial class ImagingDotNet
    {
        /// <summary>
        /// BitmapのフォーマとがFormat8bppIndexedのとき、グレースケールのパレットを設定します。
        /// </summary>
        /// <param name="bitmap">パレットを設定するBitmapオブジェクト</param>
        public static void SetGrayScalePalette(Bitmap bitmap)
        {
            // 8bitインデックスドカラーの時はグレースケールを設定する
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                var pal = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    pal.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = pal;
            }
        }
    }
}
