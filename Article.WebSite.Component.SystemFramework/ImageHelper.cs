using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Article.WebSite.Component.SystemFramework.Models;
using Article.WebSite.Component.SystemFramework.Security;

namespace Article.WebSite.Component.SystemFramework
{
    public static class ImageHelper
    {
        #region GetImageUrl
        public static string GetImageUrl(this string url) => GetImageUrl(url, 0, 0, false);

        public static string GetMobileUrl(this string url) => GetImageUrl(url, 0, 0, true);

        public static string GetImageUrl(this string url, int maxWidthHeight) => GetImageUrl(url, maxWidthHeight, maxWidthHeight, false);

        public static string GetMobileUrl(this string url, int maxWidthHeight) => GetImageUrl(url, maxWidthHeight, maxWidthHeight, true);

        public static string GetImageUrl(this string url, int maxWidth, int maxHeight) => GetImageUrl(url, maxWidth, maxHeight, false);

        public static string GetMobileUrl(this string url, int maxWidth, int maxHeight) => GetImageUrl(url, maxWidth, maxHeight, true);

        /// <summary>处理图片地址</summary>
        /// <param name="url">图片地址。绝对地址或相对地址</param>
        /// <param name="maxWidth">最大宽</param>
        /// <param name="maxHeight">最大高</param>
        /// <param name="mobile">是否是用于手机显示</param>
        /// <returns>处理后的图片地址</returns>
        public static string GetImageUrl(this string url, int maxWidth, int maxHeight, bool mobile)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            string format;

            if (url.Contains("://"))
                try
                {
                    var uri = new Uri(url);

                    if (uri.Host.Contains(".qiniucdn.com"))
                    {
                        if (!string.IsNullOrWhiteSpace(uri.Query))
                            url = url.Substring(0, url.IndexOf('?'));

                        if (mobile)
                            format = "/format/webp";
                        else
                        {
                            format = GetImageFormat(uri.AbsolutePath);
                            if (format != null)
                                format = "/format/" + format;
                        }

                        if (maxWidth < 1)
                            return maxHeight < 1 ? $"{url}?imageView2/2{format}/q/100" : $"{url}?imageView2/2/h/{maxHeight}{format}/q/100";

                        return maxHeight < 1 ? $"{url}?imageView2/2/w/{maxWidth}{format}/q/100" : $"{url}?imageView2/2/w/{maxWidth}/h/{maxHeight}{format}/q/100";
                    }

                    if (!uri.Host.Contains(".tuhu.cn"))
                        return url;

                    url = uri.AbsolutePath;
                }
                catch
                {
                    return url;
                }
            else if (url[0] != '/')
                url = "/" + url;

            var host = $"https://img{BitConverter.ToUInt64(WebSecurity.Hash(Encoding.UTF8.GetBytes(url.ToLower())), 8) % 10}-tuhu-cn.alikunlun.com";

            if (mobile)
                format = ".webp";
            else
            {
                format = GetImageFormat(url);
                if (format != null)
                    format = "." + format;
            }

            if (maxWidth < 1)
                return maxHeight < 1 ? $"{host}{url}@100Q{format}" : $"{host}{url}@{maxHeight}h_100Q{format}";

            return maxHeight < 1 ? $"{host}{url}@{maxWidth}w_100Q{format}" : $"{host}{url}@{maxWidth}w_{maxHeight}h_100Q{format}";
        }

        /// <summary>将输入格式转换成标准输出格式</summary>
        /// <param name="url"></param>
        /// <returns>bmp gif psd tiff转换成png</returns>
        private static string GetImageFormat(string url)
        {
            var format = Path.GetExtension(url);
            if (format == null)
                return null;

            switch (format.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "jpg";
                case ".bmp":
                case ".gif":
                case ".png":
                case ".psd":
                case ".tiff":
                    return "png";
                default:
                    return null;
            }
        }
        #endregion

        public static string GetProductImage(string input) => GetImageUrl(input, 0, 0);

        public static string GetProductImage(string input, int height, int width) => GetImageUrl(input, width, height);

