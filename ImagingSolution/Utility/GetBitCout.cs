using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace ImagingSolution
{
    /// <summary>
    /// ImagingDotNetクラス
    /// </summary>
    public static partial class ImagingDotNet
    {
        /// <summary>
        /// Bitmapのビット数を取得
        /// </summary>
        /// <returns></returns>
        public static int GetBitCount(this Bitmap src)
        {
            return Bitmap.GetPixelFormatSize(src.PixelFormat);
        }
    }
}
