using Amazon.DynamoDBv2.DataModel;

namespace UsingDynamoDB;

[DynamoDBTable("Employees")]
public class EmployeeDocument
{
    [DynamoDBHashKey]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public string Team { get; set; }
}