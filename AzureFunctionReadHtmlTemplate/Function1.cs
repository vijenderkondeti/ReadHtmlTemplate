using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunctionReadHtmlTemplate
{
    public static class Function1
    {
        public class account
        {

            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string ExpiryDate { get; set; }
        }
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");


            HttpStatusCode result;
            dynamic body = await req.Content.ReadAsStringAsync();
            string accessKey = Environment.GetEnvironmentVariable("SAccessKey");
            string accountName = Environment.GetEnvironmentVariable("SAccountName");
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("wabiztalkcontainer");
            //await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference("HTMLPage1.html");
            blob.Properties.ContentType = "text/html";
            account e = JsonConvert.DeserializeObject<account>(body);
            string text = blob.DownloadText();
            string EmailBody = text.Replace("Account Name", e.AccountName).Replace("Account Number", e.AccountNumber).Replace("ExpiryDate", e.ExpiryDate);
            result = HttpStatusCode.OK;
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return req.CreateResponse(result, EmailBody);
        }
    }
}
