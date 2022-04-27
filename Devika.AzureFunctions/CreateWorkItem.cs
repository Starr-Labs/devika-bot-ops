using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Devika.ConsoleApp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Devika.DevOps.WorkItems
{
    public static class CreateWorkItem
    {
        [Function("CreateWorkItem")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CreateWorkItem");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!\n");
            
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject(content);
            string project = data.project;
            string type = data.type;
            string title = data.title;

            ConsoleApp.Devika devika = new(new Uri(Environment.GetEnvironmentVariable("OrgUrl")) , Environment.GetEnvironmentVariable("Pat"));

            response.WriteString(devika.CreateNewWorkItem(project, title, type));

            return response;
        }
    }
}
