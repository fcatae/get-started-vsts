using System;

namespace ConsoleVsts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var vsts = new VisualStudioRepository();

            //vsts.CreateAsync(args[0], args[1], args[2]).Wait();

            //vsts.GetItemAsync(1, args[0], args[1], args[2]).Wait();

            //vsts.AddItemRelationAsync(1, args[0], args[1], args[2]).Wait();

            //vsts.DiscoverTypesAsync(args[0], args[1], args[2]).Wait();

            //vsts.QueryItemsAsync(args[0], args[1], args[2]).Wait();

            vsts.UpdateItemAsync(2, args[0], args[1], args[2]).Wait();
        }
    }
}
