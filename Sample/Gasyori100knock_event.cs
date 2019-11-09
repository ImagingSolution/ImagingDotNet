using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

using ImagingSolution;

namespace Sample
{
    /// <summary>
    /// 画像処理１００本ノックのC#版
    /// https://github.com/yoyoyo-yo/Gasyori100knock
    /// </summary>
    public partial class Gasyori100knock : Form
    {
        private void GasyoriExecute(object sender)
        {
            var button = sender as Button;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Bitmap dst = null;

            switch (button.Text)
            {
                case "Q.1. チャネル入れ替え":
                    if (_bitmap.GetBitCount() < 24) break;  // カラーでない時は何もしない
                    dst = ImagingDotNet.CvtColor(_bitmap, ImagingDotNet.COLOR_BGR2RGB);
                    _bitmap.Dispose();  // 処理前の画像を破棄
                    _bitmap = dst;
                    break;

                case "Q.2. グレースケール化":
                    if (_bitmap.GetBitCount() == 8) break;  // モノクロの時は何もしない
                    dst = ImagingDotNet.CvtColor(_bitmap, ImagingDotNet.COLOR_BGR2GRAY);
                    _bitmap.Dispose();  // 処理前の画像を破棄
                    _bitmap = dst;
                    break;

                case "Q.3. 二値化":
                    dst = (Bitmap)_bitmap.Clone();  // 処理後画像の確保
                    ImagingDotNet.Threshold(
                        _bitmap,                    // 処理前画像
                        dst,                        // 処理後画像
                        128,                        // しきい値
                        255,                        // しきい値後の値
                        ImagingDotNet.THRESH_BINARY // 処理の種類
                        );
                    _bitmap.Dispose();  // 処理前の画像を破棄
                    _bitmap = dst;
                    break;

                case "Q.4. 大津の二値化": // モノクロのみ対応
                    Bitmap gray;

                    if (_bitmap.GetBitCount() >= 24)
                    {
                        // カラーの時はモノクロへ変換する
                        gray = ImagingDotNet.CvtColor(_bitmap, ImagingDotNet.COLOR_BGR2GRAY);
                        _bitmap.Dispose();  //　元の画像は解放
                    }
                    else
                    {
                        gray = _bitmap;
                    }

                    var dstThre = (Bitmap)gray.Clone();  // 処理後画像の確保
                    ImagingDotNet.Threshold(
                        gray,                       // 処理前画像（グレー画像）
                        dstThre,                    // 処理後画像（グレー画像）
                        128,                        // しきい値
                        255,                        // しきい値後の値
                        ImagingDotNet.THRESH_OTSU   // 処理の種類
                        );
                    gray.Dispose();
                    _bitmap = dstThre;

                    break;

                default:
                    break;
            }

            sw.Stop();

            // 処理時間の表示
            lblInfo.Text = $"{sw.ElapsedMilliseconds} msec";

            // 画像の再描画
            RedrawImage();
        }
    }
}
