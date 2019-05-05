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
        private readonly IAmazonS3 amazonS3;

        public ImageStorage()
        {
            amazonS3 = new AmazonS3Client(RegionEndpoint.EUWest1);
        }

        public Image Get(string name)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = "rootg-default",
                Key = "images/" + name
            };
            var task = amazonS3.GetObjectAsync(getObjectRequest);
            var getObjectResponse = task.Result;
            using (var stream = getObjectResponse.ResponseStream)
            {
                return Image.FromStream(stream);
            }
        }

        public bool Put(string name, Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                memoryStream.Position = 0;
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = "rootg-default",
                    Key = "cache/" + name,
                    ContentType = "image/png",
                    InputStream = memoryStream
                };
                var task = amazonS3.PutObjectAsync(putObjectRequest);
                var putObjectResponse = task.Result;
                return putObjectResponse.HttpStatusCode.Equals(HttpStatusCode.OK);
            }
        }
    }
}