using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _project;
        private readonly VssConnection _connection;

        public VisualStudioRepository(string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            this._project = project;
            this._connection = connection;
        }

        T GetClient<T>()
            where T: VssHttpClientBase
        {
            return _connection.GetClient<T>();
        }

        public async Task CreateAsync()
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

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
            var t = await client.CreateWorkItemAsync(patchDocument, _project, workType);
        }

        public async Task<WorkItem> GetItemAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id);

            return t;
        }

        public async Task GetDeepItemAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id, expand: WorkItemExpand.Relations);
        }

        public async Task DiscoverTypesAsync()
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var rels = await client.GetRelationTypesAsync();
            var cats = await client.GetWorkItemTypeCategoriesAsync(_project);

            var catNames = cats.SelectMany(c => c.WorkItemTypes).Select(n => n.Name);
        }

        public async Task QueryItemsAsync()
        {
            var wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                    "From WorkItems " +
                    "Where [Work Item Type] = 'Bug' " +
                    "And [System.TeamProject] = '" + _project + "' " +
                    "And [System.State] <> 'Closed' " +
                    "Order By [State] Asc, [Changed Date] Desc"
            };

            var client = GetClient<WorkItemTrackingHttpClient>();

            var result = await client.QueryByWiqlAsync(wiql);
        }

        public async Task AddItemRelationAsync(int parentId)
        {
            var parent = await GetItemAsync(parentId);
            string parentUrl = ((ReferenceLink)parent.Links.Links["self"]).Href;

            var client = GetClient<WorkItemTrackingHttpClient>();

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
            var t = await client.CreateWorkItemAsync(patchDocument, _project, workType);
        }
        
        public async Task UpdateItemAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

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

        public async Task AddComment(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "Comment at " + DateTime.Now.ToShortTimeString()
                }
            );

            var t = await client.UpdateWorkItemAsync(patchDocument, id);
        }
    }
}