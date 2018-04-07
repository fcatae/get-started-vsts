using System;

namespace ConsoleVsts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var vsts = new VisualStudioRepository();

            vsts.GetItemAsync(1, args[0], args[1], args[2]).Wait();
        }
    }
}
