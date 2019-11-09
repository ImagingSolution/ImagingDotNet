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
        // thresholdTypeはOpenCVと同じ
        // http://opencv.jp/opencv-2svn/cpp/miscellaneous_image_transformations.html#cv-threshold

        /// <summary>
        /// 二値化
        /// </summary>
        /// <param name="src">入力画像</param>
        /// <param name="dst">出力画像</param>
        /// <param name="thresh">しきい値</param>
        /// <param name="maxVal">最大値の値</param>
        /// <param name="thresholdType">閾値処理の種類（OpenCV相当）</param>
        public static void Threshold(this Bitmap src, Bitmap dst, byte thresh, byte maxVal, int thresholdType)
        {
            // しきい値に基づいてLutを作成する

            var lut = new byte[256];

            switch (thresholdType)
            {
                case THRESH_BINARY:
                    for (int i = thresh; i < 256; i++)
                    {
                        lut[i] = maxVal;
                    }
                    break;

                case THRESH_BINARY_INV:
                    for (int i = 0; i < thresh; i++)
                    {
                        lut[i] = maxVal;
                    }
                    break;

                case THRESH_TRUNC:
                    for (int i = 0; i < thresh; i++)
                    {
                        lut[i] = (byte)i;
                    }
                    for (int i = thresh; i < 256; i++)
                    {
                        lut[i] = thresh;
                    }
                    break;

                case THRESH_TOZERO:
                    for (int i = thresh; i < 256; i++)
                    {
                        lut[i] = (byte)i;
                    }
                    break;

                case THRESH_TOZERO_INV:
                    for (int i = 0; i < thresh; i++)
                    {
                        lut[i] = (byte)i;
                    }
                    break;

                case THRESH_OTSU:
                    // 大津の二値化（srcはモノクロ画像のこと）
                    // https://imagingsolution.net/imaging/discriminant-analysis-method/
                    // ヒストグラムの取得
                    var hist = Histogram(src);

                    // 黒クラスの平均値
                    double mB = 0;
                    // 黒クラスの画素数
                    double wB = 0;
                    // 黒クラスの輝度値の合計
                    double sumB = 0;

                    // 白クラスの平均値
                    double mW = 0;
                    // 白クラスの画素数
                    double wW = 0;
                    // 白クラスの輝度値の合計
                    double sumW = 0;

                    int thre = 0;
                    double val;
                    double max;

                    // ヒストグラム全体を合計
                    for(int i = 0; i < 256; i++)
                    {
                        // 画素数の合計
                        wW += hist[0][i];
                        // 輝度値の合計
                        sumW += hist[0][i] * i;
                    }

                    // 白クラスの平均値
                    if (wW > 0)
                    {
                        mW = sumW / wW;
                    }else
                    {
                        mW = 0;
                    }

                    // valの値が最大となるしきい値を見つける
                    val = wB * wW * (mB - mW) * (mB - mW);
                    max = val;

                    for (int i = 0; i < 255; i++)
                    {
                        // 黒クラスの画素数の合計
                        wB += hist[0][i];
                        // 黒クラスの輝度値の合計
                        sumB += hist[0][i] * i;
                        // 黒クラスの輝度値の平均

                        // 白クラスの平均値
                        if (wB > 0)
                        {
                            mB = sumB / wB;
                        }else
                        {
                            mB = 0;
                        }

                        // 白クラスの画素数の合計
                        wW -= hist[0][i];
                        // 白クラスの輝度値の合計
                        sumW -= hist[0][i] * i;
                        // 白クラスの平均値
                        if (wW > 0)
                        {
                            mW = sumW / wW;
                        }else
                        {
                            mW = 0;
                        }

                        val = wB * wW * (mB - mW) * (mB - mW);

                        if (max < val)
                        {
                            max = val;
                            thre = i + 1;
                        }
                    }
                    // 二値化
                    Threshold(src, dst, (byte)thre, maxVal, THRESH_BINARY);
                    return;
                    //break;

                default:
                    for (int i = thresh; i < 256; i++)
                    {
                        lut[i] = 255;
                    }
                    break;
            }
            // LUTに基づいたしきい値処理
            Threshold(src, dst, lut);
        }

        /// <summary>
        /// Lutに基づいた二値化処理
        /// </summary>
        /// <param name="src">入力画像</param>
        /// <param name="dst">出力画像</param>
        /// <param name="lut">Lut</param>
        public unsafe static void Threshold(this Bitmap src, Bitmap dst, byte[] lut)
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
                        for (int ch = 0; ch < channel; ch++)
                        {
                            // LUTを介した二値化処理
                            pLineDst[ch] = lut[pLineSrc[ch]];
                        }
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
