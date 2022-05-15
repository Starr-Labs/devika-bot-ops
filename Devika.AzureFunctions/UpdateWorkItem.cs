using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Devika.ClassLib;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Devika.DevOps.WorkItems
{
    public static class UpdateWorkItem
    {
        [Function("UpdateWorkItem")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("UpdateWorkItem");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject(content);
            string project = data.project;
            string id = data.workItemID;
            string name = data.workItemPropName;
            string value = data.workItemPropValue;
            
            logger.LogInformation($"Going to update {name} with value {value} for "+
                $"{project} with id {id}");
                
            Devika.ClassLib.Devika devika = new(
                new Uri(Environment.GetEnvironmentVariable("OrgUrl")) , 
                Environment.GetEnvironmentVariable("Pat"));

            var result = await devika.UpdateWorkItem(
                project, id, name, value);

            response.WriteString(result);

            return response;
        }
    }
}
