using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Company.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function.Handlers;

public class OutputHandler
{
    [FunctionName("ProcessEmployeeOutput")]
    public void ProcessEmployeeOutput(
        [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
        [Sql("SELECT * FROM [dbo].[Employees] WHERE LastUpdated > (SELECT MAX(ExtractDate) FROM [dbo].[EmployeeExports])",
            CommandType = System.Data.CommandType.Text,
            ConnectionStringSetting = "SqlConnection")] IEnumerable<Employee> employees,
        [Blob("employee-output/employees-{DateTime}.json", Connection = "StorageConnection")] out string outputBlob,
        [Sql("dbo.EmployeeExports", ConnectionStringSetting = "SqlConnection")] out EmployeeExport newEmployeeExports,
        ILogger log)
    {
        outputBlob = employees.Any() ? outputBlob = JsonConvert.SerializeObject(employees) : null;

        newEmployeeExports = new EmployeeExport
        {
            Id = Guid.NewGuid(),
            ExtractDate = DateTime.Now
        };

        log.LogInformation($"Exported {employees.Count()} employees.");
    }
}
