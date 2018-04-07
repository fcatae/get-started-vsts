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

        public async Task GetItemAsync(int id, string vstsAccount, string project, string personalAccessToken)
        {
            var accountUri = new Uri(vstsAccount);
            var connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));

            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var t = await client.GetWorkItemAsync(id);
        }
    }
}
