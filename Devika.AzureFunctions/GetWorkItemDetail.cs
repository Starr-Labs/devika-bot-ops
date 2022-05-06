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
    public static class GetWorkItemDetail
    {
        [Function("GetWorkItemDetail")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Get workitem detail");
            logger.LogInformation("Get workitem detail.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            //get title from azure function query string parameter
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var id = query["id"];
            var idArr = id.Split(" ");
            id = idArr[0];
            id = id.Trim().Replace(":", "");
            
            if (string.IsNullOrEmpty(id))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.WriteString("Please provide an id.");
                return response;
            }
            
            
            //search for work item by title
            Devika.ClassLib.Devika devika = new(new Uri(
                Environment.GetEnvironmentVariable("OrgUrl")) , 
                Environment.GetEnvironmentVariable("Pat"));
            var item = await devika.FindWorkItemById(id);
            if (null != item)
            {
                //return json array 
                var json = (string)JsonConvert.SerializeObject(item);
                response.WriteString(json);
            }
            else
            {
                response.WriteString("");
            }

            return response;
        }
    }
}
