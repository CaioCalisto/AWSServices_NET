using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;

namespace UsingDynamoDB.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AmazonDynamoDBClient _client;
    
    public EmployeesController(AmazonDynamoDBClient client)
    {
        _client = client;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        using (var context = GetContext())
        {
            await context.SaveAsync(new EmployeeDocument()
            {
                Id = employee.Id,
                Name = employee.Name,
                Role = employee.Role,
                Team = employee.Team
            });
        }

        return Ok(new { id = employee.Id});
    }

    [HttpGet("query")]
    public async Task<IActionResult> GetAllEmployee([FromQuery]string? paginationToken = null)
    {
        using (var context = GetContext())
        {
            var table = context.GetTargetTable<EmployeeDocument>();
            var scanOps = new ScanOperationConfig()
            {
                Limit = 2
            };

            if (!string.IsNullOrEmpty(paginationToken)) 
                scanOps.PaginationToken = paginationToken;

            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();
            IEnumerable<EmployeeDocument> employees = context.FromDocuments<EmployeeDocument>(data);

            return Ok(new EmployeeViewModel() {
                PaginationToken = results.PaginationToken,
                Employees = MapToEmployees(employees),
            });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(string id)
    {
        using (var context = GetContext())
        {
            var result = await context.LoadAsync<EmployeeDocument>(id);
            return Ok(result);
        }
    }

    private IEnumerable<Employee> MapToEmployees(IEnumerable<EmployeeDocument> employees)
    {
        foreach (var employee in employees)
        {
            yield return new Employee(employee.Id, employee.Name, employee.Role, employee.Team);
        }
    }

    private IDynamoDBContext GetContext() => new DynamoDBContext(_client);
}