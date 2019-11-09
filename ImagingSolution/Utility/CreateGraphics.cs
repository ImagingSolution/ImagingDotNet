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
        /// <param name="graphics">前回のGraphicsオブジェクト</param>
        /// <param name="mode">補間モード</param>
        /// <returns>作成したGraphicsオブジェクト</returns>
        public static void CreateGraphics(this PictureBox pic, ref Graphics graphics, System.Drawing.Drawing2D.InterpolationMode mode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor)
        {
            if ((pic.Width == 0) || (pic.Height == 0)) return;

            if (graphics != null) graphics.Dispose();

            var bmp = pic.Image as Bitmap;

            if (bmp != null) bmp.Dispose();

            bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // BitmapクラスオブジェクトをPictureBocのImageへ
            pic.Image = bmp;

            graphics = Graphics.FromImage(bmp);
            graphics.InterpolationMode = mode;
        }
    }
}
