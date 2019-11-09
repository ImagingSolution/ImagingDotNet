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
        /// PictureBoxからGraphicsオブジェクトの作成
        /// </summary>
        /// <param name="pic">作成する対象のPictureBox</param>
        /// <param name="oldGraphics">前回のGraphicsオブジェクト</param>
        /// <returns></returns>
        public static Graphics CreateGraphics(this PictureBox pic, Graphics oldGraphics)
        {
            if ((pic.Width == 0) || (pic.Height == 0)) return oldGraphics;

            if (oldGraphics != null) oldGraphics.Dispose();

            var bmp = pic.Image as Bitmap;

            if (bmp != null)
            {
                bmp.Dispose();
            }

            bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // BitmapクラスオブジェクトをPictureBocのImageへ
            pic.Image = bmp;

            return Graphics.FromImage(bmp);
        }
    }
}
