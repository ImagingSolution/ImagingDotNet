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
        private string _srcFilename;// 元画像のファイル名

        public Gasyori100knock()
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
            //ファイルを開くダイアログボックスの作成  
            var ofd = new OpenFileDialog();
            //ファイルフィルタ  
            ofd.Filter = "Image File(*.bmp,*.jpg,*.png,*.tif)|*.bmp;*.jpg;*.png;*.tif|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png";
            //ダイアログの表示 （Cancelボタンがクリックされた場合は何もしない）
            if (ofd.ShowDialog() == DialogResult.Cancel) return;

            // 画像ファイルを開く
            LoadImage(ofd.FileName);


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

        /// <summary>
        /// 画像ファイルの読み込み、表示
        /// </summary>
        /// <param name="filename">画像ファイル名</param>
        private void LoadImage(string filename)
        {
            _srcFilename = filename;

            var bmp = ImagingDotNet.CreateBitmap(filename);

            if (bmp != null)
            {
                if (_bitmap != null) _bitmap.Dispose();
                _bitmap = bmp;
            }
            else return;

            // 画像をピクチャボックスに合わせて表示するアフィン変換行列の計算
            _matAffine.ZoomFit(picImage, _bitmap);

            // ウィンドウ右下に画像情報を表示
            lblImageInfo.Text = $"{_bitmap.Width} x {_bitmap.Height} x {_bitmap.PixelFormat}";

            // 画像の表示
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
            if (_bitmap != null)
            {
                _graphics.DrawImage(_bitmap, _matAffine);
            }
            // 更新
            picImage.Refresh();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            // 元画像の再読み込み
            LoadImage(_srcFilename);
        }

        private void button_Click(object sender, EventArgs e)
        {
            // 画像処理の実行
            GasyoriExecute(sender);
        }


    }
}