        public static string GetProductImage(string input, string productDefinition, int height, int width)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (productDefinition == "Wiper")
                    return GetImageUrl("/Images/Products/NopicWiper.jpg", height, width);
                else
                    return GetImageUrl("/Images/Products/NopicTires.jpg", height, width);
            }
            else
                return GetImageUrl(input, height, width);
        }

        public static string GetVehicleImage(string vehicleID, int width, int height) => GetProductImage("/Images/type/" + vehicleID + ".png", "Tires", width, height);

        public static string GetShopImage(string input, int width, int height) => GetImageUrl("/Images/Marketing/Shops/" + input, width, height);

        #region 图片处理方法
        /// <summary>缩小图片</summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>缩小后的图片</returns>
        public static Image Reduces(this Image rawImage, int width, int height)
        {
            return Reduces(rawImage, width, height, false);
        }

        /// <summary>缩小图片</summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="needFiller">是否需要补白，默认不补白</param>
        /// <returns>缩小后的图片</returns>
        public static Image Reduces(this Image rawImage, int width, int height, bool needFiller)
        {
            if (width < 1 || height < 1)
                throw new ArgumentException();

            var rawWidth = rawImage.Width;
            var rawHeight = rawImage.Height;
            var newWidth = width;
            var newHeight = height;
            if (rawWidth <= width && rawHeight <= height)
            {
                newWidth = rawWidth;
                newHeight = rawHeight;
            }
            else if (width / height > rawWidth / rawHeight)
                newWidth = rawWidth * height / rawHeight;
            else
                newHeight = rawHeight * width / rawWidth;

            int startX = 0;
            int startY = 0;
            Bitmap bitmap;
            if (needFiller)
            {
                startX = (width - newWidth) / 2;
                startY = (height - newHeight) / 2;
                bitmap = new Bitmap(width, height);
            }
            else
                bitmap = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                //插值算法的质量
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.Clear(Color.White);

                graphics.FillRectangle(Brushes.White, startX, startY, newWidth, newHeight);
                graphics.DrawImage(rawImage, startX, startY, newWidth, newHeight);

                foreach (var property in rawImage.PropertyItems)
                {
                    bitmap.SetPropertyItem(property);
                }

                return bitmap;
            }
        }

        public static byte[] SaveAsByteArray(this Image image)
        {
            return SaveAsByteArray(image, image.RawFormat);
        }
        public static byte[] SaveAsByteArray(this Image image, ImageFormat format)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToArray();
            }
        }

        /// <summary>自动旋转图片</summary>
        /// <param name="img">图片</param>
        /// <returns>是否旋转</returns>
        public static bool RotateFlip(this Image img)
        {
            var orientationProperty = img.PropertyItems.FirstOrDefault(p => p.Id == 274);
            if (orientationProperty != null)
            {
                var orientation = orientationProperty.Value[0];
                switch (orientation)
                {
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                if (orientation > 1)
                {
                    orientationProperty.Value[0] = 1;
                    img.SetPropertyItem(orientationProperty);

                    foreach (PropertyItem property in img.PropertyItems)
                    {
                        if (property.Id == 40962)
                        {
                            property.Value = BitConverter.GetBytes(img.Width);
                            img.SetPropertyItem(orientationProperty);
                        }
                        else if (property.Id == 40963)
                        {
                            property.Value = BitConverter.GetBytes(img.Height);
                            img.SetPropertyItem(orientationProperty);
                        }
                    }

                    return true;
                }
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 获取Logo图片地址(汉字转化为拼音)
        /// </summary>
        /// <param name="brand">品牌名称</param>
        /// <returns>Url</returns>
        public static string GetLogoUrlByName(string brand)
        {
            if (brand == null || brand.Length < 4)
                return null;

            return "/Images/Logo/" + HttpUtility.UrlEncode(Chs2py.Pinyin(brand.Substring(4).Trim()).ToLower()) + ".png";
        }

        /// <summary>
        /// 根据productID获取根据配置文件生成的图片路径
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static string GetShiPeiImage2(string productID) => GetImageUrl("/Images/Type/" + productID + ".png");

        /// <summary>
        /// 获取轮胎Logo图片地址(汉字转化为拼音)
        /// </summary>
        /// <param name="Pic_Nmae">品牌名称</param>
        /// <returns>Url</returns>
        public static string GetTirsLogoUrlByName(string Pic_Nmae)
        {
            string s = "/Images/TiresLogo/" + HttpUtility.UrlEncode(Chs2py.Pinyin(Pic_Nmae).ToLower().Split('/')[0] + ".png");
            return s;
        }

        /// <summary>
        /// 得到车辆品牌Logo图片地址
        /// </summary>
        /// <param name="Pic_Nmae">品牌名称</param>
        /// <returns>Url</returns>
        public static string GetShangBiao(string Pic_Nmae)
        {
            string a = Pic_Nmae.Replace(" ", "").Substring(Pic_Nmae.Replace(" ", "").IndexOf("-") + 1);
            string aa = null;
            if (a != null)
            {
                aa = Chs2py.Pinyin(a).ToLower();
            }
            string s = "/Images/Logo/" + HttpUtility.UrlEncode(aa + ".png");
            return s;
        }
    }
}
