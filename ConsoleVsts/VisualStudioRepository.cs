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
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.TeamFoundation.Core.WebApi;

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

        public async Task GetItemDefinitionAsync(string workType)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemTypeAsync(_project, workType);
        }

        public async Task<WorkItem> GetItemAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id);

            return t;
        }

        public async Task<WorkItem> GetDeepItemAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id, expand: WorkItemExpand.Relations);
            
            return t;
        }

        public async Task<VsTaskItem[]> GetChildItemsAsync(int id)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var queryChild = new Wiql
            {
                Query = $"SELECT [System.Id] FROM WorkItemLinks" +
                          $"  WHERE ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') " +
                          $"  AND ([Source.Id] = {id})" +
                          $"  ORDER BY [System.Id]"
            };
            
            var results = await client.QueryByWiqlAsync(queryChild, _project);

            var resultIds = results.WorkItemRelations
                                .Where(w => w.Rel != null)
                                .Select(w => w.Target.Id);

            var fields = new string[] {
                "System.Id",
                "System.Title",
                "System.State",
                "System.AssignedTo",
                "CSEngineering.ActivityStartDate",
                "CSEngineering.ActivityDuration"
            };

            var childItemsAll = await client.GetWorkItemsAsync(resultIds);
            var childItems = await client.GetWorkItemsAsync(resultIds, fields);

            return childItems.Select(t => new VsTaskItem
            {
                Id = (int)t.Id,
                Title = (string)t.Fields["System.Title"],
                State = (string)t.Fields["System.State"],
                AssignedTo = (string)t.Fields["System.AssignedTo"],
                StartDate = (DateTime?)t.Fields["CSEngineering.ActivityStartDate"],
                Duration = (float)(double)t.Fields["CSEngineering.ActivityDuration"]
            }).ToArray();
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


        public async Task GetItemTemplate(string template)
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemTemplateAsync(_project, template);
        }

        public async Task GetCurrentProfile()
        {
            var client = GetClient<ProfileHttpClient>();

            var name = await client.GetDisplayNameAsync(null);
            //var avatar = await client.GetAvatarAsync(AvatarSize.Large);
            //System.IO.File.WriteAllBytes("ex.png", avatar.Value);
            
            var attrib = await client.GetAttributesAsync(new AttributesQueryContext(AttributesScope.Core));
            var profile = await client.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
        }

        public async Task GetAuthenticatedUser()
        {
            var connection = _connection;

            await connection.ConnectAsync();

            var auth = connection.AuthorizedIdentity;

            var id = auth.Descriptor.Identifier;
            var account = auth.Properties.GetValue("account", "");
            var claimType = auth.Descriptor.IdentityType;
        }


        public async Task GetTagsAsync()
        {
            var projClient = GetClient<ProjectHttpClient>();
            var projId = (await projClient.GetProject(_project)).Id;

            var client = GetClient<TaggingHttpClient>();

            var tags = await client.GetTagsAsync(projId);            
        }

        public async Task GetFields()
        {
            var client = GetClient<WorkItemTrackingHttpClient>();

            var areas = await client.GetClassificationNodeAsync(_project, TreeStructureGroup.Areas, depth: 100 );
            ShowNodeTree(areas);

            var a = await client.GetFieldAsync("System.AreaPath");

            var f = await client.GetFieldsAsync();
        }

        private void ShowNodeTree(WorkItemClassificationNode node, string path = "")
        {
            path = path + "/" + node.Name;
            Console.WriteLine(path);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    ShowNodeTree(child, path);
                }
            }
        }
    }
}