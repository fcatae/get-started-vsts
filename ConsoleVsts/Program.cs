using System;

namespace ConsoleVsts
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string vstsAccount = args[0]; // eg, 'https://<site>.visualstudio.com'
            string vstsProject = args[1]; // eg, 'test'
            string vstsToken = args[2]; // eg, '<token_base64>'

            var devMeetings = new DevMeetingMgr(vstsAccount, vstsProject, vstsToken);

            devMeetings.ListMeetingsAsync().Wait();
        }

        static void Main2(string[] args)
        {
            Console.WriteLine("Hello World!");

            var vsts = new VisualStudioRepository(args[0], args[1], args[2]);

            //vsts.CreateAsync().Wait();

            //vsts.GetItemAsync(1).Wait();

            //vsts.AddItemRelationAsync(1).Wait();

            //vsts.DiscoverTypesAsync().Wait();

            vsts.GetFields().Wait();
            vsts.DiscoverTypesAsync().Wait();

            vsts.GetTagsAsync().Wait();
            vsts.GetItemDefinitionAsync("Activity").Wait();

            vsts.GetItemAsync(957906).Wait();
            vsts.GetItemAsync(957903).Wait();
            vsts.GetItemTemplate("Activity").Wait();

            //vsts.QueryItemsAsync().Wait();

            //vsts.UpdateItemAsync(2).Wait();

            //vsts.AddComment(1).Wait();

            //vsts.GetItemTemplate().Wait();

            //vsts.GetCurrentProfile().Wait();

            //vsts.GetUser().Wait();

            vsts.GetAuthenticatedUser().Wait();
        }
    }
}
