using System;
using System.Drawing;
using System.IO;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace DynamicImageResizer
{
    public class ImageStorage
    {
        private readonly IAmazonS3 _amazonS3;

        public ImageStorage()
        {
            _amazonS3 = new AmazonS3Client(RegionEndpoint.EUWest1);
        }

        public Image Get(string bucketName, string keyPrefix, string imageName)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyPrefix + imageName
            };
            var task = _amazonS3.GetObjectAsync(getObjectRequest);
            var getObjectResponse = task.Result;
            using (var stream = getObjectResponse.ResponseStream)
            {
                return Image.FromStream(stream);
            }
        }

        public bool Put(string bucketName, string keyPrefix, string imageName, Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                memoryStream.Position = 0;
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyPrefix + imageName,
                    ContentType = "image/png",
                    InputStream = memoryStream
                };
                var task = _amazonS3.PutObjectAsync(putObjectRequest);
                var putObjectResponse = task.Result;
                return putObjectResponse.HttpStatusCode.Equals(HttpStatusCode.OK);
            }
        }

        public bool Exists(string bucketName, string keyPrefix, string imageName)
        {
            var getObjectMetadataRequest = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = keyPrefix + imageName
            };
            try
            {
                var task = _amazonS3.GetObjectMetadataAsync(getObjectMetadataRequest);
                task.Wait();
            }
            catch (AggregateException e)
            {
                if (e.GetBaseException() is AmazonS3Exception)
                {
                    var amazonS3Exception = (AmazonS3Exception) e.GetBaseException();
                    if (amazonS3Exception.StatusCode.Equals(HttpStatusCode.NotFound)) return false;
                }
                throw;
            }
            return true;
        }
    }
}
