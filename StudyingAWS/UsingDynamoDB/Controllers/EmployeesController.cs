using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsingDynamoDB.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private int id = 1;
    private readonly ILogger<StorageController> _logger;
    private readonly AmazonDynamoDBClient _client;

    public EmployeesController(ILogger<StorageController> logger, AmazonDynamoDBClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        var request = new PutItemRequest()
        {
            TableName = "Employees",
            Item = new Dictionary<string, AttributeValue>()
            {
                { "Id", new AttributeValue {
                    N = id.ToString()
                }},
                { "Name", new AttributeValue {
                    S = employee.Name
                }}
            }
        };

        var response = await _client.PutItemAsync(request);

        id++;
        
        return Ok(new { id = id});
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        
        return Ok();
    }
    
    [HttpGet("{key:int}")]
    public async Task<IActionResult> GetEmployee(int key)
    {
        
        return Ok();
    }
}