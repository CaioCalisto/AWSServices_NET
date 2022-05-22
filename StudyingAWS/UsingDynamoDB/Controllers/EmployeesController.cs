using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;

namespace UsingDynamoDB.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private int id = 1;
    private readonly ILogger<StorageController> _logger;
    private readonly IAmazonDynamoDB _dynamoDb;

    public EmployeesController(ILogger<StorageController> logger, IAmazonDynamoDB dynamoDb)
    {
        _logger = logger;
        _dynamoDb = dynamoDb;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        using (var context = new DynamoDBContext(_dynamoDb))
        {
            EmployeeDocument document = new EmployeeDocument()
            {
                Id = id,
                Name = employee.Name,
                Type = "Employee",
                Team = employee.Team,
                Role = employee.Role
            };
            await context.SaveAsync(document);
        }

        id++;
        
        return Ok();
    }

    [HttpGet("{key:int}")]
    public async Task<IActionResult> GetEmployee(int key)
    {
        var table = Table.LoadTable(_dynamoDb, "Employees");
        var item = await table.GetItemAsync(key, "Employee");
        Employee employee = new Employee(item["Name"], item["Role"], item["Team"]);

        return Ok(employee);
    }
}