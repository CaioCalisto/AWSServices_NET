using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsingDynamoDB.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageController : ControllerBase
{
    private readonly IAmazonDynamoDB _amazonDynamoDb;
    public StorageController(IAmazonDynamoDB amazonDynamoDb)
    {
        _amazonDynamoDb = amazonDynamoDb;
    }
    
    [HttpGet("tables")]
    public async Task<IActionResult> GetAllTables()
    {
        var tables = await _amazonDynamoDb.ListTablesAsync();
        return Ok(tables.TableNames);
    }

    [HttpPost("tables")]
    public async Task<IActionResult> CreateTable()
    {
        var tables = await _amazonDynamoDb.ListTablesAsync();
        if (!tables.TableNames.Contains("Employees"))
        {
            var request = new CreateTableRequest()
            {
                TableName = "Employees",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        // "S" = string, "N" = number, and so on.
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        // "HASH" = hash key, "RANGE" = range key.
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                },
            };

            var response = await this._amazonDynamoDb.CreateTableAsync(request);
            return Ok(new { DynamoDbStatusCode = response.HttpStatusCode});
        }
        
        return BadRequest(new { Detail = "Table already exists!"});
    }
}