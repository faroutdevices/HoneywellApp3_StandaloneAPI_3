using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;
using System.Linq;


namespace Company.Function
{
    public abstract class DataConnectAzureAbstractClass
    {
       //public static string tempString;

        //Most of the data table connecting logic is in GetReadingFromThermostat class
        //if expanding functionality to other classes, will do so here.
    }
}
