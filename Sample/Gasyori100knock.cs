using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using ImagingSolution;

namespace Sample
{
    public partial class Gasyori100knock : Form
    {
        private Graphics _graphics; // 画像表示用Graphicsオブジェクト
        private Bitmap _bitmap;     // 最後に表示したBitmapクラス
        private Matrix _matAffine;  // 表示用アフィン変換行列

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 画像表示用のアフィン変換行列
            _matAffine = new Matrix();

            // Graphicsオブジェクトの作成
            picImage.CreateGraphics(ref _graphics);

            // ホイールイベントの追加
            this.picImage.MouseWheel
                += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseWheel);

            // 画像ファイルのDropを有効にする
            picImage.AllowDrop = true;

            RedrawImage();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Graphicsオブジェクトの作成
            picImage.CreateGraphics(ref _graphics);

            _graphics.Clear(picImage.BackColor);

            if (_bitmap != null)
            {
                // アフィン変換行列に基づいて画像の表示
                _graphics.DrawImage(_bitmap, _matAffine);
                picImage.Refresh();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_bitmap != null) _bitmap.Dispose();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            // 画像ファイルを開く(ファイルを開くダイアログボックスも表示される)
            var bmp = ImagingDotNet.CreateBitmap();

            if (bmp != null)
            {
                if (_bitmap != null) _bitmap.Dispose();
                _bitmap = bmp;
            }
            else return;

            // 画像をピクチャボックスに合わせて表示するアフィン変換行列の計算
            _matAffine.ZoomFit(picImage, _bitmap);

            // 画像の表示
            RedrawImage();
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            // 画像データが設定されていない場合は何もしない
            if (_bitmap == null) return;

            // 名前を付けて画像データを保存
            _bitmap.SaveBitmap();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private PointF _oldPoint;

        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {
            picImage.Focus();
            _oldPoint.X = e.X;
            _oldPoint.Y = e.Y;
        }

        /// <summary>
        /// 画像の移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            // マウスをクリックしながら移動中のとき、画像の移動
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                // 画像の移動
                _matAffine.Translate(e.X - _oldPoint.X, e.Y - _oldPoint.Y,
                    System.Drawing.Drawing2D.MatrixOrder.Append);
                // 画像の描画
                RedrawImage();

                // ポインタ位置の保持
                _oldPoint.X = e.X;
                _oldPoint.Y = e.Y;
            }
        }
        /// <summary>
        /// マウスホイールイベント(画像の拡大縮小)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picImage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // 拡大
                if (_matAffine.Elements[0] < 100)  // X方向の倍率を代表してチェック
                {
                    // ポインタの位置周りに拡大
                    _matAffine.ScaleAt(1.5f, e.Location);
                }
            }
            else
            {
                // 縮小
                if (_matAffine.Elements[0] > 0.01)  // X方向の倍率を代表してチェック
                {
                    // ポインタの位置周りに縮小
                    _matAffine.ScaleAt(1.0f / 1.5f, e.Location);
                }
            }
            // 画像の描画
            RedrawImage();
        }

        private void picImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 画像をピクチャボックスに合わせて表示するアフィン変換行列の計算
            _matAffine.ZoomFit(picImage, _bitmap);

            // 画像の表示
            RedrawImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var bmpDst = _bitmap.CvtColor(ImagingDotNet.COLOR_BGR2GRAY);

            sw.Stop();
            MessageBox.Show($"CvtColor: {sw.ElapsedMilliseconds} msec");

            _bitmap.Dispose();
            _bitmap = bmpDst;

            RedrawImage();
        }

        /// <summary>
        /// 画像の描画
        /// </summary>
        private void RedrawImage()
        {
            // ピクチャボックスの背景で画像を削除
            _graphics.Clear(picImage.BackColor);
            // アフィン変換行列に基づいて画像の描画
            _graphics.DrawImage(_bitmap, _matAffine);
            // 更新
            picImage.Refresh();
        }


    }
}
