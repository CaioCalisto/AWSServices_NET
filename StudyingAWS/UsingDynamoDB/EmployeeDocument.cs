using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace UsingDynamoDB;

[DynamoDBTable("Employees")]
public class EmployeeDocument : Document
{
    [DynamoDBHashKey]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public string Team { get; set; }
}