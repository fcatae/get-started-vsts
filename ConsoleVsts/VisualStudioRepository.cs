using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace ConsoleVsts
{
    class VisualStudioRepository
    {
        public async Task CreateAsync(string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "meu titulo"
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.AssignedTo",
                    Value = "fcatae@microsoft.com"
                }
            );


            string workType = "Bug";
            var t = await client.CreateWorkItemAsync(patchDocument, project, workType);
        }

        public async Task<WorkItem> GetItemAsync(int id, string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id);

            return t;
        }

        public async Task GetDeepItemAsync(int id, string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id, expand: WorkItemExpand.Relations);
        }

        public async Task DiscoverTypesAsync(string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var rels = await client.GetRelationTypesAsync();
            var cats = await client.GetWorkItemTypeCategoriesAsync(project);
        }

        public async Task QueryItemsAsync(string vstsAccount, string project, string personalAccessToken)
        {
            var wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                    "From WorkItems " +
                    "Where [Work Item Type] = 'Bug' " +
                    "And [System.TeamProject] = '" + project + "' " +
                    "And [System.State] <> 'Closed' " +
                    "Order By [State] Asc, [Changed Date] Desc"
            };

            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var result = await client.QueryByWiqlAsync(wiql);
        }

        public async Task AddItemRelationAsync(int parentId, string vstsAccount, string project, string personalAccessToken)
        {
            var parent = await GetItemAsync(parentId, vstsAccount, project, personalAccessToken);
            string parentUrl = ((ReferenceLink)parent.Links.Links["self"]).Href;

            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Linked task"
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "System.LinkTypes.Hierarchy-Reverse",
                        url = parentUrl
                    }
                }
            );

            string workType = "Task";
            var t = await client.CreateWorkItemAsync(patchDocument, project, workType);
        }
        
        public async Task UpdateItemAsync(int id, string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                //new JsonPatchOperation()
                //{
                //    Operation = Operation.Add,
                //    Path = "/fields/System.Title",
                //    Value = "Updated task"
                //}
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.State",
                    Value = "Closed"
                }
            );

            var t = await client.UpdateWorkItemAsync(patchDocument, id);
        }
    }
}