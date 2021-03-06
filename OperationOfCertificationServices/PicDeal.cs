﻿using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace OperationOfCertificationServices
{
    /// <summary>
    /// 枚举,生成缩略图模式
    /// </summary>
    public enum ThumbnailMod : byte
    {
        /// <summary>
        /// HW
        /// </summary>
        HW,
        /// <summary>
        /// W
        /// </summary>
        W,
        /// <summary>
        /// H
        /// </summary>
        H,
        /// <summary>
        /// Cut
        /// </summary>
        Cut
    };

    /// <summary>
    /// 操作图片类, 生成缩略图,添加水印
    /// </summary>
    public static class PicDeal
    {
        private static readonly Hashtable Htmimes = new Hashtable();
        internal static readonly string AllowExt = ".jpe|.jpeg|.jpg|.png|.tif|.tiff|.bmp";

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool MakeThumbnail(string originalImagePath, int width, int height, ThumbnailMod mode)
        {
            if (string.IsNullOrEmpty(originalImagePath))
            {
                throw new ArgumentNullException(originalImagePath);
            }
            string thumbnailPath = originalImagePath.Substring(0, originalImagePath.LastIndexOf('.')) + "s.jpg";
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = 100;
            int toheight = 100;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            if (ow > oh)
            {
                toheight = originalImage.Height * 100 / originalImage.Width;
            }
            else if (ow < oh)
            {
                towidth = originalImage.Width * 100 / originalImage.Height;
            }

            //switch (mode)
            //{
            //    case ThumbnailMod.HW://指定高宽缩放（可能变形）                
            //        break;
            //    case ThumbnailMod.W://指定宽，高按比例                    
            //        toheight = originalImage.Height * width / originalImage.Width;
            //        break;
            //    case ThumbnailMod.H://指定高，宽按比例
            //        towidth = originalImage.Width * height / originalImage.Height;
            //        break;
            //    case ThumbnailMod.Cut://指定高宽裁减（不变形）                
            //        if (originalImage.Width / (double)originalImage.Height > towidth / (double)toheight)
            //        {
            //            oh = originalImage.Height;
            //            ow = originalImage.Height * towidth / toheight;
            //            y = 0;
            //            x = (originalImage.Width - ow) / 2;
            //        }
            //        else
            //        {
            //            ow = originalImage.Width;
            //            oh = originalImage.Width * height / towidth;
            //            x = 0;
            //            y = (originalImage.Height - oh) / 2;
            //        }
            //        break;
            //}

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            bool isok = false;
            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                isok = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            return true;
        }

        ///// <summary>
        ///// 在图片上生成图片水印
        ///// </summary>
        ///// <param name="Path">原服务器图片路径</param>
        ///// <param name="Path_syp">生成的带图片水印的图片路径</param>
        ///// <param name="Path_sypf">水印图片路径</param>
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        public static void AddWaterPic(string Path, string Path_sypf)
        {
            try
            {
                Image image = Image.FromFile(Path);
                Image copyImage = Image.FromFile(Path_sypf);
                Graphics g = Graphics.FromImage(image);
                g.DrawImage(copyImage,
                    new Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height,
                        copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height,
                    GraphicsUnit.Pixel);
                g.Dispose();

                image.Save(Path + ".temp");
                image.Dispose();
                System.IO.File.Delete(Path);
                System.IO.File.Move(Path + ".temp", Path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 公共方法
        /// </summary>
        private static void GetImgType()
        {
            Htmimes[".jpe"] = "image/jpeg";
            Htmimes[".jpeg"] = "image/jpeg";
            Htmimes[".jpg"] = "image/jpeg";
            Htmimes[".png"] = "image/png";
            Htmimes[".tif"] = "image/tiff";
            Htmimes[".tiff"] = "image/tiff";
            Htmimes[".bmp"] = "image/bmp";
        }


        /// <summary>
        /// 返回新图片尺寸
        /// </summary>
        /// <param name="width">原始宽</param>
        /// <param name="height">原始高</param>
        /// <param name="maxWidth">新图片最大宽</param>
        /// <param name="maxHeight">新图片最大高</param>
        /// <returns></returns>
        public static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;

            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            return new Size(newWidth, newHeight);

        }
    }
}
