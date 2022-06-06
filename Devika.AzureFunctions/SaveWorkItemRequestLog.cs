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
using Microsoft.Azure.WebJobs;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Devika.DevOps.WorkItems
{
    public static class SaveWorkItemRequestLog
    {
        [Function("SaveWorkItemRequestLog")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SaveWorkItemRequestLog");

            var response = req.CreateResponse(HttpStatusCode.OK);
            //parse workitemrequestlog from request body
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            logger.LogInformation($"Work item request log: {content}");
            
            dynamic data = JsonConvert.DeserializeObject(content);
            var workItemRequestLog 
                = JsonConvert.DeserializeObject<WorkItemRequestLog>(
                    content);

            // create ado.net query based on creatorId
            string queryString = "insert into [dbo].[WorkItemRequestLog] "+
            "(creatorid, workitemid, workitemtitle, workitemurl, requesttimestamp) values ("+
            $"'{workItemRequestLog.CreatorId}', {workItemRequestLog.WorkItemId}, "+
            $"'{workItemRequestLog.WorkItemTitle}', '{workItemRequestLog.WorkItemUrl}', getdate())";

            // get connection string from azuure function config
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            // create ado.net connection
            var connectionString = config.GetConnectionString("SqlConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    //response.WriteString(ex.Message);
                }
            }
            return response;
        }
        
    }
}
