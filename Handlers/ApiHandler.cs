using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Company.Function.Generators;
using System.Text;

namespace Company.Function.Handlers;

public static class ApiHandler
{
    private static readonly EmployeeGenerator employeeGenerator = new EmployeeGenerator();

    [FunctionName("GenerateEmployeeInput")]
    public static IActionResult GenerateEmployeeInput(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employees")] HttpRequest req,
        [Blob("employee-input/employees-{DateTime}.json", FileAccess.Write, Connection = "StorageConnection")] TextWriter outputBlob,
        ILogger log)
    {
        string strCount = req.Query["count"];
        var count = 10;

        if (int.TryParse(strCount, out int numberOfEmployees))
        {
            count = numberOfEmployees > 0 ? numberOfEmployees : count;
        }

        var employees = employeeGenerator.Generate(count);
        outputBlob.Write(JsonConvert.SerializeObject(employees));

        var message = $"Generating a new employee file with {count} employees.";
        log.LogInformation(message);
        return new OkObjectResult(message);
    }
}
