using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Devika.ClassLib
{
    public class Devika
    {
        public VssConnection Connection { get; private set; }

        public Devika(Uri orgUrl, string personalAccessToken)
        {
            Connection = new(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));
        }

        public async Task<WorkItem> ShowWorkItemDetails(int workItemId)
        {
            WorkItem workitem = null;
            // Get an instance of the work item tracking client
            WorkItemTrackingHttpClient witClient = Connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                // Get the specified work item
                workitem = await witClient.GetWorkItemAsync(workItemId);

                // Output the work item's field values
                foreach (var field in workitem.Fields)
                {
                    Console.WriteLine("  {0}: {1}", field.Key, field.Value);
                }
            }
            catch (AggregateException aex)
            {
                VssServiceException vssex = aex.InnerException as VssServiceException;
                if (vssex != null)
                {
                    Console.WriteLine(vssex.Message);
                }
            }

            return workitem;
        }


        public async Task<string> CreateNewWorkItem(string project, string title, string type)
        {
            // Construct the object containing field values required for the new work item
            JsonPatchDocument patchDocument = new()
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            };

            WorkItemTrackingHttpClient witClient = Connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                // Create the new work item
                WorkItem newWorkItem = await witClient.CreateWorkItemAsync(patchDocument, 
                    project, type);

                string result = String.Format("Created a '{0}' work item ID '{1}' Title '{2}'", type, newWorkItem.Id, newWorkItem.Fields["System.Title"]);
                Console.WriteLine(result);
                return result;
                //return newWorkItem;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Error creating bug: {0}", ex.InnerException.Message);
                return null;
            }
        }

        public WorkItemDelete DeleteWorkItem(int id)
        {
            //int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);

            // Get a client
            WorkItemTrackingHttpClient workItemTrackingClient = Connection.GetClient<WorkItemTrackingHttpClient>();

            // Delete the work item (but don't destroy it completely)
            WorkItemDelete results = workItemTrackingClient.DeleteWorkItemAsync(id, destroy: false).Result;

            Console.WriteLine(results);

            return results;
        }

        public async Task<List<dynamic>> FindWorkItemByTitle(string titleFragment)
        {
            var teamProjectName = "TestProject";
            // Get a client
            WorkItemTrackingHttpClient witClient = Connection.GetClient<WorkItemTrackingHttpClient>();
            // use client to query for work items by title
            WorkItemQueryResult workItemQueryResult = await witClient.QueryByWiqlAsync(new Wiql()
            {
                Query = $"Select * From WorkItems Where [System.TeamProject] = '{teamProjectName}'"+
                " AND [System.State] <> 'Removed' AND [System.Title] contains '" + titleFragment + "'"
            });
            // convert result to list of work items
            var workItemReferences = workItemQueryResult.WorkItems.ToList();
            // get list of work item fields from list of work item references
            var workItemDetails = new List<dynamic>();
            foreach (var workItemReference in workItemReferences)
            {
                var workItem = await witClient.GetWorkItemAsync(workItemReference.Id);
                dynamic workItemDetail = new
                {
                    Id = workItem.Id,
                    Title = workItem.Fields["System.Title"],
                    State = workItem.Fields["System.State"],
                    Description = workItem.Fields["System.Description"]
                };
                workItemDetails.Add(workItemDetail);
            }

            return workItemDetails;
        }
    }


}
