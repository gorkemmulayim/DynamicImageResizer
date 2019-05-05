using System;
using System.Collections.Generic;
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
            var image = _imageStorage.Get("images/", imageName);
            var hash = ImageProcessor.hash(image);
            var cachedImageName = hash + "-" + width + "-" + height + ".png";
            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "image/png");
            headers.Add("Content-Disposition", "attachment; filename=\"" + cachedImageName + "\"");
            if (_imageStorage.Exists("cache/", cachedImageName))
            {
                var cachedImage = _imageStorage.Get("cache/", cachedImageName);
                return new APIGatewayProxyResponse
                {
                    Headers = headers,
                    Body = ImageProcessor.ToBase64String(cachedImage),
                    StatusCode = 200,
                    IsBase64Encoded = true
                };
            }
            var resizedImage = ImageProcessor.ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
            _imageStorage.Put(cachedImageName, resizedImage);
            return new APIGatewayProxyResponse
            {
                Headers = headers,
                Body = ImageProcessor.ToBase64String(resizedImage),
                StatusCode = 200,
                IsBase64Encoded = true
            };
        }
    }
}
