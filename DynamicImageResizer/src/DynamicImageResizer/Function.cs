using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace DynamicImageResizer
{
    public class Function
    {
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent)
        {
            var name = apigProxyEvent.QueryStringParameters["name"];
            var width = apigProxyEvent.QueryStringParameters["width"];
            var height = apigProxyEvent.QueryStringParameters["height"];
            Console.WriteLine("Name: " + name + ", " + "Width: " + width + ", " + "Height: " + height);
            return new APIGatewayProxyResponse
            {
                Body = "Hello World!",
                StatusCode = 200
            };
        }

        public static void Main(string[] args)
        {
            var imageStorage = new ImageStorage();
            var image = imageStorage.Get("lenna.jpeg");
            var resizedImage = ResizeImage(image, 450, 450);
            imageStorage.Put("lenna.jpeg", resizedImage);
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            var rectangle = new Rectangle(0, 0, width, height);
            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return bitmap;
        }
    }
}