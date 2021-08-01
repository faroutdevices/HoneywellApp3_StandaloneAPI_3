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
    public static class GetCurrentLoggingStatus
    {
        [FunctionName("GetCurrentLoggingStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //I'm holding EnableThermostatQuery value in memory, since the function application
            //will always be loaded, I think this is the smartest way, but test it.

            string responseMessage = "{\"LoggingStatus\":\"status\"}";
            responseMessage = responseMessage.Replace("status", GetReadingFromThermostat.EnableThermostatQuery.ToString());
            
            return new OkObjectResult(responseMessage);
        }
    }
}
