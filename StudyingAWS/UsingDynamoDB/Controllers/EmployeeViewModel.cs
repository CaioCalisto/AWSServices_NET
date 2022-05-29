namespace UsingDynamoDB.Controllers;

public class EmployeeViewModel
{
    public string PaginationToken { get; set; }

    public IEnumerable<Employee> Employees { get; set; }
}