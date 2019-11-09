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
        /// ヒストグラムの取得
        /// </summary>
        /// <param name="src">入力画像</param>
        /// <param name="dst">出力画像</param>
        /// <param name="histogram">ヒストグラム</param>
        public static int[][] Histogram(this Bitmap src)
        {
            var channel = src.GetBitCount() / 8;
            var hist = new int[channel][];

            for (int i = 0; i < channel; i++)
            {
                hist[i] = new int[256];
            }

            Histogram(src, hist);

            return hist;
        }

        /// <summary>
        /// ヒストグラムの取得
        /// </summary>
        /// <param name="src">入力画像</param>
        /// <param name="dst">出力画像</param>
        /// <param name="histogram">ヒストグラム</param>
        public unsafe static void Histogram(this Bitmap src, int[][] histogram)
        {
            using (var lbSrc = new LockBitmap(src))
            {
                var pSrc = (byte*)lbSrc.Scan0;

                var width = lbSrc.Width;
                var height = lbSrc.Height;
                var channel = lbSrc.Channel;

                // ヒストグラムの値の初期化
                for (int ch = 0; ch < channel; ch++)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        histogram[ch][i] = 0;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    // 行の先頭ポインタ
                    byte* pLineSrc = pSrc + y * lbSrc.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        for (int ch = 0; ch < channel; ch++)
                        {
                            // 輝度値のカウント
                            histogram[ch][pLineSrc[ch]]++;
                        }

                        // 次の画素へ
                        pLineSrc += channel;
                    }
                }
            }
        }
    }
}
