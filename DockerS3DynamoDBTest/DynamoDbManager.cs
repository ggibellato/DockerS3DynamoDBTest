using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;

namespace DockerS3DynamoDBTest
{
    internal class DynamoDbManager
    {
        public AmazonDynamoDBClient Client => _client;

        private readonly AmazonDynamoDBClient _client;

        public DynamoDbManager(string accessKeyId, string secretAccessKey, Amazon.RegionEndpoint region, string serviceUrl)
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = region
            };
            if(!string.IsNullOrWhiteSpace(serviceUrl))
            {
                config.ServiceURL = "http://localhost:8000";
            }
            _client = new AmazonDynamoDBClient(accessKeyId, secretAccessKey, config);
        }

        public async Task CreateTableIfNotExists(string tableName)
        {
            if (!await TableExists(tableName))
            {
                await CreateNewTable(tableName);
            }
            else
            {
                Console.WriteLine($"Table '{tableName}' already exists. Skipping table creation.");
            }
        }

        private async Task<bool> TableExists(string tableName)
        {
            var response = await _client.ListTablesAsync();
            return response.TableNames.Contains(tableName);
        }

        private async Task CreateNewTable(string tableName)
        {
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "primaryKey", // Replace with your primary key attribute name
                    AttributeType = ScalarAttributeType.S // Assuming it's a string attribute, you can change this as needed
                }
                // You can add more attribute definitions for other attributes.
            },
                KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "primaryKey", // Replace with your primary key attribute name
                    KeyType = KeyType.HASH
                }
                // You can add more key schema elements for other attributes as needed.
            },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5, // Adjust these values as needed based on your requirements
                    WriteCapacityUnits = 5
                }
            };

            var response = await _client.CreateTableAsync(request);
            Console.WriteLine($"Table '{tableName}' has been created. Status: {response.TableDescription.TableStatus}");
        }
    }

}
