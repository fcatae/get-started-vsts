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
            //vsts.GetItemAsync(223265).Wait();

            //vsts.QueryItemsAsync().Wait();

            //vsts.UpdateItemAsync(2).Wait();

            //vsts.AddComment(1).Wait();

            vsts.GetItemTemplate().Wait();
        }
    }
}
