using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FluentAssertions;

namespace DockerS3DynamoDBTest
{
    public class UnitTest1
    {
        private DynamoDbManager _dynamoDbManager;
        private S3Manager _s3Manager;
        private string _tableName = "Test1";

        public UnitTest1 () {
            _dynamoDbManager = new DynamoDbManager("fake", "fake", 
                Amazon.RegionEndpoint.EUWest1, "http://localhost:8000");

            _s3Manager = new S3Manager("rootname", "accesskey", 
                Amazon.RegionEndpoint.EUWest1, "http://localhost:9000");
        }

        [Fact]
        public async void DynamoDBTest()
        {
            await _dynamoDbManager.CreateTableIfNotExists(_tableName);
            var primaryKey = "PK1";
            var data = "{}";

            var requestPut = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>{
                    { "primaryKey", new AttributeValue { S = primaryKey } },
                    { "data", new AttributeValue { S = data } }
                    }
            };

            var responsePut = await _dynamoDbManager.Client.PutItemAsync(requestPut);
            responsePut.Should().NotBeNull();

            var requestGet = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> {
                    { "primaryKey", new AttributeValue { S = primaryKey } }
                }       
            };

            var responseGet = await _dynamoDbManager.Client.GetItemAsync(requestGet);
            responseGet.Should().NotBeNull();
        }

        [Fact]
        public async void S3Test()
        {
            var bucketName = "mybucket";
            await _s3Manager.CreateBucketIfNotExists(bucketName);
            var result =await _s3Manager.UploadStringAsync(bucketName, "file1", "{}");
            result.Should().NotBeNull();
            result.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}