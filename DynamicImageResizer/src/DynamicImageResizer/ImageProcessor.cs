using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;

namespace DynamicImageResizer
{
    public class ImageProcessor
    {
        public static Image ResizeImage(Image image, int width, int height)
        {
            return new Bitmap(image, new Size(width, height));
        }

        public static string hash(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                memoryStream.Position = 0;
                using (var sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
                {
                    return BitConverter.ToString(sha1CryptoServiceProvider.ComputeHash(memoryStream))
                        .Replace("-", "")
                        .ToLower();
                }
            }
        }

        public static string ToBase64String(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}
