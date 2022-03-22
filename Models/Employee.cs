using System;

namespace Company.Function.Models;

public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public DateTime? LastUpdated { get; set; }
}