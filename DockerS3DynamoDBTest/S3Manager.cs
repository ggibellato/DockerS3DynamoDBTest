using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace DockerS3DynamoDBTest
{
    public class S3Manager
    {
        public AmazonS3Client Client => _client;
        private readonly AmazonS3Client _client;

        public S3Manager(string accessKeyId, string secretAccessKey, Amazon.RegionEndpoint region, string serviceUrl)
        {
            AmazonS3Config config = new AmazonS3Config
            {
                RegionEndpoint = region,
                ForcePathStyle = true
            };
            if (!string.IsNullOrWhiteSpace(serviceUrl))
            {
                config.ServiceURL = serviceUrl;
            }
            _client = new AmazonS3Client(accessKeyId, secretAccessKey, config);
        }

        public async Task UploadFileAsync(string bucketName, string key, string filePath)
        {
            var fileTransferUtility = new TransferUtility(_client);

            await fileTransferUtility.UploadAsync(filePath, bucketName, key);
        }

        public async Task<PutObjectResponse> UploadStringAsync(string bucketName, string key, string content)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                ContentBody = content
            };

            return await _client.PutObjectAsync(request);
        }

        public async Task CreateBucketIfNotExists(string bucketName)
        {
            if (!await BucketExists(bucketName))
            {
                await CreateNewBucket(bucketName);
            }
            else
            {
                Console.WriteLine($"Bucket '{bucketName}' already exists. Skipping bucket creation.");
            }
        }

        private async Task<bool> BucketExists(string bucketName)
        {
            try
            {
                var response = await _client.ListBucketsAsync();
                return response.Buckets.Any(b => b.BucketName.Equals(bucketName));
            }
            catch (AmazonS3Exception)
            {
                // The AmazonS3Exception will be thrown if the bucket doesn't exist or if the user does not have access to list the buckets.
                return false;
            }
        }

        private async Task CreateNewBucket(string bucketName)
        {
            var request = new PutBucketRequest
            {
                BucketName = bucketName,
                BucketRegion = S3Region.EUWest1
            };

            await _client.PutBucketAsync(request);
            Console.WriteLine($"Bucket '{bucketName}' has been created.");
        }
    }
}
