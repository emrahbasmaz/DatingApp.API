using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Threading.Tasks;

namespace DatingApp.Api.Extension
{
    public static class AmazonUploader
    {
        public static class AmazonS3Uploader
        {
            public static string bucketName = "barstechcloudtestbucket2019";
            public static string keyName = "the-name-of-your-file";
            public static string filePath = "C:\\Users\\yourUserName\\Desktop\\myImageToUpload.jpg";

            //public AmazonS3Uploader(string filePath, string keyName)
            //{
            //    this.filePath = filePath;
            //    this.keyName = keyName;
            //}
            public static async Task<PutObjectResponse> UploadFileAsync()
            {
                var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

                try
                {
                    PutObjectRequest putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                        FilePath = filePath,
                        ContentType = "text/plain"
                    };

                    PutObjectResponse response = await client.PutObjectAsync(putRequest);
                    if (response != null)
                    {
                        return response;
                    }
                    throw new Exception("Amazon S3 response Null");
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        throw new Exception("Check the provided AWS Credentials.");
                    }
                    else
                    {
                        throw new Exception("Error occurred: " + amazonS3Exception.Message);
                    }
                }
            }
        }
    }
}