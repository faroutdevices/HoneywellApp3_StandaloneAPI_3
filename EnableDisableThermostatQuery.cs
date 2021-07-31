using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class EnableDisableThermostatQuery
    {
        [FunctionName("EnableDisableThermostatQuery")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "SetThermostatLogging/status/{changeToStatus}")] HttpRequest req,
            ILogger log, string changeToStatus)
        {
            log.LogInformation("EnableDisableThermostatQuery processed a request.");

            if (changeToStatus == "0") { GetReadingFromThermostat.EnableThermostatQuery = false; }
            if (changeToStatus == "1") { GetReadingFromThermostat.EnableThermostatQuery = true; }

            string responseMessage = "{\"LoggingStatus\": \"" + GetReadingFromThermostat.EnableThermostatQuery.ToString() + "\"}";

            return new OkObjectResult(responseMessage);
        }
    }




    public class ThermLoggingStatus
    {
       public bool LoggingStatus { get; set; }
    }
}
