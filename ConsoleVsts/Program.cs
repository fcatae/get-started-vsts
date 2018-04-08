using System;

namespace ConsoleVsts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var vsts = new VisualStudioRepository(args[0], args[1], args[2]);

            //vsts.CreateAsync().Wait();

            //vsts.GetItemAsync(1).Wait();

            //vsts.AddItemRelationAsync(1).Wait();

            //vsts.DiscoverTypesAsync().Wait();

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
