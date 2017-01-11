using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;

namespace OperationOfCertificationServices
{
    public static class ImageHelp
    {
        /// <summary>
        /// 获得图片的类型
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static System.Drawing.Imaging.ImageFormat ImgFormat(string photo)
        {
            //获得图片的后缀,不带点，小写
            var imgExt = photo.Substring(photo.LastIndexOf(".", StringComparison.Ordinal) + 1, photo.Length - photo.LastIndexOf(".", StringComparison.Ordinal) - 1).ToLower();
            System.Drawing.Imaging.ImageFormat imgFormat;
            switch (imgExt)
            {
                case "png":
                    imgFormat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                case "gif":
                    imgFormat = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case "bmp":
                    imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                default:
                    imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
            }
            return imgFormat;
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式
        /// <code>HW:指定高宽缩放（可能变形）</code>
        /// <code>W:指定宽，高按比例  </code>
        /// <code>H:指定高，宽按比例</code>
        /// <code>CUT:指定高宽裁减（不变形） </code>
        /// <code>FILL:填充</code>
        /// </param>    
        public static bool LocalImage2Thumbs(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            var originalImage = Image.FromFile(originalImagePath);
            Image2Thumbs(originalImage, thumbnailPath, width, height, mode);
            originalImage.Dispose();
            return true;
        }

        /// <summary>
        /// 生成远程图片的缩略图
        /// </summary>
        /// <param name="remoteImageUrl"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool RemoteImage2Thumbs(string remoteImageUrl, string thumbnailPath, int width, int height, string mode)
        {
            try
            {
                var request = WebRequest.Create(remoteImageUrl);
                request.Timeout = 20000;
                var stream = request.GetResponse().GetResponseStream();
                if (stream == null) return true;
                var originalImage = Image.FromStream(stream);
                Image2Thumbs(originalImage, thumbnailPath, width, height, mode);
                originalImage.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImage">源图</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="photoWidth">最终缩略图宽度</param>
        /// <param name="height">最终缩略图高度</param>
        /// <param name="photoHeight"></param>
        /// <param name="mode">生成缩略图的方式
        /// <code>HW:指定高宽缩放（可能变形）</code>
        /// <code>W:指定宽，高按比例  </code>
        /// <code>H:指定高，宽按比例</code>
        /// <code>CUT:指定高宽裁减（不变形） </code>
        /// <code>FILL:填充</code>
        /// </param> 
        public static void Image2Thumbs(Image originalImage, string thumbnailPath, int photoWidth, int photoHeight, string mode)
        {
            //最后缩略图的宽度
            var lastPhotoWidth = photoWidth;
            //最后缩略图的高度
            var lastPhotoHeight = photoHeight;
            //原图片被压缩的宽度
            var toWidth = photoWidth;
            //原图片被压缩的高度
            var toHeight = photoHeight;
            var x = 0;
            var y = 0;
            var ow = originalImage.Width;
            var oh = originalImage.Height;
            var bgX = 0;
            var bgY = 0;
            switch (mode.ToUpper())
            {
                //压缩填充至指定区域
                case "FILL":
                    toHeight = photoHeight;
                    toWidth = toHeight * ow / oh;
                    if (toWidth > photoWidth)
                    {
                        toHeight = toHeight * photoWidth / toWidth;
                        toWidth = photoWidth;
                    }
                    bgX = (photoWidth - toWidth) / 2;
                    bgY = (photoHeight - toHeight) / 2;
                    break;
                //指定高宽缩放（可能变形）
                case "HW":
                    break;
                //指定宽，高按比例 
                case "W":
                    toHeight = lastPhotoHeight = originalImage.Height * photoWidth / originalImage.Width;
                    break;
                //指定高，宽按比例
                case "H":
                    toWidth = lastPhotoWidth = originalImage.Width * photoHeight / originalImage.Height;
                    break;
                //指定高宽裁减（不变形）
                case "CUT":
                    if (originalImage.Width / (double)originalImage.Height > lastPhotoWidth / (double)lastPhotoHeight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * lastPhotoWidth / lastPhotoHeight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * photoHeight / lastPhotoWidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
            }
            //新建一个bmp图片
            Image bitmap = new Bitmap(lastPhotoWidth, lastPhotoHeight);
            //新建一个画板
            var g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            //白色
            g.Clear(Color.White);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(bgX, bgY, toWidth, toHeight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(thumbnailPath, ImgFormat(thumbnailPath));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

                bitmap.Dispose();
                g.Dispose();
            }

        }


        /// <summary>
        /// 切割后生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="toW">缩略图最终宽度</param>
        /// <param name="toH">缩略图最终高度</param>
        /// <param name="X">X坐标（zoom为1时）</param>
        /// <param name="Y">Y坐标（zoom为1时）</param>
        /// <param name="W">选择区域宽（zoom为1时）</param>
        /// <param name="H">选择区域高（zoom为1时）</param>
        public static void MakeMyThumbs(string originalImagePath, string thumbnailPath, int toW, int toH, int X, int Y, int W, int H)
        {
            var originalImage = Image.FromFile(originalImagePath);
            var towidth = toW;
            var toheight = toH;
            var x = X;
            var y = Y;
            var ow = W;
            var oh = H;
            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);
            //新建一个画板
            var g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(thumbnailPath, ImgFormat(thumbnailPath));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }


        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="path">原服务器图片路径</param>
        /// <param name="pathSy">生成的带文字水印的图片路径</param>
        /// <param name="addText">水印文字</param>
        public static void AddWater(string path, string pathSy, string addText)
        {
            var image = Image.FromFile(path);
            var g = Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            var f = new Font("Verdana", 60);
            Brush b = new SolidBrush(Color.Green);
            g.DrawString(addText, f, b, 35, 35);
            g.Dispose();

            image.Save(pathSy);
            image.Dispose();
        }


        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkFilename">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置:0=不使用 1=左上 2=中上 3=右上 4=左中 ... 9=右下</param>
        /// <param name="quality">是否是高质量图片 取值范围0--100</param> 
        /// <param name="watermarkTransparency">图片水印透明度 取值范围1--10 (10为不透明)</param>
        public static void AddImageSignPic(string path, string filename, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            var img = Image.FromFile(path);
            var g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);
            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
            {
                return;
            }
            var imageAttributes = new System.Drawing.Imaging.ImageAttributes();
            var colorMap = new System.Drawing.Imaging.ColorMap
            {
                OldColor = Color.FromArgb(255, 0, 255, 0),
                NewColor = Color.FromArgb(0, 0, 0, 0)
            };

            System.Drawing.Imaging.ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, System.Drawing.Imaging.ColorAdjustType.Bitmap);

            var transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
            {
                transparency = (watermarkTransparency / 10.0F);
            }

            float[][] colorMatrixElements = {
                                                new[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            var colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            var xpos = 0;
            var ypos = 0;
            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, System.Drawing.GraphicsUnit.Pixel, imageAttributes);

            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            System.Drawing.Imaging.ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                //if (codec.MimeType.IndexOf("jpeg") > -1)
                if (codec.MimeType.Contains("jpeg"))
                {
                    ici = codec;
                }
            }
            var encoderParams = new System.Drawing.Imaging.EncoderParameters();
            var qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }
            qualityParam[0] = quality;

            var encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
            {
                img.Save(filename, ici, encoderParams);
            }
            else
            {
                img.Save(filename);
            }

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        public static void AddWaterPic(string Path, string Path_syp, string Path_sypf)
        {
            var image = Image.FromFile(Path);
            var copyImage = Image.FromFile(Path_sypf);
            var g = Graphics.FromImage(image);
            g.DrawImage(copyImage, new Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();

            image.Save(Path_syp);
            image.Dispose();
        }

    }
}
