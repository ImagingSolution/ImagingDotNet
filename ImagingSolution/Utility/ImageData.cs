using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace ImagingSolution
{
    public static partial class ImagingDotNet
    {
        /// <summary>
        /// 画像データ管理クラス
        /// </summary>
        public class ImageData : IDisposable
        {

            /// <summary>
            /// メモリを０で初期化する(Win32API)
            /// </summary>
            /// <param name="dest">メモリのポインタ</param>
            /// <param name="size">メモリのバイト数</param>
            [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
            public static extern void ZeroMemory(IntPtr dest, int size);

            /// <summary>
            /// メモリのコピー(Win32API)
            /// </summary>
            /// <param name="dst">コピー先のポインタ</param>
            /// <param name="src">コピー元のポインタ</param>
            /// <param name="size">コピーするメモリのバイト数</param>
            [DllImport("kernel32.dll")]
            public static extern void CopyMemory(IntPtr dst, IntPtr src, int size);

            int _minVale = 0;
            int _maxVale = 0;

            /// <summary>
            /// 画像の輪郭参照方法の種類
            /// </summary>
            public enum BorderTypeEnum
            {
                //index     -7  -6  -5  -4  -3  -2  -1  0  1  2  3  4  5  6  7  8  9  10  11
                //clamp      0   0   0   0   0   0   0  0  1  2  3  3  3  3  3  3  3   3   3
                //mirror     1   2   3   3   2   1   0  0  1  2  3  3  2  1  0  0  1   2   3

                /// <summary>
                /// 外側の場合、輝度値を０にする
                /// </summary>
                ToZero,
                /// <summary>
                /// 折り返し
                /// </summary>
                Mirror,
                /// <summary>
                /// 輪郭固定
                /// </summary>
                Clamp

            }

            /// <summary>
            /// 補間モードの種類
            /// </summary>
            public enum InterpolationModeEnum
            {
                NearestNeighbor,
                Bilinear,
                Bicubic
            }

            /// <summary>
            /// マイナスの値設定時の処理
            /// </summary>
            public enum MinusValueModeEnum
            {
                Zero,
                Absolute
            }
            //typedef struct tagBITMAPFILEHEADER {全14Byte
            //        WORD    bfType;		2Byte
            //        DWORD   bfSize;		4Byte
            //        WORD    bfReserved1;		2Byte
            //        WORD    bfReserved2;		2Byte
            //        DWORD   bfOffBits;		4Byte
            //} BITMAPFILEHEADER;
            /// <summary>
            /// Win32APIのBITMAPFILEHEADER定義
            /// </summary>
            public struct BITMAPFILEHEADER
            {
                /// <summary>
                /// ファイルタイプ
                /// </summary>
                public UInt16 bfType;
                /// <summary>
                /// ファイル全体のサイズ
                /// </summary>
                public UInt32 bfSize;
                /// <summary>
                /// 予約領域
                /// </summary>
                public UInt16 bfReserved1;
                /// <summary>
                /// 予約領域
                /// </summary>
                public UInt16 bfReserved2;
                /// <summary>
                /// ファイルの先頭から画像データまでのオフセット数（バイト数）
                /// </summary>
                public UInt32 bfOffBits;
            }

            //typedef struct tagBITMAPINFOHEADER{全40Byte
            //        DWORD      biSize;		4Byte
            //        LONG       biWidth;		4Byte
            //        LONG       biHeight;		4Byte
            //        WORD       biPlanes;		2Byte
            //        WORD       biBitCount;	2Byte
            //        DWORD      biCompression;	4Byte
            //        DWORD      biSizeImage;	4Byte
            //        LONG       biXPelsPerMeter;	4Byte
            //        LONG       biYPelsPerMeter;	4Byte
            //        DWORD      biClrUsed;		4Byte
            //        DWORD      biClrImportant;	4Byte
            //} BITMAPINFOHEADER;
            /// <summary>
            /// Win32APIのBITMAPINFOHEADER定義
            /// </summary>
            public struct BITMAPINFOHEADER
            {
                /// <summary>
                /// BITMAPINFOHEADERのサイズ (40)
                /// </summary>
                public UInt32 biSize;
                /// <summary>
                /// ビットマップの幅
                /// </summary>
                public Int32 biWidth;
                /// <summary>
                /// ビットマップの高さ
                /// </summary>
                public Int32 biHeight;
                /// <summary>
                /// プレーン数(常に1)
                /// </summary>
                public UInt16 biPlanes;
                /// <summary>
                /// 1ピクセルあたりのビット数(1,4,8,16,24,32)
                /// </summary>
                public UInt16 biBitCount;
                /// <summary>
                /// 圧縮形式
                /// </summary>
                public UInt32 biCompression;
                /// <summary>
                /// イメージのサイズ(バイト数)
                /// </summary>
                public UInt32 biSizeImage;
                /// <summary>
                /// ビットマップの水平解像度
                /// </summary>
                public Int32 biXPelsPerMeter;
                /// <summary>
                /// ビットマップの垂直解像度
                /// </summary>
                public Int32 biYPelsPerMeter;
                /// <summary>
                /// カラーパレット数
                /// </summary>
                public UInt32 biClrUsed;
                /// <summary>
                /// 重要なカラーパレットのインデックス
                /// </summary>
                public UInt32 biClrImportant;
            }

            /// <summary>
            /// コンストラクタの共通部分
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="imageBit"></param>
            /// <param name="bufferBit"></param>
            private void InitBuffer(int width, int height, int imageBit, int bufferBit)
            {
                _width = width;
                _height = height;

                // オリジナルサイズ
                _widthOriginal = _width;
                _heightOriginal = _height;

                _maxVale = 255;

                if ((imageBit == 8) && (bufferBit == 8))
                {
                    // 8bitモノクロ
                    _pixelFormat = PixelFormat.Format8bppIndexed;
                }
                else if ((imageBit == 24) && (bufferBit == 24))
                {
                    // 24bitカラー
                    _pixelFormat = PixelFormat.Format24bppRgb;
                }
                else if ((imageBit == 32) && (bufferBit == 32))
                {
                    // 32bitカラー
                    _pixelFormat = PixelFormat.Format32bppArgb;
                }
                else if (bufferBit == 16)
                {
                    // 多ビットモノクロ
                    _pixelFormat = PixelFormat.Format16bppGrayScale;
                    _maxVale = (int)Math.Pow(2, imageBit) - 1;
                }
                else if ((imageBit == 30) && (bufferBit == 32))
                {
                    _pixelFormat = PixelFormat.Undefined;
                    _maxVale = 1023;
                }

                // 画像データのビット数
                _imageBit = imageBit;
                // 画像メモリのビット数
                _bufferBit = bufferBit;

                _channel = _bufferBit / 8;

                _stride = CalcStride(width, _bufferBit);

                // アライメント調整されたメモリの確保
                _scan0 = AlignedMalloc(_stride * height, 32, true, out _scan0Origin);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="width">画像の幅</param>
            /// <param name="height">画像の高さ</param>
            /// <param name="format">画像のフォーマット</param>
            public ImageData(int width, int height, PixelFormat format)
            {
                int bit = Bitmap.GetPixelFormatSize(format);

                InitBuffer(width, height, bit, bit);
            }

            /// <summary>
            /// コンストラクタ
            /// 外部メモリ参照
            /// </summary>
            /// <param name="width">画像の幅</param>
            /// <param name="height">画像の高さ</param>
            /// <param name="stride">画像１行あたりのバイト数</param>
            /// <param name="format">画像のフォーマット</param>
            /// <param name="scan0">画像データのポインタ</param>
            public ImageData(int width, int height, int stride, PixelFormat format, IntPtr scan0)
            {
                _width = width;
                _height = height;
                _pixelFormat = format;

                // オリジナルサイズ
                _widthOriginal = _width;
                _heightOriginal = _height;

                // 画像データのビット数
                _imageBit = Bitmap.GetPixelFormatSize(format);
                // 画像メモリのビット数
                _bufferBit = _imageBit;

                _channel = _bufferBit / 8;

                _stride = stride;

                // アライメント調整されたメモリの確保
                _scan0 = scan0;

                _maxVale = 255;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="width">画像の幅</param>
            /// <param name="height">画像の高さ</param>
            /// <param name="imageBit">画像のビット数(8～16, 24, 30, 32)</param>
            /// <param name="bufferBit">画像メモリのビット数(8, 16, 24, 32)</param>
            public ImageData(int width, int height, int imageBit, int bufferBit)
            {
                InitBuffer(width, height, imageBit, bufferBit);
            }

            public ImageData(string filename)
            {
                string ext = System.IO.Path.GetExtension(filename).ToLower();

                if ((ext != ".bmp") &&
                    (ext != ".gif") &&
                    (ext != ".jpg") &&
                    (ext != ".jpeg") &&
                    (ext != ".png") &&
                    (ext != ".tif") &&
                    (ext != ".tiff"))
                {
                    throw new System.IO.FileNotFoundException("非対応のファイルフォーマットです。");
                }

                Bitmap b;

                _maxVale = 255;

                try
                {
                    if (ext == ".bmp")
                    {
                        LoadBitmapBinary(filename);
                    }
                    else
                    {
                        using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
                        {
                            b = new Bitmap(fs);
                        }
                        if (b == null)
                        {
                            throw new System.IO.FileNotFoundException("非対応のファイルフォーマットです。");
                        }

                        _width = b.Width;
                        _height = b.Height;
                        _pixelFormat = b.PixelFormat;

                        // オリジナルサイズ
                        _widthOriginal = _width;
                        _heightOriginal = _height;

                        // 画像データのビット数
                        _imageBit = Bitmap.GetPixelFormatSize(_pixelFormat);
                        // 画像メモリのビット数
                        _bufferBit = _imageBit;

                        _channel = _bufferBit / 8;

                        _stride = CalcStride(_width, _bufferBit);

                        // アライメント調整されたメモリの確保
                        _scan0 = AlignedMalloc(_stride * _height, 32, true, out _scan0Origin);

                        BitmapData data = b.LockBits(
                            new Rectangle(0, 0, _width, _height),
                            ImageLockMode.ReadWrite,
                            _pixelFormat);

                        CopyMemory(_scan0, data.Scan0, _stride * _height);
                        b.UnlockBits(data);

                        b.Dispose();

                    }
                }
                catch
                {
                    throw new ArgumentException("ファイルを開くことができません。");

                }
            }

            /////////////////////////////////////////////////////////////////////////////////////
            /// インデクサ
            /////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// 輝度値を取得、設定します。
            /// </summary>
            /// <param name="row">画像の行の位置(y座標)</param>
            /// <param name="column">画像の列の位置(x座標)</param>
            /// <returns></returns>
            public virtual int this[int row, int column]
            {
                set
                {
                    if (_scan0 == IntPtr.Zero) return;

                    if (_minusValueMode == MinusValueModeEnum.Absolute)
                    {
                        value = Math.Abs(value);
                    }

                    value = (value < _minVale) ? _minVale : ((value > _maxVale) ? _maxVale : value);

                    // 画像の外側の設定は行わない
                    if ((column < 0) || (column >= _width * _channel))
                    {
                        return;
                    }

                    if ((row < 0) || (row >= _height))
                    {
                        return;
                    }

                    unsafe
                    {
                        if (_maxVale == 255)
                        {

                            byte* pVal = (byte*)_scan0;
                            pVal[column + row * _stride] = (byte)value;
                        }
                        else
                        {
                            if (_bufferBit == 16)
                            {
                                ushort* pVal = (ushort*)_scan0;
                                pVal[column + row * _stride / 2] = (ushort)value;
                            }
                        }
                    }
                }

                get
                {
                    if (_scan0 == IntPtr.Zero) return -1;

                    int x = column;
                    int y = row;

                    int xtemp;

                    if (x < 0)
                    {
                        if (_BorderType == BorderTypeEnum.Mirror)
                        {
                            //x = (-(column - _channel + 1) / _channel) * _channel + (column % _channel);
                            xtemp = -(column - _channel + 1) / _channel;
                            x = xtemp * _channel + column + xtemp * _channel;
                        }
                        else if (_BorderType == BorderTypeEnum.Clamp)
                        {
                            x = 0;
                        }
                        else if (_BorderType == BorderTypeEnum.ToZero)
                        {
                            return 0;
                        }
                        return this[y, x];
                    }
                    else if (x >= _width * _channel)
                    {
                        if (_BorderType == BorderTypeEnum.Mirror)
                        {
                            xtemp = (column + _channel - 1) / _channel - _width + 1;
                            x = (_width - 1 - xtemp) * _channel + (column % _channel);
                        }
                        else if (_BorderType == BorderTypeEnum.Clamp)
                        {
                            x = _width * _channel - 1;
                        }
                        else if (_BorderType == BorderTypeEnum.ToZero)
                        {
                            return 0;
                        }
                        return this[y, x];
                    }

                    if (y < 0)
                    {
                        if (_BorderType == BorderTypeEnum.Mirror)
                        {
                            y = -row;

                        }
                        else if (_BorderType == BorderTypeEnum.Clamp)
                        {
                            y = 0;
                        }
                        else if (_BorderType == BorderTypeEnum.ToZero)
                        {
                            return 0;
                        }
                        return this[y, x];
                    }
                    else if (y >= _height)
                    {
                        if (_BorderType == BorderTypeEnum.Mirror)
                        {
                            y = _height * 2 - 2 - row;

                        }
                        else if (_BorderType == BorderTypeEnum.Clamp)
                        {
                            y = _height - 1;
                        }
                        else if (_BorderType == BorderTypeEnum.ToZero)
                        {
                            return 0;
                        }
                        return this[y, x];
                    }

                    int val = 0;
                    unsafe
                    {
                        if (_maxVale == 255)
                        {

                            byte* pVal = (byte*)_scan0;
                            val = pVal[column + row * _stride];
                        }
                        else
                        {
                            if (_bufferBit == 16)
                            {
                                ushort* pVal = (ushort*)_scan0;
                                val = pVal[column + row * _stride / 2];
                            }
                        }
                    }
                    return val;
                }
            }

            /// <summary>
            /// 輝度値を取得、設定します。
            /// </summary>
            /// <param name="row">画像の行の位置(y座標)</param>
            /// <param name="column">画像の列の位置(x座標)</param>
            /// <param name="ch">色番号 0, 1, 2</param>
            /// <returns></returns>
            public virtual int this[int row, int column, int ch]
            {
                set
                {
                    this[row, column * _channel + ch] = value;
                }

                get
                {
                    return this[row, column * _channel + ch];
                }
            }

            /////////////////////////////////////////////////////////////////////
            // biCubic用関数
            private double BiCubicWeight(double t)
            {
                double weight;

                double absT = Math.Abs(t);

                if (absT <= 1.0)
                {
                    weight = (_biCubicVal + 2.0) * absT * absT * absT - (_biCubicVal + 3.0) * absT * absT + 1.0;
                }
                else if ((1 < absT) && (absT <= 2.0))
                {
                    weight = _biCubicVal * absT * absT * absT - 5.0 * _biCubicVal * absT * absT + 8.0 * _biCubicVal * absT - 4.0 * _biCubicVal;
                }
                else
                {
                    weight = 0.0;
                }
                return weight;
            }


            /////////////////////////////////////////////////////////////////////


            /// <summary>
            /// 輝度値を取得、設定します。
            /// </summary>
            /// <param name="row">画像の行の位置(y座標)</param>
            /// <param name="column">画像の列の位置(x座標)</param>
            /// <returns></returns>
            public virtual int this[float row, float column]
            {
                get
                {
                    int value = 0;

                    //if (row == 0)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(column.ToString());

                    //}

                    if (_interpolation == InterpolationModeEnum.Bilinear)
                    {
                        int X1 = (int)Math.Floor(column);
                        double dX = (double)column - (double)X1;

                        int Y1 = (int)Math.Floor(row);
                        double dY = (double)row - (double)Y1;

                        value = (int)(
                            (1.0 - dX) * ((1.0 - dY) * this[Y1, X1] + dY * this[Y1 + 1, X1])
                          + dX * ((1.0 - dY) * this[Y1, X1 + _channel] + dY * this[Y1 + 1, X1 + _channel])
                          + 0.5);
                    }
                    else if (_interpolation == InterpolationModeEnum.NearestNeighbor)
                    {
                        int X1 = (int)Math.Floor(column + 0.5);
                        int Y1 = (int)Math.Floor(row + 0.5);

                        value = this[Y1, X1];
                    }
                    else if (_interpolation == InterpolationModeEnum.Bicubic)
                    {
                        int X = (int)Math.Floor(column);
                        int Y = (int)Math.Floor(row);

                        double dX = column;
                        double dY = row;
                        double flX = Math.Floor(dX);
                        double flY = Math.Floor(dY);

                        double hX1 = BiCubicWeight(1.0 + dX - flX);
                        double hX2 = BiCubicWeight(dX - flX);
                        double hX3 = BiCubicWeight(flX + 1.0 - dX);
                        double hX4 = BiCubicWeight(flX + 2.0 - dX);

                        double hY1 = BiCubicWeight(1.0 + dY - flY);
                        double hY2 = BiCubicWeight(dY - flY);
                        double hY3 = BiCubicWeight(flY + 1.0 - dY);
                        double hY4 = BiCubicWeight(flY + 2.0 - dY);

                        double tX1 = this[Y - 1, X - _channel] * hX1
                                    + this[Y - 1, X] * hX2
                                    + this[Y - 1, X + _channel] * hX3
                                    + this[Y - 1, X + _channel * 2] * hX4;

                        double tX2 = this[Y, X - _channel] * hX1
                                    + this[Y, X] * hX2
                                    + this[Y, X + _channel] * hX3
                                    + this[Y, X + _channel * 2] * hX4;

                        double tX3 = this[Y + 1, X - _channel] * hX1
                                    + this[Y + 1, X] * hX2
                                    + this[Y + 1, X + _channel] * hX3
                                    + this[Y + 1, X + _channel * 2] * hX4;

                        double tX4 = this[Y + 2, X - _channel] * hX1
                                   + this[Y + 2, X] * hX2
                                   + this[Y + 2, X + _channel] * hX3
                                   + this[Y + 2, X + _channel * 2] * hX4;

                        value = (int)(
                                    hY1 * tX1
                                  + hY2 * tX2
                                  + hY3 * tX3
                                  + hY4 * tX4
                                  + 0.5
                            );

                        value = (value < 0) ? 0 : ((value > 255) ? 255 : value);
                    }
                    return value;
                }
            }

            /// <summary>
            /// 輝度値を取得、設定します。
            /// </summary>
            /// <param name="row">画像の行の位置(y座標)</param>
            /// <param name="column">画像の列の位置(x座標)</param>
            /// <param name="ch">色番号 0, 1, 2</param>
            /// <returns></returns>
            public virtual int this[float row, float column, int ch]
            {
                get
                {
                    //return this[row, (float)Math.Floor(column) * (float)_channel + column - (float)Math.Floor(column) + (float)ch];
                    //return this[row, (float)Math.Floor(column) * (float)_channel + (column - (float)Math.Floor(column)) + (float)ch];

                    int value = 0;


                    if (_interpolation == InterpolationModeEnum.Bilinear)
                    {
                        int X1 = (int)Math.Floor(column);
                        double dX = (double)column - (double)X1;

                        int X = X1 * _channel + ch;

                        int Y1 = (int)Math.Floor(row);
                        double dY = (double)row - (double)Y1;

                        value = (int)(
                            (1.0 - dX) * ((1.0 - dY) * this[Y1, X] + dY * this[Y1 + 1, X])
                          + dX * ((1.0 - dY) * this[Y1, X + _channel] + dY * this[Y1 + 1, X + _channel])
                          + 0.5);
                    }
                    else if (_interpolation == InterpolationModeEnum.NearestNeighbor)
                    {
                        int X1 = (int)Math.Floor(column + 0.5);
                        int Y1 = (int)Math.Floor(row + 0.5);

                        value = this[Y1, X1 * _channel + ch];
                    }
                    else if (_interpolation == InterpolationModeEnum.Bicubic)
                    {
                        int X = (int)Math.Floor(column) * _channel + ch;
                        int Y = (int)Math.Floor(row);

                        double dX = column;
                        double dY = row;
                        double flX = Math.Floor(dX);
                        double flY = Math.Floor(dY);

                        double hX1 = BiCubicWeight(1.0 + dX - flX);
                        double hX2 = BiCubicWeight(dX - flX);
                        double hX3 = BiCubicWeight(flX + 1.0 - dX);
                        double hX4 = BiCubicWeight(flX + 2.0 - dX);

                        double hY1 = BiCubicWeight(1.0 + dY - flY);
                        double hY2 = BiCubicWeight(dY - flY);
                        double hY3 = BiCubicWeight(flY + 1.0 - dY);
                        double hY4 = BiCubicWeight(flY + 2.0 - dY);

                        double tX1 = this[Y - 1, X - _channel] * hX1
                                    + this[Y - 1, X] * hX2
                                    + this[Y - 1, X + _channel] * hX3
                                    + this[Y - 1, X + _channel * 2] * hX4;

                        double tX2 = this[Y, X - _channel] * hX1
                                    + this[Y, X] * hX2
                                    + this[Y, X + _channel] * hX3
                                    + this[Y, X + _channel * 2] * hX4;

                        double tX3 = this[Y + 1, X - _channel] * hX1
                                    + this[Y + 1, X] * hX2
                                    + this[Y + 1, X + _channel] * hX3
                                    + this[Y + 1, X + _channel * 2] * hX4;

                        double tX4 = this[Y + 2, X - _channel] * hX1
                                   + this[Y + 2, X] * hX2
                                   + this[Y + 2, X + _channel] * hX3
                                   + this[Y + 2, X + _channel * 2] * hX4;

                        value = (int)(
                                    hY1 * tX1
                                  + hY2 * tX2
                                  + hY3 * tX3
                                  + hY4 * tX4
                                  + 0.5
                            );

                        value = (value < 0) ? 0 : ((value > 255) ? 255 : value);
                    }

                    return value;
                }
            }

            /// <summary>
            /// メモリの解放を行います。
            /// </summary>
            public void Dispose()
            {
                // メモリの解放
                AlignedFree();
            }

            ~ImageData()
            {
                //if (_scan0Origin != IntPtr.Zero)
                //Debug.WriteLine("AlignedBuffer : Disposeし忘れ。");

                Dispose();
            }

            /////////////////////////////////////////////////////////////////////////////////////
            /// プロパティ
            /////////////////////////////////////////////////////////////////////////////////////

            private int _widthOriginal = 0;
            private int _heightOriginal = 0;

            private int _width = 0;
            /// <summary>
            /// 画像の幅
            /// </summary>
            public int Width
            {
                get { return this._width; }
            }

            private int _height = 0;
            /// <summary>
            /// 画像の高さ
            /// </summary>
            public int Height
            {
                get { return _height; }
            }

            private int _stride = 0;
            /// <summary>
            /// 画像データ幅のバイト数
            /// </summary>
            public int Stride
            {
                get { return _stride; }
            }

            private int _channel = 0;
            /// <summary>
            /// 画像のチャンネル数
            /// </summary>
            public int Channel
            {
                get { return _channel; }
            }

            PixelFormat _pixelFormat = PixelFormat.Undefined;
            /// <summary>
            /// 画像のピクセルフォーマット
            /// </summary>
            public PixelFormat PixelFormat
            {
                get { return _pixelFormat; }
            }

            private int _imageBit = 0;
            /// <summary>
            /// 画像のビット数
            /// </summary>
            public int ImageBit
            {
                get { return _imageBit; }
            }

            private int _bufferBit = 0;
            /// <summary>
            /// 画像メモリのビット数
            /// </summary>
            public int BufferBit
            {
                get { return _bufferBit; }
            }

            private IntPtr _scan0 = IntPtr.Zero;
            /// <summary>
            /// 画像データのポインタ
            /// </summary>
            public IntPtr Scan0
            {
                set { _scan0 = value; }
                get { return _scan0; }
            }

            private int _ByteAlignment = 0;
            /// <summary>
            /// アライメント単位のバイト数
            /// </summary>
            public int ByteAlignment
            {
                get { return _ByteAlignment; }
            }

            private BorderTypeEnum _BorderType = BorderTypeEnum.Mirror;
            /// <summary>
            ///  画像の輪郭位置の参照方法の取得、設定
            /// </summary>
            public BorderTypeEnum BorderType
            {
                set { _BorderType = value; }
                get { return _BorderType; }
            }

            private Rectangle _region = Rectangle.Empty;
            /// <summary>
            ///  画像の輪郭位置の参照方法の取得、設定
            /// </summary>
            public Rectangle Region
            {
                set
                {

                    int X1, X2, Y1, Y2;

                    X1 = value.X;
                    Y1 = value.Y;
                    X2 = value.X + value.Width - 1;
                    Y2 = value.Y + value.Height - 1;

                    if (X1 < 0) X1 = 0;
                    if (Y1 < 0) Y1 = 0;

                    if (X2 > _widthOriginal - 1) X2 = _widthOriginal - 1;
                    if (Y2 > _heightOriginal - 1) Y2 = _heightOriginal - 1;

                    _region.X = X1;
                    _region.Y = Y1;
                    _region.Width = X2 - X1 + 1;
                    _region.Height = Y2 - Y1 + 1;

                    _width = _region.Width;
                    _height = _region.Height;

                    // ポインタのオフセット
                    if (_bufferBit == 16)
                    {
                        _scan0 = IntPtr.Add(_scan0Origin, _offset + X1 * _channel * 2 + Y1 * _stride);
                    }
                    else
                    {
                        _scan0 = IntPtr.Add(_scan0Origin, _offset + X1 * _channel + Y1 * _stride);
                    }
                }

                get { return _region; }
            }

            /// <summary>
            /// ストライドの計算
            /// </summary>
            /// <param name="Width">画像の幅</param>
            /// <param name="BitCount">画像データ（メモリ）のビット数</param>
            /// <returns></returns>
            private int CalcStride(int width, int bitCount)
            {
                return (width * bitCount + 31) / 32 * 4;
            }

            private InterpolationModeEnum _interpolation = InterpolationModeEnum.Bilinear;
            /// <summary>
            ///  画像の輪郭位置の参照方法の取得、設定
            /// </summary>
            public InterpolationModeEnum InterpolationMode
            {
                set { _interpolation = value; }
                get { return _interpolation; }
            }

            private double _biCubicVal = -0.75;

            /// <summary>
            /// biCubic計算時の係数
            /// </summary>
            public double BiCubicVal
            {
                set { _biCubicVal = value; }
                get { return _biCubicVal; }
            }

            private MinusValueModeEnum _minusValueMode = MinusValueModeEnum.Zero;
            /// <summary>
            /// マイナスの値設定時の処理
            /// </summary>
            public MinusValueModeEnum MinusValueMode
            {
                set { _minusValueMode = value; }
                get { return _minusValueMode; }
            }


            /////////////////////////////////////////////////////////////////////////////////////
            /// メモリ関連
            /////////////////////////////////////////////////////////////////////////////////////

            private IntPtr _scan0Origin;    // 確保されたメモリ
            private int _size;              // 確保したメモリのサイズ
            private int _offset;            // ROIで示された画像左上までのポインタオフセット
            private object _lockObj = new object();

            /// <summary>
            /// アライメントされたメモリの確保
            /// </summary>
            /// <param name="size">確保するメモリのサイズ</param>
            /// <param name="byteAlignment">何バイトアライメントにするか？4, 8, 16, 32,・・・</param>
            /// <param name="zeroClear"></param>
            /// <param name="scan0Origin">確保されたメモリのポインタ</param>
            /// <returns>アライメント調整されたメモリのポインタ</returns>
            private IntPtr AlignedMalloc(int size, int byteAlignment, bool zeroClear, out IntPtr scan0Origin)
            {
                if (!IsValidByteAlignment(byteAlignment))
                    throw new ArgumentException("byteAlignmentの値が不正。");
                if (size <= 0)
                    throw new ArgumentException("sizeの値が不正。");

                scan0Origin = Marshal.AllocHGlobal(size + byteAlignment - 1);
                _size = size;
                _ByteAlignment = byteAlignment;

                UInt64 p = (UInt64)_scan0Origin;
                _offset = byteAlignment - (int)(p % (UInt64)byteAlignment);
                if (_offset == byteAlignment)
                    _offset = 0;

                if (zeroClear)
                    ZeroClear();

                // アライメント調整されたポインタ
                return IntPtr.Add(scan0Origin, _offset);
            }

            /// <summary>
            /// メモリの解放
            /// </summary>
            private void AlignedFree()
            {
                if (_scan0Origin != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_scan0Origin);
                    _scan0Origin = IntPtr.Zero;
                }
                _scan0 = IntPtr.Zero;
            }

            private IntPtr ThreadUnsafePointer
            {
                get
                {
                    return _scan0Origin + _offset;
                }
            }

            private IntPtr Lock()
            {
                Monitor.Enter(_lockObj);
                return ThreadUnsafePointer;
            }

            private void Unlock()
            {
                Monitor.Exit(_lockObj);
            }

            private bool IsValidByteAlignment(int byteAlignment)
            {
                int ByteAlignmentMax = 1073741824;
                int ByteAlignmentMin = 4;

                for (int i = ByteAlignmentMin; i <= ByteAlignmentMax; i <<= 1)
                    if (byteAlignment == i)
                        return true;

                return false;
            }

            /////////////////////////////////////////////////////////////////////////////////////
            // メソッド
            /////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// 画像の輝度値を０に初期化します。
            /// </summary>
            public virtual void ZeroClear()
            {
                lock (_lockObj)
                {
                    ZeroMemory(_scan0Origin, _size + _ByteAlignment - 1);
                }
            }

            /// <summary>
            /// imgAとimgBの画像の大きさを比較する
            /// </summary>
            /// <param name="imgA">比較するImageDataクラスオブジェクト</param>
            /// <param name="imgB">比較するImageDataクラスオブジェクト</param>
            /// <returns>大きさが同じ場合true、異なる場合false</returns>
            public static bool IsEqualImageSize(ImageData imgA, ImageData imgB)
            {
                if (
                    (imgA.Width != imgB.Width) ||
                    (imgA.Height != imgB.Height)
                    ) return false;

                return true;
            }

            /// <summary>
            /// ImageDataクラスオブジェクトをBitmapクラスオブジェクトへ変換
            /// </summary>
            /// <returns>Bitmapクラスオブジェクト</returns>
            public virtual Bitmap ToBitmap()
            {
                if ((_width <= 0) ||
                    (_height <= 0) ||
                    (_stride <= 0) ||
                    (_pixelFormat == System.Drawing.Imaging.PixelFormat.Undefined) ||
                    (_scan0 == IntPtr.Zero))
                {
                    return null;
                }

                Bitmap bmp;

                if ((_stride % 4) == 0)
                {
                    bmp = new Bitmap(_width, _height, _stride, _pixelFormat, _scan0);

                    // モノクロの場合はカラーパレットを設定
                    if (_pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    {
                        var pal = bmp.Palette;

                        for (int i = 0; i < 256; i++)
                        {
                            pal.Entries[i] = Color.FromArgb(255, i, i, i);
                        }
                        bmp.Palette = pal;
                    }
                }
                else
                {
                    // Strideが４の倍数でない場合
                    bmp = NoBoundaryToBitmap();
                }

                return bmp;
            }

            /// <summary>
            /// クローンの作成
            /// </summary>
            /// <param name="copyFlg">画像データをコピーする場合はtrue、しない場合はfalse</param>
            /// <returns></returns>
            public virtual ImageData Clone(bool copyFlg)
            {
                var img = new ImageData(this.Width, this.Height, this.ImageBit, this.BufferBit);

                img.BiCubicVal = this.BiCubicVal;
                img.BorderType = this.BorderType;
                img.InterpolationMode = this.InterpolationMode;
                img.MinusValueMode = this.MinusValueMode;

                if (copyFlg == true)
                {
                    if (_region == Rectangle.Empty)
                    {
                        CopyMemory(img.Scan0, _scan0, _stride * _height);
                    }
                    else
                    {
                        CopyTo(img);
                    }
                }
                return img;
            }

            /// <summary>
            /// クローンの作成
            /// </summary>
            /// <returns></returns>
            public virtual ImageData Clone()
            {
                return Clone(true);
            }

            /// <summary>
            /// 画像のコピー
            /// </summary>
            /// <param name="dst">コピー先のImageDataクラスオブジェクト</param>
            /// <returns></returns>
            public virtual bool CopyTo(ImageData dst)
            {
                if (
                    (_width != dst.Width) ||
                    (_height != dst.Height) ||
                    (_imageBit != dst.ImageBit) ||
                    (_bufferBit != dst.BufferBit)
                    ) return false;

                if ((_region == Rectangle.Empty) && (dst.Region == Rectangle.Empty))
                {
                    CopyMemory(dst.Scan0, _scan0, _stride * _height);
                }
                else
                {
                    if (_bufferBit != 16)
                    {
                        for (int j = 0; j < _height; j++)
                        {
                            CopyMemory(IntPtr.Add(dst.Scan0, dst.Stride * j), IntPtr.Add(_scan0, _stride * j), _width * _channel);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < _height; j++)
                        {
                            CopyMemory(IntPtr.Add(dst.Scan0, dst.Stride * j), IntPtr.Add(_scan0, _stride * j), _width * 2 * _channel);
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// Regionの初期化
            /// </summary>
            public virtual void ResetRegion()
            {
                this.Region = Rectangle.Empty;
                _scan0 = _scan0Origin + _offset;

                // オリジナルサイズ
                _width = _widthOriginal;
                _height = _heightOriginal;
            }

            /// <summary>
            /// ビットマップファイルをバイナリで読込
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            private int LoadBitmapBinary(string filename)
            {
                BITMAPFILEHEADER bfh;
                BITMAPINFOHEADER bih;
                System.Drawing.Color[] ColorPal;

                // データ読込用配列の確保
                byte[] ReadData = new byte[4];
                try
                {
                    using (var fs = System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        //////////////////////////////////
                        //
                        // BITMAPFILEHEADERの読込
                        //
                        //////////////////////////////////

                        // bfType
                        fs.Read(ReadData, 0, 2);
                        bfh.bfType = BitConverter.ToUInt16(ReadData, 0);
                        // bfSize
                        fs.Read(ReadData, 0, 4);
                        bfh.bfSize = BitConverter.ToUInt32(ReadData, 0);
                        // bfReserved1
                        fs.Read(ReadData, 0, 2);
                        bfh.bfReserved1 = BitConverter.ToUInt16(ReadData, 0);
                        // bfReserved2
                        fs.Read(ReadData, 0, 2);
                        bfh.bfReserved2 = BitConverter.ToUInt16(ReadData, 0);
                        // bfOffBits
                        fs.Read(ReadData, 0, 4);
                        bfh.bfOffBits = BitConverter.ToUInt32(ReadData, 0);

                        //////////////////////////////////
                        //
                        // BITMAPINFOHEADERの読込
                        //
                        //////////////////////////////////

                        // biSize
                        fs.Read(ReadData, 0, 4);
                        bih.biSize = BitConverter.ToUInt32(ReadData, 0);
                        // biWidth
                        fs.Read(ReadData, 0, 4);
                        bih.biWidth = BitConverter.ToInt32(ReadData, 0);
                        // biHeight
                        fs.Read(ReadData, 0, 4);
                        bih.biHeight = BitConverter.ToInt32(ReadData, 0);
                        // biPlanes
                        fs.Read(ReadData, 0, 2);
                        bih.biPlanes = BitConverter.ToUInt16(ReadData, 0);
                        //biBitCount
                        fs.Read(ReadData, 0, 2);
                        bih.biBitCount = BitConverter.ToUInt16(ReadData, 0);
                        // biCompression
                        fs.Read(ReadData, 0, 4);
                        bih.biCompression = BitConverter.ToUInt32(ReadData, 0);
                        // biSizeImage
                        fs.Read(ReadData, 0, 4);
                        bih.biSizeImage = BitConverter.ToUInt32(ReadData, 0);
                        // biXPelsPerMeter
                        fs.Read(ReadData, 0, 4);
                        bih.biXPelsPerMeter = BitConverter.ToInt32(ReadData, 0);
                        // biYPelsPerMeter
                        fs.Read(ReadData, 0, 4);
                        bih.biYPelsPerMeter = BitConverter.ToInt32(ReadData, 0);
                        // biClrUsed
                        fs.Read(ReadData, 0, 4);
                        bih.biClrUsed = BitConverter.ToUInt32(ReadData, 0);
                        // biClrImportant
                        fs.Read(ReadData, 0, 4);
                        bih.biClrImportant = BitConverter.ToUInt32(ReadData, 0);

                        //////////////////////////////////
                        //
                        // カラーパレットの読込
                        //
                        //////////////////////////////////

                        // カラーパレットのサイズの計算
                        //   bfOffBitsからBITMAPFILEHEADERとBITMAPINFOHEADERのサイズ文を
                        //   引いたのがカラーパレットのサイズ
                        long PalSize = (bfh.bfOffBits - 14 - 40) / 4;

                        if (PalSize != 0)
                        {
                            ColorPal = new System.Drawing.Color[PalSize];

                            for (int i = 0; i < PalSize; i++)
                            {
                                fs.Read(ReadData, 0, 4);
                                ColorPal[i] =
                                    System.Drawing.Color.FromArgb(
                                            ReadData[3], // A
                                            ReadData[2], // R
                                            ReadData[1], // G
                                            ReadData[0]  // B
                                            );
                            }
                        }
                        else
                        {
                            ColorPal = null;
                        }

                        //////////////////////////////////
                        //
                        // 有効ビット数の計算
                        //
                        //////////////////////////////////
                        if (bih.biCompression == 0)
                        {
                            _imageBit = bih.biBitCount;
                        }
                        else
                        {
                            if (bih.biBitCount != 32)   // モノクロの場合
                            {
                                // ビットフィールドの時、Bを代表して有効ビットを調べる
                                ReadData[0] = ColorPal[2].B;
                                ReadData[1] = ColorPal[2].G;
                                ReadData[2] = ColorPal[2].R;
                                ReadData[3] = ColorPal[2].A;

                                UInt32 bf = BitConverter.ToUInt32(ReadData, 0);

                                int effectiveBits = 16;

                                for (int i = 0; i < 32; i++)
                                {
                                    if ((bf & (1 << i)) == (1 << i))
                                    {
                                        effectiveBits = i + 1;
                                    }
                                }
                                _imageBit = effectiveBits;
                                _maxVale = (int)(Math.Pow(2, _imageBit)) - 1;

                            }
                            else
                            {
                                // カラーの場合（30bitのみ）
                                _imageBit = 30;
                                _maxVale = 1023;
                            }
                        }

                        _width = bih.biWidth;
                        _height = Math.Abs(bih.biHeight);

                        // オリジナルサイズ
                        _widthOriginal = _width;
                        _heightOriginal = _height;

                        _bufferBit = _imageBit;
                        _channel = _bufferBit / 8;

                        if ((8 < _bufferBit) && (_bufferBit <= 16))
                        {
                            _bufferBit = 16;
                            _channel = 1;
                        }
                        else if (_bufferBit == 30)
                        {
                            _bufferBit = 32;
                            _channel = 3;
                        }

                        if (_bufferBit == 8)
                        {
                            _pixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                        }
                        else if (_bufferBit == 16)
                        {
                            _pixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
                        }
                        else if (_bufferBit == 24)
                        {
                            _pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        }
                        else if (_bufferBit == 32)
                        {
                            if (_imageBit == 30)
                            {
                                _pixelFormat = System.Drawing.Imaging.PixelFormat.Indexed;
                            }
                            else
                            {
                                _pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                            }
                        }
                        else
                        {
                            _pixelFormat = System.Drawing.Imaging.PixelFormat.Undefined;
                        }

                        //////////////////////////////////
                        //
                        // 画像データ（輝度値）の読込
                        //
                        //////////////////////////////////

                        _stride = CalcStride(_width, _bufferBit);

                        // アライメント調整されたメモリの確保
                        _scan0 = AlignedMalloc(_stride * _height, 32, true, out _scan0Origin);

                        //メモリの確保
                        Byte[] BitData = new Byte[_stride * Math.Abs(bih.biHeight)];

                        //画像データを画像の下側から読み込む（上下を反転させて読み込む）
                        if (bih.biHeight > 0)
                        {
                            for (int j = bih.biHeight - 1; j >= 0; j--)
                            {
                                fs.Read(BitData, j * _stride, Stride);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < Math.Abs(bih.biHeight); j++)
                            {
                                fs.Read(BitData, j * _stride, Stride);
                            }
                        }

                        bih.biHeight = Math.Abs(bih.biHeight);

                        // 画像メモリをコピー
                        Marshal.Copy(BitData, 0, _scan0, _stride * _height);

                        BitData = null;

                        //ファイルを閉じる
                        fs.Close();
                    }
                }
                catch
                {
                    _scan0 = IntPtr.Zero;
                }

                return 0;

            }

            /// <summary>
            /// 画像データのファイル保存
            /// </summary>
            /// <param name="fileName">ファイル名(*.bmp,)</param>
            /// <returns></returns>
            public virtual bool Save(String filename)
            {
                if (_region != Rectangle.Empty)
                {
                    var id = this.Clone();

                    bool ret = id.Save(filename);

                    id.Dispose();

                    return ret;
                }

                // 拡張子の確認
                String ext = System.IO.Path.GetExtension(filename).ToLower();

                if (ext != ".bmp")
                {
                    if ((ext == ".gif") || (ext == ".jpg") || (ext == ".jpeg") || (ext == ".png") || (ext == ".tif") || (ext == ".tiff"))// BMP、GIF、EXIG、JPG、PNG、および TIFF 
                    {
                        try
                        {
                            System.Drawing.Bitmap bmp = NoBoundaryToBitmap();

                            System.Drawing.Imaging.ImageFormat imgformat = null;
                            if (ext == ".gif")
                                imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                            else if (ext == ".jpg")
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                            else if ((ext == ".jpg") || (ext == ".jpeg"))
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                            else if (ext == ".png")
                                imgformat = System.Drawing.Imaging.ImageFormat.Png;
                            else if ((ext == ".tif") || (ext == ".tiff"))
                                imgformat = System.Drawing.Imaging.ImageFormat.Tiff;

                            // ファイル保存
                            bmp.Save(filename, imgformat);

                            bmp = null;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else if ((ext == ".csv"))
                    {
                        try
                        {

                            using (var sw = new System.IO.StreamWriter(filename, false, System.Text.Encoding.GetEncoding("shift-jis")))
                            {
                                int i, j;

                                for (j = 0; j < _height; j++)
                                {
                                    for (i = 0; i < _width * _channel - 1; i++)
                                    {
                                        sw.Write(this[j, i].ToString() + ", ");
                                    }
                                    sw.WriteLine(this[j, _width * _channel - 1].ToString());
                                }
                                sw.Close();
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // bmpの場合はバイナリで保存する（多ビット対応のため）

                    //カラーパレットの個数
                    UInt32 PalSize = 0;
                    byte[] ColorPal = null;
                    if (_bufferBit == 8)
                    {
                        PalSize = 256;
                        //カラーパレットをバイト配列で確保
                        ColorPal = new byte[PalSize * 4];
                        for (int i = 0; i < 256; i++)
                        {
                            ColorPal[i * 4] = (byte)i;    //B
                            ColorPal[i * 4 + 1] = (byte)i;    //G
                            ColorPal[i * 4 + 2] = (byte)i;    //R
                            ColorPal[i * 4 + 3] = 0;          //A
                        }
                    }
                    else if (_bufferBit == 16)
                    {
                        PalSize = 3;
                        //カラーパレットをバイト配列で確保
                        ColorPal = new byte[PalSize * 4];

                        // ビットフィールドの設定
                        for (int i = 0; i < PalSize; i++)
                        {
                            ColorPal[i * 4] = 255;      //B
                            ColorPal[i * 4 + 1] = (byte)((1 << (_imageBit - 8)) - 1);    //G
                            ColorPal[i * 4 + 2] = 0;    //R
                            ColorPal[i * 4 + 3] = 0;    //A
                        }
                    }
                    else if ((_bufferBit == 32) && (_imageBit == 30))
                    {
                        PalSize = 3;
                        ColorPal = new byte[PalSize * 4];

                        int i = 0;  // R
                        ColorPal[i * 4] = 255;
                        ColorPal[i * 4 + 1] = 3;
                        ColorPal[i * 4 + 2] = 0;
                        ColorPal[i * 4 + 3] = 0;
                        i = 1;  // G
                        ColorPal[i * 4] = 0;
                        ColorPal[i * 4 + 1] = 252;
                        ColorPal[i * 4 + 2] = 15;
                        ColorPal[i * 4 + 3] = 0;
                        i = 2;  // B
                        ColorPal[i * 4] = 0;
                        ColorPal[i * 4 + 1] = 0;
                        ColorPal[i * 4 + 2] = 240;
                        ColorPal[i * 4 + 3] = 63;
                    }

                    //BITMAPFILEHEADERの作成
                    BITMAPFILEHEADER bfh;
                    bfh.bfType = 0x4d42;
                    //bfh.bfSize = 0;
                    bfh.bfReserved1 = 0;
                    bfh.bfReserved2 = 0;
                    bfh.bfOffBits = 14 + 40 + PalSize * 4;
                    bfh.bfSize = bfh.bfOffBits + (uint)(_stride * _height);

                    //BITMAPINFOHEADERの作成
                    BITMAPINFOHEADER bih;
                    bih.biSize = 40;
                    bih.biWidth = _width;
                    bih.biHeight = _height;
                    bih.biPlanes = 1;
                    bih.biBitCount = (ushort)_bufferBit;
                    if (PalSize != 3)
                    {
                        bih.biCompression = 0;
                    }
                    else
                    {
                        bih.biCompression = 3;  // ビットフィールド
                    }
                    bih.biSizeImage = 0;
                    bih.biXPelsPerMeter = 0;
                    bih.biYPelsPerMeter = 0;
                    bih.biClrUsed = PalSize;
                    bih.biClrImportant = PalSize;

                    // ファイルを開く
                    using (System.IO.FileStream fs = System.IO.File.Open(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        //////////////////////////////////
                        //
                        // BITMAPFILEHEADERの書込
                        //
                        //////////////////////////////////

                        // bfType
                        fs.Write(BitConverter.GetBytes(bfh.bfType), 0, 2);
                        // bfSize
                        fs.Write(BitConverter.GetBytes(bfh.bfSize), 0, 4);
                        // bfReserved1
                        fs.Write(BitConverter.GetBytes(bfh.bfReserved1), 0, 2);
                        // bfReserved2
                        fs.Write(BitConverter.GetBytes(bfh.bfReserved2), 0, 2);
                        // bfOffBits
                        fs.Write(BitConverter.GetBytes(bfh.bfOffBits), 0, 4);

                        //////////////////////////////////
                        //
                        // BITMAPINFOHEADERの書込
                        //
                        //////////////////////////////////

                        // biSize
                        fs.Write(BitConverter.GetBytes(bih.biSize), 0, 4);
                        // biWidth
                        fs.Write(BitConverter.GetBytes(bih.biWidth), 0, 4);
                        // biHeight
                        fs.Write(BitConverter.GetBytes(bih.biHeight), 0, 4);
                        // biPlanes
                        fs.Write(BitConverter.GetBytes(bih.biPlanes), 0, 2);
                        //biBitCount
                        fs.Write(BitConverter.GetBytes(bih.biBitCount), 0, 2);
                        // biCompression
                        fs.Write(BitConverter.GetBytes(bih.biCompression), 0, 4);
                        // biSizeImage
                        fs.Write(BitConverter.GetBytes(bih.biSizeImage), 0, 4);
                        // biXPelsPerMeter
                        fs.Write(BitConverter.GetBytes(bih.biXPelsPerMeter), 0, 4);
                        // biYPelsPerMeter
                        fs.Write(BitConverter.GetBytes(bih.biYPelsPerMeter), 0, 4);
                        // biClrUsed
                        fs.Write(BitConverter.GetBytes(bih.biClrUsed), 0, 4);
                        // biClrImportant
                        fs.Write(BitConverter.GetBytes(bih.biClrImportant), 0, 4);

                        //////////////////////////////////
                        //
                        // カラーパレットの書込
                        //
                        //////////////////////////////////

                        if (PalSize != 0)
                        {
                            fs.Write(ColorPal, 0, ColorPal.Length);
                        }

                        //////////////////////////////////
                        //
                        // 画像データ（輝度値）の書込
                        //
                        //////////////////////////////////

                        int stride = (_stride + 3) / 4 * 4;

                        Byte[] BitData = new Byte[stride * bih.biHeight];

                        // 画像データを配列にコピー
                        Marshal.Copy(_scan0, BitData, 0, stride * _height);

                        //画像データを画像の下側から書き込む（上下を反転させて書き込む）
                        for (int j = bih.biHeight - 1; j >= 0; j--)
                        {
                            fs.Write(BitData, j * stride, stride);
                        }

                        BitData = null;

                        //ファイルを閉じる
                        fs.Close();
                    }
                }
                // 正常書込
                return true;
            }

            /// <summary>
            /// Strideが４の倍数でない場合のBitmap変換
            /// </summary>
            /// <returns></returns>
            private Bitmap NoBoundaryToBitmap()
            {
                // Strideが４の倍数でない場合
                Bitmap bmp = new Bitmap(_width, _height, _pixelFormat);

                IntPtr scan0;
                int stride;

                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, _width, _height),
                    ImageLockMode.ReadWrite,
                    _pixelFormat);

                scan0 = data.Scan0;
                stride = data.Stride;

                for (int j = 0; j < _height; j++)
                {
                    CopyMemory(IntPtr.Add(scan0, stride * j), IntPtr.Add(_scan0, _stride * j), _stride);
                }
                bmp.UnlockBits(data);

                // モノクロの場合はカラーパレットを設定
                if (_pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    var pal = bmp.Palette;

                    for (int i = 0; i < 256; i++)
                    {
                        pal.Entries[i] = Color.FromArgb(255, i, i, i);
                    }
                    bmp.Palette = pal;
                }

                return bmp;

            }

            /////////////////////////////////////////////////////////////////////////////////////
            /// Operator
            /////////////////////////////////////////////////////////////////////////////////////

            //二項+演算子をオーバーロードする


            /// <summary>
            /// ２つの画像の各画素の輝度値を加算する
            /// </summary>
            /// <param name="imgA">加算する画像A</param>
            /// <param name="imgB">加算する画像B</param>
            /// <returns>imgAとimgBが加算された画像</returns>      
            public static ImageData operator +(ImageData imgA, ImageData imgB)
            {
                if ((imgA == null) || (imgB == null)) return null;

                if (
                    (imgA.Width != imgB.Width) ||
                    (imgA.Height != imgB.Height) ||
                    (imgA.ImageBit != imgB.ImageBit) ||
                    (imgA.BufferBit != imgB.BufferBit)
                    ) return null;

                int i, j, ch;
                int width = imgA.Width;
                int height = imgA.Height;
                int channel = imgA.Channel;

                var resultImg = imgA.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = imgA[j, i, ch] + imgB[j, i, ch];
                        }

                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にvalueを加算
            /// </summary>
            /// <param name="img">加算される画像</param>
            /// <param name="value">加算する値</param>
            /// <returns></returns>
            public static ImageData operator +(ImageData img, int value)
            {
                if (img == null) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = img[j, i, ch] + value;
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にvalueを加算
            /// </summary>
            /// <param name="value">加算する値</param>
            /// <param name="img">加算される画像</param>
            /// <returns></returns>
            public static ImageData operator +(int value, ImageData img)
            {
                return (img + value);
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を加算
            /// </summary>
            /// <param name="img">加算される画像</param>
            /// <param name="value">加算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <returns></returns>
            public static ImageData operator +(ImageData img, int[] value)
            {
                if (img == null) return null;
                if (value.Length < img.Channel) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = img[j, i, ch] + value[ch];
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を加算
            /// </summary>
            /// <param name="value">加算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <param name="img">加算される画像</param>
            /// <returns></returns>
            public static ImageData operator +(int[] value, ImageData img)
            {
                return (value + img);
            }

            /// <summary>
            /// ２つの画像の各画素の輝度値を減算する
            /// </summary>
            /// <param name="imgA">減算する画像A</param>
            /// <param name="imgB">減算する画像B</param>
            /// <returns>imgAとimgBが減算された画像</returns>      
            public static ImageData operator -(ImageData imgA, ImageData imgB)
            {
                if ((imgA == null) || (imgB == null)) return null;

                if (
                    (imgA.Width != imgB.Width) ||
                    (imgA.Height != imgB.Height) ||
                    (imgA.ImageBit != imgB.ImageBit) ||
                    (imgA.BufferBit != imgB.BufferBit)
                    ) return null;

                int i, j, ch;
                int width = imgA.Width;
                int height = imgA.Height;
                int channel = imgA.Channel;

                var resultImg = imgA.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = imgA[j, i, ch] - imgB[j, i, ch];
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にvalueを減算
            /// </summary>
            /// <param name="img">減算される画像</param>
            /// <param name="value">減算する値</param>
            /// <returns></returns>
            public static ImageData operator -(ImageData img, int value)
            {
                return (img + (-value));
            }

            /// <summary>
            /// 画像の全輝度値にvalueを減算
            /// </summary>
            /// <param name="value">減算する値</param>
            /// <param name="img">減算する画像</param>
            /// <returns></returns>
            public static ImageData operator -(int value, ImageData img)
            {
                if (img == null) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = value - img[j, i, ch];
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を減算
            /// </summary>
            /// <param name="img">減算される画像</param>
            /// <param name="value">加算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <returns></returns>
            public static ImageData operator -(ImageData img, int[] value)
            {
                var val = new int[value.Length];
                if (value.Length < img.Channel) return null;

                for (int i = 0; i < val.Length; i++)
                {
                    val[i] = -value[i];
                }
                return (img + val);
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を減算
            /// </summary>
            /// <param name="img">減算される画像</param>
            /// <param name="value">加算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <returns></returns>
            public static ImageData operator -(int[] value, ImageData img)
            {
                if (img == null) return null;
                if (value.Length < img.Channel) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = value[ch] - img[j, i, ch];
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にimgAとimgBを積算
            /// </summary>
            /// <param name="imgA">積算される画像</param>
            /// <param name="imgB">積算する画像</param>
            /// <returns></returns>
            public static ImageData operator *(ImageData imgA, ImageData imgB)
            {
                if ((imgA == null) || (imgB == null)) return null;

                if (
                    (imgA.Width != imgB.Width) ||
                    (imgA.Height != imgB.Height) ||
                    (imgA.ImageBit != imgB.ImageBit) ||
                    (imgA.BufferBit != imgB.BufferBit)
                    ) return null;

                int i, j, ch;
                int width = imgA.Width;
                int height = imgA.Height;
                int channel = imgA.Channel;

                var resultImg = imgA.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = imgA[j, i, ch] * imgB[j, i, ch];
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にvalueを積算
            /// </summary>
            /// <param name="img">積算される画像</param>
            /// <param name="value">積算する値</param>
            /// <returns></returns>
            public static ImageData operator *(ImageData img, double value)
            {
                if (img == null) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = (int)(img[j, i, ch] * value + 0.5);
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値にvalueを積算
            /// </summary>
            /// <param name="value">積算する値</param>
            /// <param name="img">積算される画像</param>
            /// <returns></returns>
            public static ImageData operator *(int value, ImageData img)
            {
                return (img * value);
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を積算
            /// </summary>
            /// <param name="img">積算される画像</param>
            /// <param name="value">積算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <returns></returns>
            public static ImageData operator *(ImageData img, double[] value)
            {
                if (img == null) return null;
                if (value.Length < img.Channel) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            resultImg[j, i, ch] = (int)(img[j, i, ch] * value[ch] + 0.5);
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を積算
            /// </summary>
            /// <param name="value">積算する値の配列 b, g, r, aの順番に格納すること</param>
            /// <param name="img">積算する画像</param>
            /// <returns></returns>
            public static ImageData operator *(double[] value, ImageData img)
            {
                return (img * value);
            }

            /// <summary>
            /// 画像の全輝度値にvalueを徐算
            /// </summary>
            /// <param name="img">徐算される画像</param>
            /// <param name="value">徐算する値</param>
            /// <returns></returns>
            public static ImageData operator /(ImageData img, double value)
            {
                return (img * (1.0 / value));
            }

            /// <summary>
            /// 画像の全輝度値にvalueを徐算
            /// </summary>
            /// <param name="value">徐算する値</param>
            /// <param name="img">徐算される画像</param>
            /// <returns></returns>
            public static ImageData operator /(double value, ImageData img)
            {
                if (img == null) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            if (img[j, i, ch] == 0)
                            {
                                resultImg[j, i, ch] = 0;
                            }
                            else
                            {
                                resultImg[j, i, ch] = (int)(value / img[j, i, ch] * +0.5);
                            }
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を徐算
            /// </summary>
            /// <param name="img">徐算する画像</param>
            /// <param name="value">徐算する値の配列 b, g, r, aの徐算に格納すること</param>
            /// <returns></returns>
            public static ImageData operator /(ImageData img, double[] value)
            {
                if (img == null) return null;
                if (value.Length < img.Channel) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            if (value[ch] != 0)
                            {
                                resultImg[j, i, ch] = (int)(img[j, i, ch] / value[ch] + 0.5);
                            }
                            else
                            {
                                resultImg[j, i, ch] = 0;
                            }
                        }
                    }
                }
                return resultImg;
            }

            /// <summary>
            /// 画像の全輝度値に各チャンネルごとにvalue[]を徐算
            /// </summary>
            /// <param name="value">徐算する値の配列 b, g, r, aの徐算に格納すること</param>
            /// <param name="img">徐算する画像</param>
            /// <returns></returns>
            public static ImageData operator /(double[] value, ImageData img)
            {
                if (img == null) return null;
                if (value.Length < img.Channel) return null;

                int i, j, ch;
                int width = img.Width;
                int height = img.Height;
                int channel = img.Channel;

                var resultImg = img.Clone(false);

                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width; i++)
                    {
                        for (ch = 0; ch < channel; ch++)
                        {
                            if (img[j, i, ch] != 0)
                            {
                                resultImg[j, i, ch] = (int)(value[ch] / img[j, i, ch] + 0.5);
                            }
                            else
                            {
                                resultImg[j, i, ch] = 0;
                            }

                        }
                    }
                }
                return resultImg;
            }
        }
    }
}
