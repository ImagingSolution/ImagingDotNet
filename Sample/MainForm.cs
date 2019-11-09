using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImagingSolution;

namespace Sample
{
    public partial class MainForm : Form
    {
        private Graphics _graphics;

        public MainForm()
        {
            InitializeComponent();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {



            //using (var lb = new ImagingDotNet.LockBitmap(bmp))
            //{


            //    for (int y = 0; y < lb.Height; y++)
            //    {
            //        for (int x = 0; x < lb.Width; x++)
            //        {
            //            lb[y, x, 0] = (byte)((x + y) % 256);
            //            lb[y, x, 1] = (byte)((x + y) % 256);
            //            lb[y, x, 2] = (byte)((x + y) % 256);
            //        }
            //    }
            //}

            //picImage.Image = bmp;
            //picImage.Refresh();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var bmp = new Bitmap(6000, 4000, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            /*
            // Bitmapをロック
            using (var lb = new ImagingDotNet.LockBitmap(bmp))
            {
                // 画像データ格納用配列

                byte r, g, b;

                for (int y = 0; y < lb.Height; y++)
                {
                    // 行の先頭ポインタ
                    for (int x = 0; x < lb.Width; x++)
                    {
                        // 輝度値の取得
                        r = lb[y, x, 2];
                        g = lb[y, x, 1];
                        b = lb[y, x, 0];

                        // 輝度値の設定

                        lb[y, x, 2] = (byte)(255 - r);
                        lb[y, x, 1] = (byte)(255 - g);
                        lb[y, x, 0] = (byte)(255 - b);
                    }
                }
            }
            */
            /*
                        unsafe 
                        {
                            using (var lb = new ImagingDotNet.LockBitmap(bmp))
                            {
                                // 画像データ格納用配列
                                var ptr = (byte*)lb.Scan0;

                                Parallel.For(0, lb.Height, y =>
                                {
                                // 行の先頭ポインタ
                                byte* pLine = ptr + y * lb.Stride;

                                    for (int x = 0; x < lb.Width; x++)
                                    {
                                    // 輝度値の取得
                                    byte r = pLine[2];
                                        byte g = pLine[1];
                                        byte b = pLine[0];

                                    // 輝度値の設定
                                    pLine[2] = (byte)(255 - r);
                                        pLine[1] = (byte)(255 - g);
                                        pLine[0] = (byte)(255 - b);

                                    // 次の画素へ
                                    pLine += 3;
                                    }
                                }
                                );
                            }
                        }


                        sw.Stop();
                        MessageBox.Show($"NegativeImage1: {sw.ElapsedMilliseconds} msec");
                    }
            */
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _graphics = picImage.CreateGraphics(_graphics);

            _graphics.Clear(Color.Red);
            picImage.Refresh();
        }
    }
}
