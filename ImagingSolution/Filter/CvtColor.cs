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
        /// カラー変換
        /// </summary>
        /// <param name="src">変換前の画像データ</param>
        /// <param name="code">カラー変換コード　ImagingDotNet.COLOR_BGR2GRAY など、名前部分はOpenCVと同じ</param>
        /// <returns>変換後の画像データ</returns>
        public static Bitmap CvtColor(this Bitmap src, int code)
        {
            if (src == null) return null;

            Bitmap dst = null;

            switch (code)
            {
                case COLOR_BGR2GRAY:
                    dst = CreateBitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                    break;

                default:
                    break;
            }

            CvtColor(src, dst, code);

            return dst;
        }

        /// <summary>
        /// カラー変換
        /// </summary>
        /// <param name="src">変換前の画像データ</param>
        /// <param name="dst">変換後の画像データ</param>
        /// <param name="code">カラー変換コード　ImagingDotNet.COLOR_BGR2GRAY など、名前部分はOpenCVと同じ</param>
        public unsafe static void CvtColor(Bitmap src, Bitmap dst, int code)
        {
            if ((src == null) || (dst == null)) return;

            switch (code)
            {
                case COLOR_BGR2GRAY:

                    using (var lbSrc = new LockBitmap(src))
                    using (var lbDst = new LockBitmap(dst))
                    {
                        var pSrc = (byte*)lbSrc.Scan0;
                        var pDst = (byte*)lbDst.Scan0;

                        byte r, g, b;
                        byte* pLineSrc = pSrc;
                        byte* pLineDst = pDst;

                        var width = lbSrc.Width;
                        var height = lbSrc.Height;
                        var channel = lbSrc.Channel;

                        for (int y = 0; y < height; y++)
                        {
                            // 行の先頭ポインタ
                            pSrc = pLineSrc;
                            pDst = pLineDst;

                            for (int x = 0; x < width; x++)
                            {
                                // 輝度値（カラー）の取得
                                r = pSrc[2];
                                g = pSrc[1];
                                b = pSrc[0];

                                // 輝度値(モノクロ)の設定
                                // gray = 0.299 * R + 0.587 * G + 0.114 * B 
                                pDst[0] = (byte)((77 * r + 150 * g + 29 * b) >> 8);

                                // 次の画素へ
                                pSrc += channel;
                                pDst++;
                            }
                            // 次のラインへ
                            pLineSrc += lbSrc.Stride;
                            pLineDst += lbDst.Stride;
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
