using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagingSolution
{
    /// <summary>
    /// ImagingDotNetクラス
    /// </summary>
    public static partial class ImagingDotNet
    {
        // enum ColorConversionCodes相当の定数
        public const int COLOR_BGR2BGRA = 0; //!< add alpha channel to RGB or BGR image
        public const int COLOR_RGB2RGBA = COLOR_BGR2BGRA;

        public const int COLOR_BGRA2BGR = 1; //!< remove alpha channel from RGB or BGR image
        public const int COLOR_RGBA2RGB = COLOR_BGRA2BGR;

        public const int COLOR_BGR2RGBA = 2; //!< convert between RGB and BGR color spaces (with or without alpha channel)
        public const int COLOR_RGB2BGRA = COLOR_BGR2RGBA;

        public const int COLOR_RGBA2BGR = 3;
        public const int COLOR_BGRA2RGB = COLOR_RGBA2BGR;

        public const int COLOR_BGR2RGB = 4;
        public const int COLOR_RGB2BGR = COLOR_BGR2RGB;

        public const int COLOR_BGRA2RGBA = 5;
        public const int COLOR_RGBA2BGRA = COLOR_BGRA2RGBA;

        public const int COLOR_BGR2GRAY = 6; //!< convert between RGB/BGR and grayscale; @ref color_convert_rgb_gray "color conversions"
        public const int COLOR_RGB2GRAY = 7;
        public const int COLOR_GRAY2BGR = 8;
        public const int COLOR_GRAY2RGB = COLOR_GRAY2BGR;
        public const int COLOR_GRAY2BGRA = 9;
        public const int COLOR_GRAY2RGBA = COLOR_GRAY2BGRA;
        public const int COLOR_BGRA2GRAY = 10;
        public const int COLOR_RGBA2GRAY = 11;

        public const int COLOR_BGR2BGR565 = 12; //!< convert between RGB/BGR and BGR565 (16-bit images)
        public const int COLOR_RGB2BGR565 = 13;
        public const int COLOR_BGR5652BGR = 14;
        public const int COLOR_BGR5652RGB = 15;
        public const int COLOR_BGRA2BGR565 = 16;
        public const int COLOR_RGBA2BGR565 = 17;
        public const int COLOR_BGR5652BGRA = 18;
        public const int COLOR_BGR5652RGBA = 19;

        public const int COLOR_GRAY2BGR565 = 20; //!< convert between grayscale to BGR565 (16-bit images)
        public const int COLOR_BGR5652GRAY = 21;

        public const int COLOR_BGR2BGR555 = 22;  //!< convert between RGB/BGR and BGR555 (16-bit images)
        public const int COLOR_RGB2BGR555 = 23;
        public const int COLOR_BGR5552BGR = 24;
        public const int COLOR_BGR5552RGB = 25;
        public const int COLOR_BGRA2BGR555 = 26;
        public const int COLOR_RGBA2BGR555 = 27;
        public const int COLOR_BGR5552BGRA = 28;
        public const int COLOR_BGR5552RGBA = 29;

        public const int COLOR_GRAY2BGR555 = 30; //!< convert between grayscale and BGR555 (16-bit images)
        public const int COLOR_BGR5552GRAY = 31;

        public const int COLOR_BGR2XYZ = 32; //!< convert RGB/BGR to CIE XYZ; @ref color_convert_rgb_xyz "color conversions"
        public const int COLOR_RGB2XYZ = 33;
        public const int COLOR_XYZ2BGR = 34;
        public const int COLOR_XYZ2RGB = 35;

        public const int COLOR_BGR2YCrCb = 36; //!< convert RGB/BGR to luma-chroma (aka YCC); @ref color_convert_rgb_ycrcb "color conversions"
        public const int COLOR_RGB2YCrCb = 37;
        public const int COLOR_YCrCb2BGR = 38;
        public const int COLOR_YCrCb2RGB = 39;

        public const int COLOR_BGR2HSV = 40; //!< convert RGB/BGR to HSV (hue saturation value); @ref color_convert_rgb_hsv "color conversions"
        public const int COLOR_RGB2HSV = 41;

        public const int COLOR_BGR2Lab = 44; //!< convert RGB/BGR to CIE Lab; @ref color_convert_rgb_lab "color conversions"
        public const int COLOR_RGB2Lab = 45;

        public const int COLOR_BGR2Luv = 50; //!< convert RGB/BGR to CIE Luv; @ref color_convert_rgb_luv "color conversions"
        public const int COLOR_RGB2Luv = 51;
        public const int COLOR_BGR2HLS = 52; //!< convert RGB/BGR to HLS (hue lightness saturation); @ref color_convert_rgb_hls "color conversions"
        public const int COLOR_RGB2HLS = 53;

        public const int COLOR_HSV2BGR = 54; //!< backward conversions to RGB/BGR
        public const int COLOR_HSV2RGB = 55;

        public const int COLOR_Lab2BGR = 56;
        public const int COLOR_Lab2RGB = 57;
        public const int COLOR_Luv2BGR = 58;
        public const int COLOR_Luv2RGB = 59;
        public const int COLOR_HLS2BGR = 60;
        public const int COLOR_HLS2RGB = 61;

        public const int COLOR_BGR2HSV_FULL = 66;
        public const int COLOR_RGB2HSV_FULL = 67;
        public const int COLOR_BGR2HLS_FULL = 68;
        public const int COLOR_RGB2HLS_FULL = 69;

        public const int COLOR_HSV2BGR_FULL = 70;
        public const int COLOR_HSV2RGB_FULL = 71;
        public const int COLOR_HLS2BGR_FULL = 72;
        public const int COLOR_HLS2RGB_FULL = 73;

        public const int COLOR_LBGR2Lab = 74;
        public const int COLOR_LRGB2Lab = 75;
        public const int COLOR_LBGR2Luv = 76;
        public const int COLOR_LRGB2Luv = 77;

        public const int COLOR_Lab2LBGR = 78;
        public const int COLOR_Lab2LRGB = 79;
        public const int COLOR_Luv2LBGR = 80;
        public const int COLOR_Luv2LRGB = 81;

        public const int COLOR_BGR2YUV = 82; //!< convert between RGB/BGR and YUV
        public const int COLOR_RGB2YUV = 83;
        public const int COLOR_YUV2BGR = 84;
        public const int COLOR_YUV2RGB = 85;

        //! YUV 4:2:0 family to RGB
        public const int COLOR_YUV2RGB_NV12 = 90;
        public const int COLOR_YUV2BGR_NV12 = 91;
        public const int COLOR_YUV2RGB_NV21 = 92;
        public const int COLOR_YUV2BGR_NV21 = 93;
        public const int COLOR_YUV420sp2RGB = COLOR_YUV2RGB_NV21;
        public const int COLOR_YUV420sp2BGR = COLOR_YUV2BGR_NV21;

        public const int COLOR_YUV2RGBA_NV12 = 94;
        public const int COLOR_YUV2BGRA_NV12 = 95;
        public const int COLOR_YUV2RGBA_NV21 = 96;
        public const int COLOR_YUV2BGRA_NV21 = 97;
        public const int COLOR_YUV420sp2RGBA = COLOR_YUV2RGBA_NV21;
        public const int COLOR_YUV420sp2BGRA = COLOR_YUV2BGRA_NV21;

        public const int COLOR_YUV2RGB_YV12 = 98;
        public const int COLOR_YUV2BGR_YV12 = 99;
        public const int COLOR_YUV2RGB_IYUV = 100;
        public const int COLOR_YUV2BGR_IYUV = 101;
        public const int COLOR_YUV2RGB_I420 = COLOR_YUV2RGB_IYUV;
        public const int COLOR_YUV2BGR_I420 = COLOR_YUV2BGR_IYUV;
        public const int COLOR_YUV420p2RGB = COLOR_YUV2RGB_YV12;
        public const int COLOR_YUV420p2BGR = COLOR_YUV2BGR_YV12;

        public const int COLOR_YUV2RGBA_YV12 = 102;
        public const int COLOR_YUV2BGRA_YV12 = 103;
        public const int COLOR_YUV2RGBA_IYUV = 104;
        public const int COLOR_YUV2BGRA_IYUV = 105;
        public const int COLOR_YUV2RGBA_I420 = COLOR_YUV2RGBA_IYUV;
        public const int COLOR_YUV2BGRA_I420 = COLOR_YUV2BGRA_IYUV;
        public const int COLOR_YUV420p2RGBA = COLOR_YUV2RGBA_YV12;
        public const int COLOR_YUV420p2BGRA = COLOR_YUV2BGRA_YV12;

        public const int COLOR_YUV2GRAY_420 = 106;
        public const int COLOR_YUV2GRAY_NV21 = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV2GRAY_NV12 = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV2GRAY_YV12 = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV2GRAY_IYUV = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV2GRAY_I420 = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV420sp2GRAY = COLOR_YUV2GRAY_420;
        public const int COLOR_YUV420p2GRAY = COLOR_YUV2GRAY_420;

        //! YUV 4:2:2 family to RGB
        public const int COLOR_YUV2RGB_UYVY = 107;
        public const int COLOR_YUV2BGR_UYVY = 108;
        //COLOR_YUV2RGB_VYUY = 109;
        //COLOR_YUV2BGR_VYUY = 110;
        public const int COLOR_YUV2RGB_Y422 = COLOR_YUV2RGB_UYVY;
        public const int COLOR_YUV2BGR_Y422 = COLOR_YUV2BGR_UYVY;
        public const int COLOR_YUV2RGB_UYNV = COLOR_YUV2RGB_UYVY;
        public const int COLOR_YUV2BGR_UYNV = COLOR_YUV2BGR_UYVY;

        public const int COLOR_YUV2RGBA_UYVY = 111;
        public const int COLOR_YUV2BGRA_UYVY = 112;
        //COLOR_YUV2RGBA_VYUY = 113;
        //COLOR_YUV2BGRA_VYUY = 114;
        public const int COLOR_YUV2RGBA_Y422 = COLOR_YUV2RGBA_UYVY;
        public const int COLOR_YUV2BGRA_Y422 = COLOR_YUV2BGRA_UYVY;
        public const int COLOR_YUV2RGBA_UYNV = COLOR_YUV2RGBA_UYVY;
        public const int COLOR_YUV2BGRA_UYNV = COLOR_YUV2BGRA_UYVY;

        public const int COLOR_YUV2RGB_YUY2 = 115;
        public const int COLOR_YUV2BGR_YUY2 = 116;
        public const int COLOR_YUV2RGB_YVYU = 117;
        public const int COLOR_YUV2BGR_YVYU = 118;
        public const int COLOR_YUV2RGB_YUYV = COLOR_YUV2RGB_YUY2;
        public const int COLOR_YUV2BGR_YUYV = COLOR_YUV2BGR_YUY2;
        public const int COLOR_YUV2RGB_YUNV = COLOR_YUV2RGB_YUY2;
        public const int COLOR_YUV2BGR_YUNV = COLOR_YUV2BGR_YUY2;

        public const int COLOR_YUV2RGBA_YUY2 = 119;
        public const int COLOR_YUV2BGRA_YUY2 = 120;
        public const int COLOR_YUV2RGBA_YVYU = 121;
        public const int COLOR_YUV2BGRA_YVYU = 122;
        public const int COLOR_YUV2RGBA_YUYV = COLOR_YUV2RGBA_YUY2;
        public const int COLOR_YUV2BGRA_YUYV = COLOR_YUV2BGRA_YUY2;
        public const int COLOR_YUV2RGBA_YUNV = COLOR_YUV2RGBA_YUY2;
        public const int COLOR_YUV2BGRA_YUNV = COLOR_YUV2BGRA_YUY2;

        public const int COLOR_YUV2GRAY_UYVY = 123;
        public const int COLOR_YUV2GRAY_YUY2 = 124;
        //CV_YUV2GRAY_VYUY    = CV_YUV2GRAY_UYVY;
        public const int COLOR_YUV2GRAY_Y422 = COLOR_YUV2GRAY_UYVY;
        public const int COLOR_YUV2GRAY_UYNV = COLOR_YUV2GRAY_UYVY;
        public const int COLOR_YUV2GRAY_YVYU = COLOR_YUV2GRAY_YUY2;
        public const int COLOR_YUV2GRAY_YUYV = COLOR_YUV2GRAY_YUY2;
        public const int COLOR_YUV2GRAY_YUNV = COLOR_YUV2GRAY_YUY2;

        //! alpha premultiplication
        public const int COLOR_RGBA2mRGBA = 125;
        public const int COLOR_mRGBA2RGBA = 126;

        //! RGB to YUV 4:2:0 family
        public const int COLOR_RGB2YUV_I420 = 127;
        public const int COLOR_BGR2YUV_I420 = 128;
        public const int COLOR_RGB2YUV_IYUV = COLOR_RGB2YUV_I420;
        public const int COLOR_BGR2YUV_IYUV = COLOR_BGR2YUV_I420;

        public const int COLOR_RGBA2YUV_I420 = 129;
        public const int COLOR_BGRA2YUV_I420 = 130;
        public const int COLOR_RGBA2YUV_IYUV = COLOR_RGBA2YUV_I420;
        public const int COLOR_BGRA2YUV_IYUV = COLOR_BGRA2YUV_I420;
        public const int COLOR_RGB2YUV_YV12 = 131;
        public const int COLOR_BGR2YUV_YV12 = 132;
        public const int COLOR_RGBA2YUV_YV12 = 133;
        public const int COLOR_BGRA2YUV_YV12 = 134;

        //! Demosaicing
        public const int COLOR_BayerBG2BGR = 46;
        public const int COLOR_BayerGB2BGR = 47;
        public const int COLOR_BayerRG2BGR = 48;
        public const int COLOR_BayerGR2BGR = 49;

        public const int COLOR_BayerBG2RGB = COLOR_BayerRG2BGR;
        public const int COLOR_BayerGB2RGB = COLOR_BayerGR2BGR;
        public const int COLOR_BayerRG2RGB = COLOR_BayerBG2BGR;
        public const int COLOR_BayerGR2RGB = COLOR_BayerGB2BGR;

        public const int COLOR_BayerBG2GRAY = 86;
        public const int COLOR_BayerGB2GRAY = 87;
        public const int COLOR_BayerRG2GRAY = 88;
        public const int COLOR_BayerGR2GRAY = 89;

        //! Demosaicing using Variable Number of Gradients
        public const int COLOR_BayerBG2BGR_VNG = 62;
        public const int COLOR_BayerGB2BGR_VNG = 63;
        public const int COLOR_BayerRG2BGR_VNG = 64;
        public const int COLOR_BayerGR2BGR_VNG = 65;

        public const int COLOR_BayerBG2RGB_VNG = COLOR_BayerRG2BGR_VNG;
        public const int COLOR_BayerGB2RGB_VNG = COLOR_BayerGR2BGR_VNG;
        public const int COLOR_BayerRG2RGB_VNG = COLOR_BayerBG2BGR_VNG;
        public const int COLOR_BayerGR2RGB_VNG = COLOR_BayerGB2BGR_VNG;

        //! Edge-Aware Demosaicing
        public const int COLOR_BayerBG2BGR_EA = 135;
        public const int COLOR_BayerGB2BGR_EA = 136;
        public const int COLOR_BayerRG2BGR_EA = 137;
        public const int COLOR_BayerGR2BGR_EA = 138;

        public const int COLOR_BayerBG2RGB_EA = COLOR_BayerRG2BGR_EA;
        public const int COLOR_BayerGB2RGB_EA = COLOR_BayerGR2BGR_EA;
        public const int COLOR_BayerRG2RGB_EA = COLOR_BayerBG2BGR_EA;
        public const int COLOR_BayerGR2RGB_EA = COLOR_BayerGB2BGR_EA;

        //! Demosaicing with alpha channel
        public const int COLOR_BayerBG2BGRA = 139;
        public const int COLOR_BayerGB2BGRA = 140;
        public const int COLOR_BayerRG2BGRA = 141;
        public const int COLOR_BayerGR2BGRA = 142;

        public const int COLOR_BayerBG2RGBA = COLOR_BayerRG2BGRA;
        public const int COLOR_BayerGB2RGBA = COLOR_BayerGR2BGRA;
        public const int COLOR_BayerRG2RGBA = COLOR_BayerBG2BGRA;
        public const int COLOR_BayerGR2RGBA = COLOR_BayerGB2BGRA;

        public const int COLOR_COLORCVT_MAX = 143;



    }
}
