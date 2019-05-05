using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace DynamicImageResizer
{
    public class Function
    {
        private readonly ImageStorage _imageStorage = new ImageStorage();

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            var imageName = apiGatewayProxyRequest.QueryStringParameters["image-name"];
            var width = apiGatewayProxyRequest.QueryStringParameters["width"];
            var height = apiGatewayProxyRequest.QueryStringParameters["height"];
            Console.WriteLine("Name: " + imageName + ", " + "Width: " + width + ", " + "Height: " + height);
            var image = _imageStorage.Get(imageName);
            var resizedImage = ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
            _imageStorage.Put(imageName, resizedImage);
            using (var memoryStream = new MemoryStream())
            {
                var headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "image/png");
                headers.Add("Content-Disposition", "attachment; filename=\"" + imageName + "\".png");
                resizedImage.Save(memoryStream, resizedImage.RawFormat);
                var base64String = Convert.ToBase64String(memoryStream.ToArray());
                return new APIGatewayProxyResponse
                {
                    Headers = headers,
                    Body = base64String,
                    StatusCode = 200,
                    IsBase64Encoded = true
                };
            }
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            return new Bitmap(image, new Size(width, height));
        }
    }
}
