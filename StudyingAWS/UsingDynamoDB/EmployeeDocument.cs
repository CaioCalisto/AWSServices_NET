using Amazon.DynamoDBv2.DataModel;

namespace UsingDynamoDB;

[DynamoDBTable("Employees")]
public class EmployeeDocument
{
    [DynamoDBHashKey]
    public int Id { get; set; }
    
    [DynamoDBRangeKey]
    public string Type { get; set; }
    
    public string Name { get; set; }

    public string Role { get; set; }

    public string Team { get; set; }
}