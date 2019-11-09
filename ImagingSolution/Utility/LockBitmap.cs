using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImagingSolution
{
    /// <summary>
    /// ImagingDotNetクラス
    /// </summary>
    public static partial class ImagingDotNet
    {
        /// <summary>
        /// Bitmapのロック～アンロックを行うクラス
        /// </summary>
        public class LockBitmap : IDisposable
        {
            public LockBitmap(Bitmap bmp) : this(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height))
            {
            }

            public LockBitmap(Bitmap bmp, Rectangle rect)
            {
                // プロパティの代入
                _bitmap = bmp;
                this.Width = bmp.Width;
                this.Height = bmp.Height;
                this.PixelFormat = bmp.PixelFormat;
                this.BitCount = Bitmap.GetPixelFormatSize(this.PixelFormat);
                this.Channel = this.BitCount / 8;

                // Bitmapをロック
                _bitmapData = bmp.LockBits(
                        new Rectangle(0, 0, this.Width, this.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        this.PixelFormat
                    );

                this.Stride = Math.Abs(_bitmapData.Stride);

                this.Scan0 = _bitmapData.Scan0;
            }

            public void Dispose()
            {
                if ((_bitmap == null) || (_bitmapData == null)) return;

                // アンロック
                _bitmap.UnlockBits(_bitmapData);
                _bitmapData = null;
                _bitmap = null;
            }

            ~LockBitmap()
            {
                Dispose();
            }

            private Bitmap _bitmap;
            public Bitmap Bitmap {
                get
                {
                    return _bitmap;
                }
            }

            private BitmapData _bitmapData;
            public BitmapData BitmapData {
                get
                {
                    return _bitmapData;
                }
            }

            public int Width { get; }
            public int Height { get; }
            public int BitCount { get; }
            public PixelFormat PixelFormat { get; }
            public int Channel { get; }
            public int Stride { get; }
            public IntPtr Scan0 { get; }

            /// <summary>
            /// 輝度値を取得設定するインデクサ
            /// </summary>
            /// <param name="y">画像のY座標</param>
            /// <param name="x">画像のX座標</param>
            /// <returns></returns>
            public byte this[int y, int x]
            {
                set
                {
                    unsafe
                    {
                        var ptr = (byte*)this.Scan0;
                        ptr[x + y * this.Stride] = value;
                    }
                }
                get
                {
                    unsafe
                    {
                        var ptr = (byte*)this.Scan0;
                        return ptr[x + y * this.Stride];
                    }
                }
            }

            /// <summary>
            /// 輝度値を取得設定するインデクサ
            /// </summary>
            /// <param name="y">画像のY座標</param>
            /// <param name="x">画像のX座標</param>
            /// <param name="ch">画像のチャンネル番号 0:B, 1:G, 2:R</param>
            /// <returns></returns>
            public byte this[int y, int x, int ch]
            {
                set
                {
                    unsafe
                    {
                        var ptr = (byte*)this.Scan0;
                        ptr[x * this.Channel + ch + y * this.Stride] = value;
                    }
                }
                get
                {
                    unsafe
                    {
                        var ptr = (byte*)this.Scan0;
                        return ptr[x * this.Channel + ch + y * this.Stride];
                    }
                }
            }

        }

    }
}
