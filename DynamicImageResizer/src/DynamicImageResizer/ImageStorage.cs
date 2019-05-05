using System.Drawing;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

namespace DynamicImageResizer
{
    public class ImageStorage
    {
        private readonly IAmazonS3 amazonS3;

        public ImageStorage()
        {
            amazonS3 = new AmazonS3Client();
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
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = "rootg-default",
                Key = "cache/" + name,
                ContentType = "image/jpeg"
            };
            var task = amazonS3.PutObjectAsync(putObjectRequest);
            var putObjectResponse = task.Result;
            return putObjectResponse.HttpStatusCode.Equals(HttpStatusCode.OK);
        }
    }
}