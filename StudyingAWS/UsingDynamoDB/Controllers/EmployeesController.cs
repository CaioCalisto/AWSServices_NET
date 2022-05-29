using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;

namespace UsingDynamoDB.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IDynamoDBContext _context;

    public EmployeesController(IDynamoDBContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        await _context.SaveAsync(new EmployeeDocument()
        {
            Id = employee.Id,
            Name = employee.Name,
            Role = employee.Role,
            Team = employee.Team
        });

        return Ok(new { id = employee.Id});
    }

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch([FromBody] IEnumerable<Employee> employees)
    {
        var batch = _context.CreateBatchWrite<EmployeeDocument>();
        foreach (var employee in employees)
        {
            batch.AddPutItem(new EmployeeDocument()
            {
                Id = employee.Id,
                Name = employee.Name,
                Role = employee.Role,
                Team = employee.Team
            });
        }

        await batch.ExecuteAsync();

        return Ok();
    }

    [HttpGet("query")]
    public async Task<IActionResult> GetAllEmployee([FromQuery]string? paginationToken = null, [FromQuery]int? limit = 0)
    {
        var table = _context.GetTargetTable<EmployeeDocument>();
        var scanOps = new ScanOperationConfig();

        if (limit == 0)
            scanOps.Limit = 2;
        else
            scanOps.Limit = limit.Value;

        if (!string.IsNullOrEmpty(paginationToken)) 
            scanOps.PaginationToken = paginationToken;

        var results = table.Scan(scanOps);
        List<Document> data = await results.GetNextSetAsync();
        IEnumerable<EmployeeDocument> employees = _context.FromDocuments<EmployeeDocument>(data);

        return Ok(new EmployeeViewModel() {
            PaginationToken = results.PaginationToken,
            Employees = MapToEmployees(employees),
        });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(string id)
    {
        var result = await _context.LoadAsync<EmployeeDocument>(id);
        return Ok(Map(result));
    }

    [HttpGet("byName/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var search = _context.ScanAsync<EmployeeDocument>(
            new[]
            {
                new ScanCondition
                (
                    nameof(EmployeeDocument.Name),
                    ScanOperator.Equal,
                    name
                )
            });
        var result = await search.GetRemainingAsync();
        if (!result.Any())
            return NotFound();
        
        return Ok(Map(result.First()));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _context.DeleteAsync<EmployeeDocument>(id);
        return Ok();
        
    }

    private IEnumerable<Employee> MapToEmployees(IEnumerable<EmployeeDocument> employees)
    {
        foreach (var employee in employees)
        {
            yield return Map(employee);
        }
    }

    private Employee Map(EmployeeDocument employee) =>
        new (employee.Id, employee.Name, employee.Role, employee.Team);
}