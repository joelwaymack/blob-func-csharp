using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Company.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function.Handlers;

public class InputHandler
{
    [FunctionName("ProcessEmployeeInput")]
    public void ProcessEmployeeInput(
        [BlobTrigger("employee-input/{name}", Connection = "StorageConnection")] Stream myBlob,
        [Sql("dbo.Employees", ConnectionStringSetting = "SqlConnection")] IAsyncCollector<Employee> newEmployees,
        string name,
        ILogger log)
    {
        var employeesJson = new StreamReader(myBlob).ReadToEnd();
        var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesJson);

        var timestamp = DateTime.Now;
        var tasks = new List<Task>();
        employees.ForEach(e =>
        {
            e.LastUpdated = timestamp;
            tasks.Add(newEmployees.AddAsync(e));
        });

        Task.WaitAll(tasks.ToArray());

        log.LogInformation($"Processed file {name} with {employees.Count} employees.");
    }
}
