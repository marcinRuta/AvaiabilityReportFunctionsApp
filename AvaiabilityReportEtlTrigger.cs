using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GymAvaiability
{
    public class AvaiabilityReportEtlTrigger
    {
        private readonly ILogger _logger;

        public AvaiabilityReportEtlTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AvaiabilityReportEtlTrigger>();
        }

        [Function("AvaiabilityReportEtlTrigger")]
        public void Run([TimerTrigger("0 0 23 * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
             using (HttpClient client = new HttpClient())
            {
                // Specify the URL of the endpoint you want to call
                string endpointUrl = "https://avaiabilityreportapi.azurewebsites.net/api/AvaiabilityReport";
                
                // Make the HTTP GET request
                HttpResponseMessage response = client.GetAsync(endpointUrl).Result;
                if( response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Success: {response.StatusCode}");
                    if(response.Content != null)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        _logger.LogInformation(responseContent);
                    }
                }
                else
                {
                    _logger.LogInformation($"Error: {response.StatusCode}");
                }

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }

}
}