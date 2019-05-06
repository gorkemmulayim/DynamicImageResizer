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
        private static readonly string S3BucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        private static readonly string S3CacheKeyPrefix = Environment.GetEnvironmentVariable("S3_CACHE_KEY_PREFIX");
        private static readonly string S3ImageKeyPrefix = Environment.GetEnvironmentVariable("S3_IMAGE_KEY_PREFIX");
        private readonly ImageStorage _imageStorage = new ImageStorage();

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            var imageName = apiGatewayProxyRequest.QueryStringParameters["image-name"];
            var width = apiGatewayProxyRequest.QueryStringParameters["width"];
            var height = apiGatewayProxyRequest.QueryStringParameters["height"];
            var image = _imageStorage.Get(S3BucketName, S3ImageKeyPrefix, imageName);
            var hash = ImageProcessor.hash(image);
            var cachedImageName = hash + "-" + width + "-" + height + ".png";
            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "image/png");
            headers.Add("Content-Disposition", "attachment; filename=\"" + cachedImageName + "\"");
            if (_imageStorage.Exists(S3BucketName, S3CacheKeyPrefix, cachedImageName))
            {
                var cachedImage = _imageStorage.Get(S3BucketName, S3CacheKeyPrefix, cachedImageName);
                return new APIGatewayProxyResponse
                {
                    Headers = headers,
                    Body = ImageProcessor.ToBase64String(cachedImage),
                    StatusCode = 200,
                    IsBase64Encoded = true
                };
            }

            var resizedImage = ImageProcessor.ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
            _imageStorage.Put(S3BucketName, S3CacheKeyPrefix, cachedImageName, resizedImage);
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
