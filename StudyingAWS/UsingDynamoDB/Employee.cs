namespace UsingDynamoDB;

public class Employee
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string Role { get; set; }

    public string Team { get; set; }

    public Employee(string id, string name, string role, string team)
    {
        Id = id;
        Name = name;
        Role = role;
        Team = team;
    }
}