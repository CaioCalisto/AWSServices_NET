namespace UsingDynamoDB;

public class Employee
{
    public string Name { get; set; }

    public string Role { get; set; }

    public string Team { get; set; }

    public Employee(string name, string role, string team)
    {
        Name = name;
        Role = role;
        Team = team;
    }
}