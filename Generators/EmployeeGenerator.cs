using System;
using System.Collections.Generic;
using Bogus;
using Company.Function.Models;

namespace Company.Function.Generators;

public class EmployeeGenerator
{
    private readonly Faker<Employee> employeeFaker;

    public EmployeeGenerator()
    {
        employeeFaker = new Faker<Employee>()
            .RuleFor(o => o.Id, f => Guid.NewGuid())
            .RuleFor(o => o.FirstName, f => f.Name.FirstName())
            .RuleFor(o => o.LastName, f => f.Name.LastName())
            .RuleFor(o => o.Address, f => f.Address.FullAddress())
            .RuleFor(o => o.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));
    }

    public IList<Employee> Generate(int count)
    {
        return employeeFaker.Generate(count);
    }
}