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
        int employeeId = id;
        var request = new PutItemRequest()
        {
            TableName = "Employees",
            Item = new Dictionary<string, AttributeValue>()
            {
                { "Id", new AttributeValue {
                    N = employeeId.ToString()
                }},
                { "EmployeeName", new AttributeValue {
                    S = employee.Name
                }},
                { "EmployeeRole", new AttributeValue {
                    S = employee.Role
                }},
                { "EmployeeTeam", new AttributeValue {
                    S = employee.Team
                }}
            }
        };

        var response = await _client.PutItemAsync(request);

        id++;
        
        return Ok(new { Status = response.HttpStatusCode.ToString(), id = employeeId});
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetEmployee(string key)
    {
        var request = new GetItemRequest
        {
            TableName = "Employees",
            Key = new Dictionary<string, AttributeValue>()
            {
                { "Id", new AttributeValue {
                    N = key
                } }
            },
            ProjectionExpression = "Id, EmployeeName, EmployeeRole, EmployeeTeam",
            ConsistentRead = true
        };
        var response = await _client.GetItemAsync(request);
        
        return Ok(DeserializeToEmployee(response.Item));
    }

    private Employee DeserializeToEmployee(Dictionary<string, AttributeValue> attributeValues) =>
        new Employee(
            attributeValues["EmployeeName"].S, 
            attributeValues["EmployeeRole"].S, 
            attributeValues["EmployeeTeam"].S);
}