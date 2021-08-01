using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;


namespace Company.Function
{
    public class GetReadingFromThermostat : DataConnectAzureAbstractClass
    {
        //I'm holding EnableThermostatQuery value in memory, since the function application
        //will always be loaded, I think this is the smartest way, but test it.
        public static bool EnableThermostatQuery = true;

        [FunctionName("GetReadingFromThermostat")]
        public static void Run([TimerTrigger("0 */3 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"HoneywellT6ThermostatGetData Timer trigger function executed at: {DateTime.Now}.");

            if (EnableThermostatQuery)
            {
                RecordEntity record = GetData(); //Connect to Honeywell thermostat and get data
                InsertIntoAzureTable(record);
                log.LogInformation($"HoneywellT6ThermostatGetData got data and inserted into table");
            }
        }

        static RecordEntity GetData()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            string _user = config["Honeywell_User"];
            string _pwd = config["Honeywell_Pwd"];
            string _clientId = config["Honeywell_ClientId"];
            string _clientSecret = config["Honeywell_ClientSecret"];
            string _tokenUrl = config["Honeywell_TokenUrl"];
            string _refreshToken;
            string _accessToken = config["Honeywell_AccessToken"];
            string _url = config["Honeywell_Url"];
            string _newAccessToken = string.Empty;

            using (var client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("username", _user));
                postData.Add(new KeyValuePair<string, string>("password", _pwd));
                postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                postData.Add(new KeyValuePair<string, string>("client_id", _clientId));
                postData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));

                HttpContent content = new FormUrlEncodedContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var responseResult = client.PostAsync(_tokenUrl, content).Result;
                _accessToken = responseResult.Content.ReadAsStringAsync().Result;

                var myDeserializedjson = JsonConvert.DeserializeObject<dynamic>(_accessToken);
                _accessToken = myDeserializedjson["access_token"];
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                HttpResponseMessage result = client.GetAsync(_url).Result;

                string kdkd = result.Content.ReadAsStringAsync().Result;
                var myDeserializedClass2 = JsonConvert.DeserializeObject<dynamic>(kdkd);

                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //////////////RefreshToken(); /* Or reenter resource owner credentials if refresh token is not implemented */
                    //////if (/* token refreshed, repeat the request using the new access token */)
                    //////{
                    //////    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _newAccessToken);

                    //////    result = client.GetAsync(_url).Result;

                    //////    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    //////    {
                    //////        // Process the error
                    //////    }
                    //////}
                }

                int locationID = myDeserializedClass2[0].locationID;
                string deviceID = myDeserializedClass2[0].devices[0].deviceID;
                double indoorTemperature = myDeserializedClass2[0].devices[0].indoorTemperature;
                double outdoorTemperature = myDeserializedClass2[0].devices[0].outdoorTemperature;
                bool isAlive = myDeserializedClass2[0].devices[0].isAlive;
                string mode = myDeserializedClass2[0].devices[0].operationStatus.mode;
                string fanRequest = myDeserializedClass2[0].devices[0].operationStatus.fanRequest;
                string circulationFanRequest = myDeserializedClass2[0].devices[0].operationStatus.circulationFanRequest;

                RecordEntity _record = new RecordEntity();
                _record.PartitionKey = "HoneywellRecord";
                _record.RowKey = Guid.NewGuid().ToString();
                _record.indoorTemperature = indoorTemperature.ToString();
                _record.outdoorTemperature = outdoorTemperature.ToString();
                _record.mode = mode;
                _record.fanRequest = fanRequest;
                _record.circulationFanRequest = circulationFanRequest;

                return _record;
            }
        }

        private static bool InsertIntoAzureTable(RecordEntity record)
        {
            return InsertIntoAzureTable(record, true);
        }

        static Boolean InsertIntoAzureTable(RecordEntity record, bool v)
        {
            Boolean bSuccess = false;

            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["AzureTableConnection"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("HoneywellThermostatReadings");
            table.CreateIfNotExistsAsync();
            TableOperation insertOperation = TableOperation.Insert(record);

            try
            {
                table.ExecuteAsync(insertOperation);
                bSuccess = v;
            }
            catch (Exception ex)
            {
                bSuccess = false;
            }

            return bSuccess;
        }
    }

}
