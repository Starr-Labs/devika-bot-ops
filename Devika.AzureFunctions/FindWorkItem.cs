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
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace Devika.DevOps.WorkItems
{
    public static class FindWorkItem
    {
        [Function("FindWorkItem")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CreateWorkItem");
            logger.LogInformation("Find workitem request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            //get title from azure function query string parameter
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var title = query["title"];

            if (string.IsNullOrEmpty(title))
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(content);
                title = data.titleFragment;
            }

            if (string.IsNullOrEmpty(title))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.WriteString("Please provide a title fragment.");
                return response;
            }
            
            
            //search for work item by title
            Devika.ClassLib.Devika devika = new(new Uri(Environment.GetEnvironmentVariable("OrgUrl")) , Environment.GetEnvironmentVariable("Pat"));
            var items = await devika.FindWorkItemByTitle(title);
            if (null != items)
            {
                //return json array 
                response.WriteString(JsonConvert.SerializeObject(items));
            }
            //return empty array
            else
            {
                response.WriteString(JsonConvert.SerializeObject(new List<WorkItemReference>()));
            }

            return response;
        }
    }
}
