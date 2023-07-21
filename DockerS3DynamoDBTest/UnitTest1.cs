using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FluentAssertions;

namespace DockerS3DynamoDBTest
{
    public class UnitTest1
    {
        private DynamoDbManager _dynamoDbManager;
        private string _tableName = "Test1";

        public UnitTest1 () {
            _dynamoDbManager = new DynamoDbManager("fake", "fake", 
                Amazon.RegionEndpoint.EUWest1, "http://localhost:8000");
            
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
    }
}