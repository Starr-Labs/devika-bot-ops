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
    public static class GetWorkItemRequestLog
    {
        [Function("GetWorkItemRequestLog")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            // get creatorId from azure function query string parameter
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var creatorId = query["creatorId"];
            // create ado.net query based on creatorId
            string queryString = "SELECT top 5 * FROM WorkItemRequestLog WHERE CreatorId = '" + creatorId + "' order by RequestTimeStamp desc";
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
                    SqlDataReader reader = command.ExecuteReader();
                     var Res =
                        from Dr in reader.Enumerate()
                        select new WorkItemRequestLog 
                        {
                            CreatorId = (string)Dr["CreatorId"],
                            Id = (int)Dr["ID"],
                            WorkItemId = (int)Dr["WorkItemId"],
                            WorkItemTitle = (string)Dr["WorkItemTitle"],
                            WorkItemUrl = (string)Dr["WorkItemUrl"],
                            RequestTimeStamp = (DateTime)Dr["RequestTimeStamp"]
                        };
                    var logs = Res.ToList();
                    response.WriteString(JsonConvert.SerializeObject(logs));
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
