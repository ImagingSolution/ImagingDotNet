using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

// カラー変換

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

                case COLOR_BGR2RGB: // COLOR_RGB2BGR も同じ
                    dst = CreateBitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
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
        public static void CvtColor(Bitmap src, Bitmap dst, int code)
        {
            if ((src == null) || (dst == null)) return;

            switch (code)
            {
                case COLOR_BGR2GRAY:
                    cvt_COLOR_BGR2GRAY(src, dst);
                    break;

                case COLOR_BGR2RGB:
                    cvt_COLOR_BGR2RGB(src, dst);
                    break;


                default:
                    break;
            }
        }

        /// <summary>
        /// カラー→モノクロ変換
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        private unsafe static void cvt_COLOR_BGR2GRAY(Bitmap src, Bitmap dst)
        {
            using (var lbSrc = new LockBitmap(src))
            using (var lbDst = new LockBitmap(dst))
            {
                var pSrc = (byte*)lbSrc.Scan0;
                var pDst = (byte*)lbDst.Scan0;

                var width = lbSrc.Width;
                var height = lbSrc.Height;
                var channel = lbSrc.Channel;

                Parallel.For(0, height, y =>
                {
                    // 行の先頭ポインタ
                    byte* pLineSrc = pSrc + y * lbSrc.Stride;
                    byte* pLineDst = pDst + y * lbDst.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        // 輝度値(モノクロ)の設定
                        // gray = ((0.299 * R + 0.587 * G + 0.114 * B) * 256) / 256
                        pLineDst[0] = (byte)((77 * pLineSrc[2] + 150 * pLineSrc[1] + 29 * pLineSrc[0]) >> 8);

                        // 次の画素へ
                        pLineSrc += channel;
                        pLineDst++;
                    }
                }
                );
            }
        }

        /// <summary>
        /// チャンネル入れ替え
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        private unsafe static void cvt_COLOR_BGR2RGB(Bitmap src, Bitmap dst)
        {
            using (var lbSrc = new LockBitmap(src))
            using (var lbDst = new LockBitmap(dst))
            {
                var pSrc = (byte*)lbSrc.Scan0;
                var pDst = (byte*)lbDst.Scan0;

                var width = lbSrc.Width;
                var height = lbSrc.Height;
                var channel = lbSrc.Channel;

                Parallel.For(0, height, y =>
                {
                    // 行の先頭ポインタ
                    byte* pLineSrc = pSrc + y * lbSrc.Stride;
                    byte* pLineDst = pDst + y * lbDst.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        // チャンネル入れ替え
                        pLineDst[0] = pLineSrc[2];
                        pLineDst[1] = pLineSrc[1];
                        pLineDst[2] = pLineSrc[0];

                        // 次の画素へ
                        pLineSrc += channel;
                        pLineDst += channel;
                    }
                }
                );
            }
        }


    }
}
